using KleeneStar.Automation.WebAutomation;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using WebExpress.WebCore;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebCore.WebPlugin;

namespace KleeneStar.Automation.WebManager
{
    /// <summary>
    /// The actions an automation rule can perform, collected from the plugins by WebExpress's
    /// <see cref="PluginTypeCatalog{TItem}"/>: the platform's own (<c>WebAutomation/Actions</c>)
    /// and every <c>public sealed</c> <see cref="IAutomationAction"/> a plugin ships - an
    /// integration an outgoing call to its system. There is no enum of actions. Formerly the
    /// static <c>AutomationActionManager</c>.
    /// </summary>
    public sealed class AutomationActionManager : IAutomationActionManager
    {
        private readonly PluginTypeCatalog<IAutomationAction> _catalog = new(x => x.Key);

        /// <summary>
        /// Initializes a new instance of the manager. Invoked by WebExpress via reflection.
        /// </summary>
        /// <param name="componentHub">The component hub.</param>
        /// <param name="httpServerContext">The HTTP server context.</param>
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used via reflection.")]
        private AutomationActionManager(IComponentHub componentHub, IHttpServerContext httpServerContext)
        {
            if (componentHub?.PluginManager is not null)
            {
                _catalog.Attach(componentHub, httpServerContext);
            }
            else
            {
                // without a plugin manager (a test fixture) the manager knows its own assembly's items
                _catalog.Collect(typeof(AutomationActionManager).Assembly);
            }
        }

        /// <inheritdoc/>
        public IReadOnlyList<IAutomationAction> All => [.. _catalog.Items.OrderBy(x => x.Key, StringComparer.Ordinal)];

        /// <inheritdoc/>
        public IAutomationAction Get(string key)
        {
            return _catalog.Find(key);
        }

        /// <summary>
        /// Gets a number that moves on every change of the items, so a cached picker can tell
        /// that an item arrived or left after it was built.
        /// </summary>
        public long Version => _catalog.Version;

        /// <inheritdoc/>
        public void Register(IAutomationAction item)
        {
            _catalog.Register(item);
        }

        /// <summary>
        /// Stops following the plugins.
        /// </summary>
        public void Dispose()
        {
            _catalog.Dispose();
        }
    }
}
