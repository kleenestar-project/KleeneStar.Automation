using KleeneStar.Automation.WebManager;
using KleeneStar.Core;
using KleeneStar.Core.WebManager;
using KleeneStar.Model;
using KleeneStar.Model.Settings;
using System;
using System.Reflection;
using WebExpress.WebCore;
using WebExpress.WebCore.WebComponent;

namespace KleeneStar.Automation.Test
{
    /// <summary>
    /// Test fixture that wires <see cref="CoreHub"/>, <see cref="AutomationHub"/> and
    /// <see cref="ModelHub"/> to an isolated in-memory database and pre-populates the private
    /// manager backing fields so that <see cref="CoreHub.ClassManager"/> and siblings - and the
    /// plugin's <see cref="AutomationHub.AutomationManager"/> - resolve without touching any real
    /// <see cref="IComponentHub"/>.
    /// </summary>
    internal static class AutomationHubFixture
    {
        /// <summary>
        /// Points <see cref="ModelHub"/> at an isolated in-memory database and seeds
        /// <see cref="CoreHub"/>'s private manager backing fields with freshly constructed
        /// instances so that the cached <c>??=</c> accessor bypasses
        /// <see cref="IComponentHub.GetComponentManager{T}"/>.
        /// </summary>
        /// <param name="connectionString">
        /// A unique per-test connection string so parallel-phase state does not leak between
        /// cases in the same <c>[Collection("NonParallelTests")]</c> collection.
        /// </param>
        public static void Initialize(string connectionString)
        {
            // the path index is a process-wide switch; a new database starts with it off, so a
            // test that rebuilt it for its own store cannot leak a 'ready' index into the next
            KleeneStar.Core.WebManager.ObjectPathIndex.Disable();

            ModelHub.DatabaseSettings = new DatabaseSettings
            {
                Assembly = "KleeneStar.Automation.Test",
                ConnectionString = connectionString
            };

            // every manager of the core, found by reflection over CoreHub's backing fields, so the
            // fixture never has to be kept in step with the core by hand
            foreach (var field in typeof(CoreHub).GetFields(BindingFlags.NonPublic | BindingFlags.Static))
            {
                var ctor = field.FieldType.IsClass && !field.FieldType.IsAbstract && typeof(IComponentManager).IsAssignableFrom(field.FieldType)
                    ? field.FieldType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, [typeof(IComponentHub), typeof(IHttpServerContext)], null)
                    : null;

                if (ctor is not null)
                {
                    field.SetValue(null, ctor.Invoke([null, null]));
                }
            }

            // the plugin's managers; without a plugin manager each knows the items of its own
            // assembly, so the platform's triggers and actions are there as in a running host
            object Plugin(Type managerType, string fieldName)
            {
                var instance = managerType.GetConstructor
                (
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    [typeof(IComponentHub), typeof(IHttpServerContext)],
                    null
                )?.Invoke([null, null])
                    ?? throw new InvalidOperationException($"Private ctor not found on {managerType.FullName}.");

                (typeof(AutomationHub).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static)
                    ?? throw new InvalidOperationException($"Backing field {fieldName} not found on AutomationHub."))
                    .SetValue(null, instance);

                return instance;
            }

            Plugin(typeof(AutomationTriggerManager), "_automationTriggerManager");
            Plugin(typeof(AutomationActionManager), "_automationActionManager");

            Plugin(typeof(AutomationManager), "_automationManager");
        }

        /// <summary>
        /// Creates a DbContext against the same in-memory database that <see cref="ModelHub.CreateDbContext"/>
        /// is routed to, for use in the <c>arrange</c> phase when seeding entities.
        /// </summary>
        /// <param name="connectionString">The in-memory database name.</param>
        /// <returns>A configured KleeneStarDbContext instance.</returns>
        public static KleeneStarDbContext CreateDbContext(string connectionString)
        {
            return InMemoryDbContextFactory.Create(connectionString);
        }
    }
}
