using KleeneStar.Core;
using KleeneStar.Model.Entities;
using System;
using System.Linq;

namespace KleeneStar.Automation.WebAutomation.Actions
{
    /// <summary>
    /// <c>approval &lt;policy&gt;</c> - requests an approval of the object under a policy of the
    /// rule's workspace.
    /// </summary>
    /// <remarks>
    /// An approval already pending under the policy counts as done, which also makes a resumed
    /// job request nothing twice.
    /// </remarks>
    public sealed class ApprovalAction : IAutomationAction
    {
        /// <inheritdoc/>
        public string Key => "approval";

        /// <inheritdoc/>
        public string Syntax => "kleenestar.automation:automation.action.approval";

        /// <inheritdoc/>
        public string Validate(string argument, Class @class)
        {
            return string.IsNullOrWhiteSpace(argument) ? "name the approval policy." : null;
        }

        /// <inheritdoc/>
        public string Validate(string argument, Class @class, Guid workspaceId)
        {
            if (Validate(argument, @class) is { } problem || workspaceId == Guid.Empty)
            {
                return Validate(argument, @class);
            }

            var policy = Find(workspaceId, argument);

            if (policy is null)
            {
                return $"the workspace has no approval policy '{argument.Trim()}'.";
            }

            return @class is not null && policy.ClassId != @class.Id
                ? "the policy approves objects of another class."
                : null;
        }

        /// <inheritdoc/>
        public void Execute(AutomationActionContext context, string argument)
        {
            var policy = Find(context.Rule.WorkspaceId, argument)
                ?? throw new InvalidOperationException($"The workspace has no approval policy '{argument?.Trim()}'.");

            if (CoreHub.ApprovalManager.GetApprovals(context.Object.Id).Any(x => x.PolicyId == policy.Id && x.State == ApprovalState.Pending))
            {
                return;
            }

            CoreHub.ApprovalManager.Request(context.Object.Id, policy.Id, context.ActorId, null);
        }

        /// <summary>
        /// Finds a policy of a workspace by name.
        /// </summary>
        /// <param name="workspaceId">The workspace.</param>
        /// <param name="name">The name.</param>
        /// <returns>The policy, or <see langword="null"/>.</returns>
        private static ApprovalPolicy Find(Guid workspaceId, string name)
        {
            return CoreHub.ApprovalManager
                .GetPolicies(workspaceId)
                .FirstOrDefault(x => string.Equals(x.Name, name?.Trim(), StringComparison.OrdinalIgnoreCase));
        }
    }
}
