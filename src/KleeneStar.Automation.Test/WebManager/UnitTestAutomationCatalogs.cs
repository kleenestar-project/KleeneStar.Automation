using KleeneStar.Automation.WebAutomation;
using KleeneStar.Automation.WebManager;

namespace KleeneStar.Automation.Test.WebManager
{
    /// <summary>
    /// Tests that the platform's triggers and actions are what the plugin's managers collect:
    /// every one is <c>public sealed</c> with a parameterless constructor, and the managers find
    /// all of them - a trigger or an action that breaks the rule would be missing silently.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestAutomationCatalogs
    {
        /// <summary>
        /// The triggers and the actions of the platform are collected.
        /// </summary>
        [Fact]
        public void PlatformItems_AreCollected()
        {
            AutomationHubFixture.Initialize(nameof(PlatformItems_AreCollected));

            Assert.Equal(11, AutomationHub.AutomationTriggerManager.Triggers.Count);
            Assert.True(AutomationHub.AutomationTriggerManager.Get(AutomationTriggerManager.Schedule)?.Scheduled);
            Assert.Equal(
                ["approval", "archive", "assign", "comment", "escalate", "notify", "postfunction", "set", "subtask", "transition", "webhook"],
                AutomationHub.AutomationActionManager.All.Select(x => x.Key));
        }

        /// <summary>
        /// Every trigger and action the plugin ships keeps to the discovery rule.
        /// </summary>
        /// <param name="extensionPoint">The extension point.</param>
        [Theory]
        [InlineData(typeof(IAutomationTrigger))]
        [InlineData(typeof(IAutomationAction))]
        public void ShippedItems_AreDiscoverable(Type extensionPoint)
        {
            var items = typeof(AutomationHub).Assembly.GetTypes()
                .Where(x => x.IsClass && !x.IsAbstract && extensionPoint.IsAssignableFrom(x))
                .ToList();

            Assert.NotEmpty(items);
            Assert.All(items, x => Assert.True(x.IsPublic && x.IsSealed && x.GetConstructor(Type.EmptyTypes) is not null, $"{x.FullName} is not discoverable."));
        }
    }
}
