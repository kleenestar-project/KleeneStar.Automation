using KleeneStar.Automation.WebAutomation;
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
    /// The actions an automation rule can perform, collected from the plugins
    /// (<see cref="PluginItemManager{TItem}"/>): the platform's own (<c>WebAutomation/Actions</c>)
    /// and every <c>public sealed</c> <see cref="IAutomationAction"/> a plugin ships - an
    /// integration an outgoing call to its system. There is no enum of actions. Formerly the
    /// static <c>AutomationActionManager</c>.
    /// </summary>
    public sealed class AutomationActionManager : PluginItemManager<IAutomationAction>, IAutomationActionManager
    {
        /// <summary>
        /// Initializes a new instance of the manager. Invoked by WebExpress via reflection.
        /// </summary>
        /// <param name="componentHub">The component hub.</param>
        /// <param name="httpServerContext">The HTTP server context.</param>
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used via reflection.")]
        private AutomationActionManager(IComponentHub componentHub, IHttpServerContext httpServerContext)
            : base(componentHub, httpServerContext)
        {
        }

        /// <inheritdoc/>
        protected override string KeyOf(IAutomationAction item)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(item.Key);

            return item.Key.Trim();
        }

        /// <inheritdoc/>
        public IReadOnlyList<IAutomationAction> All => [.. Snapshot().OrderBy(x => x.Key, StringComparer.Ordinal)];

        /// <inheritdoc/>
        public IAutomationAction Get(string key)
        {
            return Find(key);
        }
    }
}
