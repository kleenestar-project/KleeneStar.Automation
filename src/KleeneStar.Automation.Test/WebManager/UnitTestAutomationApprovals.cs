using KleeneStar.Core.WebApproval;
using KleeneStar.Automation.WebAutomation;
using KleeneStar.Automation.WebManager;
using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebWorkflow;
using KleeneStar.Model;
using KleeneStar.Model.Entities;
using ObjectEntity = KleeneStar.Model.Entities.Object;

namespace KleeneStar.Automation.Test.WebManager
{
    /// <summary>
    /// Tests multi-step approvals: steps are passed in order and decided by their approvers -
    /// one, all, or a quorum, accounts or group members; one rejection ends the approval and
    /// needs a reason; the requester does not decide; a step whose condition does not hold is
    /// passed over; every decision is kept; the outcome moves the object through its workflow,
    /// and reaching the request state asks for the approval.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestAutomationApprovals
    {
        private static readonly Guid WorkspaceId = Guid.Parse("C0C0C000-0000-4000-8000-000000000001");
        private static readonly Guid ClassId = Guid.Parse("C0C0C000-0000-4000-8000-000000000002");
        private static readonly Guid WorkflowId = Guid.Parse("C0C0C000-0000-4000-8000-000000000003");
        private static readonly Guid FieldId = Guid.Parse("C0C0C000-0000-4000-8000-000000000004");
        private static readonly Guid CategoryId = Guid.Parse("C0C0C000-0000-4000-8000-000000000005");
        private static readonly Guid DraftId = Guid.Parse("C0C0C000-0000-4000-8000-000000000006");
        private static readonly Guid AwaitingId = Guid.Parse("C0C0C000-0000-4000-8000-000000000007");
        private static readonly Guid ApprovedId = Guid.Parse("C0C0C000-0000-4000-8000-000000000008");
        private static readonly Guid RejectedId = Guid.Parse("C0C0C000-0000-4000-8000-000000000009");
        private static readonly Guid RequesterId = Guid.Parse("C0C0C000-0000-4000-8000-000000000011");
        private static readonly Guid ManagerId = Guid.Parse("C0C0C000-0000-4000-8000-000000000012");
        private static readonly Guid BoardOneId = Guid.Parse("C0C0C000-0000-4000-8000-000000000013");
        private static readonly Guid BoardTwoId = Guid.Parse("C0C0C000-0000-4000-8000-000000000014");
        private static readonly Guid OutsiderId = Guid.Parse("C0C0C000-0000-4000-8000-000000000015");
        private static readonly Guid CabId = Guid.Parse("C0C0C000-0000-4000-8000-000000000016");

        /// <summary>
        /// Seeds a change class with the workflow Draft → Awaiting approval → Approved | Rejected,
        /// five accounts and the group CAB with two of them.
        /// </summary>
        /// <param name="database">The per-test in-memory database name.</param>
        private static void Seed(string database)
        {
            AutomationHubFixture.Initialize(database);

            using var db = AutomationHubFixture.CreateDbContext(database);

            if (db.Workspaces.Any(x => x.Id == WorkspaceId))
            {
                return;
            }

            db.Workspaces.Add(new Workspace { Id = WorkspaceId, Key = "CHG", Name = "Changes" });
            db.Classes.Add(new Class { Id = ClassId, Name = "Change", WorkspaceId = WorkspaceId });
            db.StatusCategories.Add(new StatusCategory { Id = CategoryId, Name = "ToDo", Color = "#888888", IsDefault = true });
            db.Statuses.AddRange
            (
                new Status { Id = DraftId, Name = "Draft", ClassId = ClassId, CategoryId = CategoryId, State = StatusState.Active },
                new Status { Id = AwaitingId, Name = "Awaiting approval", ClassId = ClassId, CategoryId = CategoryId, State = StatusState.Active },
                new Status { Id = ApprovedId, Name = "Approved", ClassId = ClassId, CategoryId = CategoryId, State = StatusState.Active },
                new Status { Id = RejectedId, Name = "Rejected", ClassId = ClassId, CategoryId = CategoryId, State = StatusState.Active }
            );
            db.Workflows.Add(new Workflow
            {
                Id = WorkflowId,
                Name = "Change lifecycle",
                ClassId = ClassId,
                State = WorkflowState.Active,
                WorkflowStatuses =
                [
                    new WorkflowStatus { StatusId = DraftId, IsStart = true },
                    new WorkflowStatus { StatusId = AwaitingId },
                    new WorkflowStatus { StatusId = ApprovedId, IsEnd = true },
                    new WorkflowStatus { StatusId = RejectedId, IsEnd = true }
                ]
            });
            db.Transitions.AddRange
            (
                new Transition { Name = "Submit", WorkflowId = WorkflowId, SourceId = DraftId, TargetId = AwaitingId, State = TransitionState.Active },
                new Transition { Name = "Approve", WorkflowId = WorkflowId, SourceId = AwaitingId, TargetId = ApprovedId, State = TransitionState.Active },
                new Transition { Name = "Reject", WorkflowId = WorkflowId, SourceId = AwaitingId, TargetId = RejectedId, State = TransitionState.Active }
            );
            db.Fields.Add(new Field { Id = FieldId, Name = "Status", ClassId = ClassId, FieldType = FieldType.Workflow, WorkflowId = WorkflowId, State = FieldState.Active });

            Identity Account(Guid id, string name) => new() { Id = id, Name = name, UserName = name, Email = $"{name}@example.com", PasswordHash = "x" };

            var requester = Account(RequesterId, "requester");
            var manager = Account(ManagerId, "manager");
            var boardOne = Account(BoardOneId, "board.one");
            var boardTwo = Account(BoardTwoId, "board.two");
            var outsider = Account(OutsiderId, "outsider");
            var cab = new Group { Id = CabId, Name = "CAB" };

            db.Identities.AddRange(requester, manager, boardOne, boardTwo, outsider);
            db.Groups.Add(cab);
            db.Set<IdentityGroupMembership>().AddRange
            (
                new IdentityGroupMembership { Identity = boardOne, Group = cab },
                new IdentityGroupMembership { Identity = boardTwo, Group = cab }
            );
            db.SaveChanges();
        }

        /// <summary>
        /// Creates a change.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="summary">The summary.</param>
        /// <returns>The object id.</returns>
        private static Guid Change(string database, string summary)
        {
            var id = Guid.NewGuid();

            using var db = AutomationHubFixture.CreateDbContext(database);

            db.Objects.Add(new ObjectEntity(id) { Key = "CHG-" + Random.Shared.Next(1000, 9999), Summary = summary, WorkspaceId = WorkspaceId, ClassId = ClassId });
            db.SaveChanges();

            return id;
        }

        /// <summary>
        /// Adds a policy.
        /// </summary>
        /// <param name="steps">The steps.</param>
        /// <param name="configure">Changes the policy before it is added.</param>
        /// <returns>The policy.</returns>
        private static ApprovalPolicy Policy(string steps, Action<ApprovalPolicy> configure = null)
        {
            var policy = new ApprovalPolicy
            {
                Name = "policy " + Guid.NewGuid().ToString("N")[..6],
                WorkspaceId = WorkspaceId,
                ClassId = ClassId,
                Steps = steps,
                State = ApprovalPolicyState.Active
            };

            configure?.Invoke(policy);
            CoreHub.ApprovalManager.AddPolicy(policy);

            return policy;
        }

        /// <summary>
        /// A step that takes longer than it may raises <c>approval.overdue</c> once, and a rule
        /// reacting to it is queued once.
        /// </summary>
        [Fact]
        public void Overdue_IsRaisedOncePerStep()
        {
            var database = nameof(Overdue_IsRaisedOncePerStep);
            Seed(database);

            // the core announces the overdue step as ApprovalOccurred; the manager hears it once connected
            AutomationHub.AutomationManager.Connect();

            var policy = Policy("Board: any group:CAB within 4h");
            var rule = new AutomationRule
            {
                Name = "escalate overdue",
                WorkspaceId = WorkspaceId,
                Trigger = AutomationTriggerManager.ApprovalOverdue,
                Actions = "escalate",
                RunAsId = ManagerId,
                State = AutomationRuleState.Active
            };

            AutomationHub.AutomationManager.AddRule(rule);

            var approval = CoreHub.ApprovalManager.Request(Change(database, "Slow board"), policy.Id, RequesterId, null);

            Assert.Equal(0, CoreHub.ApprovalManager.RaiseOverdue(DateTime.UtcNow.AddHours(3)));
            Assert.Equal(1, CoreHub.ApprovalManager.RaiseOverdue(DateTime.UtcNow.AddHours(5)));
            Assert.Equal(1, CoreHub.ApprovalManager.RaiseOverdue(DateTime.UtcNow.AddHours(6)));

            Assert.Single(ModelHub.GetAutomationJobs([rule.Id], null, 10));
            Assert.Equal(ApprovalState.Pending, CoreHub.ApprovalManager.GetApproval(approval.Id).State);
        }

        /// <summary>
        /// A rule's <c>approval</c> line names a policy of its workspace and class.
        /// </summary>
        [Fact]
        public void ApprovalAction_IsValidatedAgainstThePolicies()
        {
            Seed(nameof(ApprovalAction_IsValidatedAgainstThePolicies));

            var policy = Policy("Manager: any manager");

            Assert.Empty(AutomationHub.AutomationManager.Validate(new AutomationRule { Name = "ok", WorkspaceId = WorkspaceId, Trigger = AutomationTriggerManager.ObjectCreated, Actions = $"approval {policy.Name}" }));
            Assert.Contains(AutomationHub.AutomationManager.Validate(new AutomationRule { Name = "no", WorkspaceId = WorkspaceId, Trigger = AutomationTriggerManager.ObjectCreated, Actions = "approval nonexistent" }), x => x.StartsWith("Line 1:", StringComparison.Ordinal));
        }

    }
}
