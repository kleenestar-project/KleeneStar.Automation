using KleeneStar.Automation.WebManager;

namespace KleeneStar.Automation.WebAutomation.Triggers
{
    /// <summary>
    /// A trigger the platform ships: a key, the i18n keys of its label and of its parameter, and
    /// whether the scheduler evaluates it. Every trigger is a <c>public sealed</c> class with a
    /// parameterless constructor, which is what <see cref="AutomationTriggerManager"/> finds - in
    /// this assembly and in every plugin that ships one.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="label">The i18n key of the label.</param>
    /// <param name="parameter">The i18n key describing the parameter, or <see langword="null"/>.</param>
    /// <param name="scheduled">Whether the scheduler evaluates the trigger.</param>
    public abstract class AutomationTrigger(string key, string label, string parameter, bool scheduled = false) : IAutomationTrigger
    {
        /// <inheritdoc/>
        public string Key => key;

        /// <inheritdoc/>
        public string Label => label;

        /// <inheritdoc/>
        public string Parameter => parameter;

        /// <inheritdoc/>
        public bool Scheduled => scheduled;
    }

    /// <summary>
    /// An object was created.
    /// </summary>
    public sealed class ObjectCreatedTrigger : AutomationTrigger
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ObjectCreatedTrigger()
            : base(AutomationTriggerManager.ObjectCreated, "kleenestar.automation:automation.trigger.object.created", null)
        {
        }
    }

    /// <summary>
    /// An object was changed; the parameter narrows to the attributes that changed.
    /// </summary>
    public sealed class ObjectUpdatedTrigger : AutomationTrigger
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ObjectUpdatedTrigger()
            : base(AutomationTriggerManager.ObjectUpdated, "kleenestar.automation:automation.trigger.object.updated", "kleenestar.automation:automation.trigger.object.updated.parameter")
        {
        }
    }

    /// <summary>
    /// A workflow transition was executed; the parameter narrows to the target state.
    /// </summary>
    public sealed class TransitionExecutedTrigger : AutomationTrigger
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public TransitionExecutedTrigger()
            : base(AutomationTriggerManager.TransitionExecuted, "kleenestar.automation:automation.trigger.transition.executed", "kleenestar.automation:automation.trigger.transition.executed.parameter")
        {
        }
    }

    /// <summary>
    /// A comment was added; the parameter narrows to its audience.
    /// </summary>
    public sealed class CommentAddedTrigger : AutomationTrigger
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public CommentAddedTrigger()
            : base(AutomationTriggerManager.CommentAdded, "kleenestar.automation:automation.trigger.comment.added", "kleenestar.automation:automation.trigger.comment.added.parameter")
        {
        }
    }

    /// <summary>
    /// A service level target passed its warning threshold (evaluated by the scheduler); the parameter narrows to the target.
    /// </summary>
    public sealed class SlaAtRiskTrigger : AutomationTrigger
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public SlaAtRiskTrigger()
            : base(AutomationTriggerManager.SlaAtRisk, "kleenestar.automation:automation.trigger.sla.atrisk", "kleenestar.automation:automation.trigger.sla.atrisk.parameter", scheduled: true)
        {
        }
    }

    /// <summary>
    /// A service level target was missed (evaluated by the scheduler); the parameter narrows to the target.
    /// </summary>
    public sealed class SlaBreachedTrigger : AutomationTrigger
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public SlaBreachedTrigger()
            : base(AutomationTriggerManager.SlaBreached, "kleenestar.automation:automation.trigger.sla.breached", "kleenestar.automation:automation.trigger.sla.breached.parameter", scheduled: true)
        {
        }
    }

    /// <summary>
    /// An approval was requested (raised by the core); the parameter narrows to the policy.
    /// </summary>
    public sealed class ApprovalRequestedTrigger : AutomationTrigger
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ApprovalRequestedTrigger()
            : base(AutomationTriggerManager.ApprovalRequested, "kleenestar.automation:automation.trigger.approval.requested", "kleenestar.automation:automation.trigger.approval.requested.parameter")
        {
        }
    }

    /// <summary>
    /// An approval ended (raised by the core); the parameter narrows to the policy.
    /// </summary>
    public sealed class ApprovalCompletedTrigger : AutomationTrigger
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ApprovalCompletedTrigger()
            : base(AutomationTriggerManager.ApprovalCompleted, "kleenestar.automation:automation.trigger.approval.completed", "kleenestar.automation:automation.trigger.approval.completed.parameter")
        {
        }
    }

    /// <summary>
    /// A step of an approval took longer than it may (raised by the core).
    /// </summary>
    public sealed class ApprovalOverdueTrigger : AutomationTrigger
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ApprovalOverdueTrigger()
            : base(AutomationTriggerManager.ApprovalOverdue, "kleenestar.automation:automation.trigger.approval.overdue", "kleenestar.automation:automation.trigger.approval.overdue.parameter")
        {
        }
    }

    /// <summary>
    /// The scheduler runs the rule every so many minutes over the objects of its scope; the parameter is the interval.
    /// </summary>
    public sealed class ScheduleTrigger : AutomationTrigger
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ScheduleTrigger()
            : base(AutomationTriggerManager.Schedule, "kleenestar.automation:automation.trigger.schedule", "kleenestar.automation:automation.trigger.schedule.parameter", scheduled: true)
        {
        }
    }
}
