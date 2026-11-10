using KleeneStar.Core.WebRestApi;
using KleeneStar.Model.Entities;
using System.Collections.Generic;
using System.Linq;

namespace KleeneStar.Automation.WWW.Api._1_.Automation
{
    /// <summary>
    /// The wire shapes of the automation endpoints. The plumbing they share with the other
    /// process endpoints of a workspace - the gate, the JSON answer, the body - is the core's
    /// <see cref="KleeneStar.Core.WebRestApi.JsonApi"/>.
    /// </summary>
    /// <remarks>
    /// Rules act on every object of a workspace on its administrators' authority, so reading and
    /// writing them - and running a failed job again - is workspace administration
    /// (<see cref="KleeneStar.Core.WebRestApi.JsonApi.Authorize"/>), the same demand the
    /// automation page makes.
    /// </remarks>
    internal static class AutomationApi
    {
        /// <summary>
        /// Projects a rule onto its wire shape.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <returns>The DTO.</returns>
        internal static object ToDto(AutomationRule rule)
        {
            return new
            {
                id = rule.Id,
                name = rule.Name,
                description = rule.Description,
                state = rule.State.ToString().ToLowerInvariant(),
                classId = rule.ClassId,
                trigger = rule.Trigger,
                triggerParameter = rule.TriggerParameter,
                condition = rule.Condition,
                actions = rule.Actions,
                runAsId = rule.RunAsId,
                nextRun = rule.NextRun,
                updated = rule.Updated
            };
        }

        /// <summary>
        /// Projects jobs onto their wire shape.
        /// </summary>
        /// <param name="jobs">The jobs.</param>
        /// <returns>The DTOs.</returns>
        internal static IEnumerable<object> ToDto(IEnumerable<AutomationJob> jobs)
        {
            return jobs.Select(job => new
            {
                id = job.Id,
                ruleId = job.RuleId,
                objectId = job.ObjectId,
                trigger = job.Trigger,
                state = job.State.ToString().ToLowerInvariant(),
                attempts = job.Attempts,
                completedActions = job.CompletedActions,
                nextAttempt = job.NextAttempt,
                lastError = job.LastError,
                created = job.Created,
                updated = job.Updated
            });
        }

        /// <summary>
        /// The JSON body of a rule written through the automation page or a client.
        /// </summary>
        internal sealed class RulePayload
        {
            /// <summary>Gets or sets the id of the rule to change, or empty for a new one.</summary>
            public string Id { get; set; }

            /// <summary>Gets or sets the name.</summary>
            public string Name { get; set; }

            /// <summary>Gets or sets the description.</summary>
            public string Description { get; set; }

            /// <summary>Gets or sets the state, <c>active</c> or <c>disabled</c>.</summary>
            public string State { get; set; }

            /// <summary>Gets or sets the class the rule is narrowed to, or empty for all.</summary>
            public string ClassId { get; set; }

            /// <summary>Gets or sets the trigger key.</summary>
            public string Trigger { get; set; }

            /// <summary>Gets or sets the trigger parameter.</summary>
            public string TriggerParameter { get; set; }

            /// <summary>Gets or sets the WQL condition.</summary>
            public string Condition { get; set; }

            /// <summary>Gets or sets the action script.</summary>
            public string Actions { get; set; }

            /// <summary>Gets or sets the account the rule acts as: a user name, an e-mail address, or empty.</summary>
            public string RunAs { get; set; }
        }
    }
}
