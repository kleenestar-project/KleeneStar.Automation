using KleeneStar.Automation.WebManager;
using KleeneStar.Core;
using WebExpress.WebCore.WebComponent;

namespace KleeneStar.Automation
{
    /// <summary>
    /// The static facade of the automation plugin, beside the core's <see cref="CoreHub"/>.
    /// </summary>
    /// <remarks>
    /// WebExpress resolves a component manager by its exact class, and the core cannot name a
    /// class of this plugin, so the plugin's manager is reached here - by this plugin and by the
    /// plugins that extend it (<c>KleeneStar.Integration</c>, <c>KleeneStar.Templates</c>). The core
    /// knows no automation.
    /// </remarks>
    public static class AutomationHub
    {
        private static AutomationManager _automationManager;
        private static AutomationTriggerManager _automationTriggerManager;
        private static AutomationActionManager _automationActionManager;

        /// <summary>
        /// Gets or sets the component hub the plugin was handed; the core's is used while none is set.
        /// </summary>
        public static IComponentHub ComponentHub { get; internal set; }

        /// <summary>
        /// Gets the automation platform: its rules and its persistent job queue.
        /// </summary>
        public static IAutomationManager AutomationManager => _automationManager ??= (ComponentHub ?? CoreHub.ComponentHub)?.GetComponentManager<AutomationManager>();

        /// <summary>
        /// Gets the triggers a rule can react to, collected from the plugins.
        /// </summary>
        public static IAutomationTriggerManager AutomationTriggerManager => _automationTriggerManager ??= (ComponentHub ?? CoreHub.ComponentHub)?.GetComponentManager<AutomationTriggerManager>();

        /// <summary>
        /// Gets the actions a rule can perform, collected from the plugins.
        /// </summary>
        public static IAutomationActionManager AutomationActionManager => _automationActionManager ??= (ComponentHub ?? CoreHub.ComponentHub)?.GetComponentManager<AutomationActionManager>();
    }
}
