using KleeneStar.Automation.WebAutomation;
using System.Collections.Generic;
using WebExpress.WebCore.WebComponent;

namespace KleeneStar.Automation.WebManager
{
    /// <summary>
    /// The triggers an automation rule can react to; see <see cref="AutomationTriggerManager"/>.
    /// </summary>
    public interface IAutomationTriggerManager : IComponentManager
    {
        /// <summary>
        /// Gets the registered triggers, ordered by key.
        /// </summary>
        IReadOnlyList<IAutomationTrigger> Triggers { get; }

        /// <summary>
        /// Registers a trigger that is no type of a plugin; one registered under an existing key replaces it.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        void Register(IAutomationTrigger trigger);

        /// <summary>
        /// Returns a trigger.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The trigger, or <see langword="null"/> for an unknown key.</returns>
        IAutomationTrigger Get(string key);
    }
}
