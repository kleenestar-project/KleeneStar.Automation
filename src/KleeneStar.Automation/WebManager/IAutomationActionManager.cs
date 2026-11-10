using KleeneStar.Automation.WebAutomation;
using System.Collections.Generic;
using WebExpress.WebCore.WebComponent;

namespace KleeneStar.Automation.WebManager
{
    /// <summary>
    /// The actions an automation rule can perform; see <see cref="AutomationActionManager"/>.
    /// </summary>
    public interface IAutomationActionManager : IComponentManager
    {
        /// <summary>
        /// Gets the registered actions, ordered by key.
        /// </summary>
        IReadOnlyList<IAutomationAction> All { get; }

        /// <summary>
        /// Registers an action that is no type of a plugin; one registered under an existing key replaces it.
        /// </summary>
        /// <param name="action">The action.</param>
        void Register(IAutomationAction action);

        /// <summary>
        /// Returns an action.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The action, or <see langword="null"/> for an unknown key.</returns>
        IAutomationAction Get(string key);
    }
}
