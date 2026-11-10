using KleeneStar.Automation.WebAutomation;
using KleeneStar.Core.WebAutomation;
using KleeneStar.Core.WebManager;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using WebExpress.WebCore;
using WebExpress.WebCore.WebComponent;

namespace KleeneStar.Automation.WebManager
{
    /// <summary>
    /// The triggers an automation rule can react to, collected from the plugins
    /// (<see cref="PluginItemManager{TItem}"/>): the platform's own
    /// (<c>WebAutomation/Triggers</c>) and every <c>public sealed</c> <see cref="IAutomationTrigger"/>
    /// a plugin ships - an integration its incoming events. There is no enum of triggers.
    /// Formerly the static <c>AutomationTriggerManager</c>.
    /// </summary>
    /// <remarks>
    /// The core raises some of them itself (approvals, incoming webhooks) and names them by the
    /// keys of <see cref="AutomationTriggers"/>; this manager labels them and offers them to a rule.
    /// </remarks>
    public sealed class AutomationTriggerManager : PluginItemManager<IAutomationTrigger>, IAutomationTriggerManager
    {
        /// <summary>An object was created.</summary>
        public const string ObjectCreated = "object.created";

        /// <summary>An object was changed; the parameter narrows to the attributes that changed.</summary>
        public const string ObjectUpdated = "object.updated";

        /// <summary>A workflow transition was executed; the parameter narrows to the target state.</summary>
        public const string TransitionExecuted = "transition.executed";

        /// <summary>A comment was added; the parameter narrows to its audience.</summary>
        public const string CommentAdded = "comment.added";

        /// <summary>A service level target was missed; the parameter narrows to the target.</summary>
        public const string SlaBreached = "sla.breached";

        /// <summary>The scheduler runs the rule every so many minutes over the objects of its scope.</summary>
        public const string Schedule = "schedule";

        /// <summary>
        /// The key of the trigger another system raises through an incoming webhook (raised by the core).
        /// </summary>
        public const string IntegrationReceived = AutomationTriggers.IntegrationReceived;

        /// <summary>The key of the trigger the scheduler raises when a service level target passes its warning threshold.</summary>
        public const string SlaAtRisk = "sla.atrisk";

        /// <summary>The key of the trigger raised when an approval is requested (raised by the core).</summary>
        public const string ApprovalRequested = AutomationTriggers.ApprovalRequested;

        /// <summary>The key of the trigger raised when an approval ends (raised by the core).</summary>
        public const string ApprovalCompleted = AutomationTriggers.ApprovalCompleted;

        /// <summary>The key of the trigger raised when a step of an approval took longer than it may (raised by the core).</summary>
        public const string ApprovalOverdue = AutomationTriggers.ApprovalOverdue;

        /// <summary>
        /// Initializes a new instance of the manager. Invoked by WebExpress via reflection.
        /// </summary>
        /// <param name="componentHub">The component hub.</param>
        /// <param name="httpServerContext">The HTTP server context.</param>
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used via reflection.")]
        private AutomationTriggerManager(IComponentHub componentHub, IHttpServerContext httpServerContext)
            : base(componentHub, httpServerContext)
        {
        }

        /// <inheritdoc/>
        protected override string KeyOf(IAutomationTrigger item)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(item.Key);

            return item.Key.Trim();
        }

        /// <inheritdoc/>
        public IReadOnlyList<IAutomationTrigger> Triggers => [.. Snapshot().OrderBy(x => x.Key, StringComparer.Ordinal)];

        /// <inheritdoc/>
        public IAutomationTrigger Get(string key)
        {
            return Find(key);
        }

        /// <summary>
        /// Determines whether an event satisfies a rule's trigger parameter: a rule naming no
        /// parameter reacts to every occurrence, one naming a parameter to the occurrences about
        /// it (a comma-separated list names several).
        /// </summary>
        /// <param name="parameter">The rule's trigger parameter.</param>
        /// <param name="subjects">What the event is about.</param>
        /// <returns><see langword="true"/> when the rule reacts.</returns>
        public static bool Matches(string parameter, IReadOnlyCollection<string> subjects)
        {
            if (string.IsNullOrWhiteSpace(parameter))
            {
                return true;
            }

            var wanted = parameter.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            return (subjects ?? []).Any(x => wanted.Contains(x, StringComparer.OrdinalIgnoreCase));
        }
    }
}
