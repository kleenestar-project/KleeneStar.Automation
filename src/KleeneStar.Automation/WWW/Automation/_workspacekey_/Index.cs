using KleeneStar.Automation.WebAutomation;
using KleeneStar.Automation.WebManager;
using KleeneStar.Core.WebAttribute;
using KleeneStar.Core.WebParameter;
using KleeneStar.Core;
using KleeneStar.Model.Entities;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System;
using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebPage;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Automation.WWW.Automation._workspacekey_
{
    /// <summary>
    /// The automation of a workspace: its rules, and the jobs they ran - the failed ones first,
    /// each with the way to run it again.
    /// </summary>
    /// <remarks>
    /// The page is workspace administration (<see cref="AutomationAuthorization"/>, demanded in <c>Process</c>):
    /// rules act on every object of the workspace on its administrators' authority.
    /// </remarks>
    [WebIcon<IconBolt>]
    [Title("kleenestar.automation:automation.manage.title")]
    [Description("kleenestar.automation:automation.manage.description")]
    [WorkspaceKeySegment]
    [Scope<IScopeGeneral>]
    public sealed class Index : IPage<VisualTreeWebApp>, IScopeGeneral
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Index()
        {
        }

        /// <summary>
        /// Processing of the resource.
        /// </summary>
        /// <param name="renderContext">The context for rendering the page.</param>
        /// <param name="visualTree">The visual tree of the web application.</param>
        public void Process(IRenderContext renderContext, VisualTreeWebApp visualTree)
        {
            string Translate(string key) => I18N.Translate(renderContext, key);

            var keyParameter = renderContext.Request.GetParameter<WorkspaceKeyParameter>();
            var workspace = CoreHub.WorkspaceManager.GetWorkspaceByKey(keyParameter?.Value);

            // the page is workspace administration; a refusal is answered by WebExpress in place
            AutomationAuthorization.Demand(workspace, renderContext.Request);


            visualTree.Content.MainPanel.Headline.Title = "kleenestar.automation:automation.manage.title";

            if (workspace is null)
            {
                return;
            }

            visualTree.Content.MainPanel.AddPrimary(new ControlText()
            {
                Text = _ => "kleenestar.automation:automation.manage.description",
                TextColor = _ => new PropertyColorText(TypeColorText.Secondary),
                Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.Three, PropertySpacing.Space.None)
            });

            var newUri = CoreHub.GetUri<Rule>()?.BindParameters(keyParameter);

            visualTree.Content.MainPanel.AddPrimary(new ControlButtonLink()
            {
                Text = _ => "kleenestar.automation:automation.rule.add",
                Icon = _ => new IconPlus(),
                Uri = _ => newUri,
                BackgroundColor = _ => new PropertyColorButton(TypeColorButton.Primary),
                Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.Three, PropertySpacing.Space.None)
            });

            var rules = AutomationHub.AutomationManager.GetRules(workspace.Id);
            var names = rules.ToDictionary(x => x.Id, x => x.Name);

            var ruleTable = new ControlTable() { Striped = _ => TypeStripedTable.Row }
                .AddColumn(Translate("kleenestar.automation:automation.rule.name"))
                .AddColumn(Translate("kleenestar.automation:automation.rule.trigger"))
                .AddColumn(Translate("kleenestar.automation:automation.rule.state"))
                .AddColumn("");

            foreach (var rule in rules)
            {
                var editUri = CoreHub.GetUri<Rule>()?.BindParameters(keyParameter)?.Add(new UriQuery("rule", rule.Id.ToString()));
                var deleteUri = CoreHub.GetUri<Delete>()?.BindParameters(keyParameter)?.Add(new UriQuery("rule", rule.Id.ToString()));
                var trigger = Translate(AutomationHub.AutomationTriggerManager.Get(rule.Trigger)?.Label ?? rule.Trigger)
                    + (string.IsNullOrWhiteSpace(rule.TriggerParameter) ? string.Empty : $" · {rule.TriggerParameter}");
                var state = Translate(rule.State == AutomationRuleState.Active ? "kleenestar.automation:automation.rule.state.active" : "kleenestar.automation:automation.rule.state.disabled");
                var name = rule.Name;
                var deleteLabel = Translate("webexpress.webapp:delete.label");

                ruleTable.AddRow
                (
                    new ControlTableCellPanel().Add(new ControlLink() { Text = _ => name, Uri = _ => editUri }),
                    new ControlTableCell() { Text = _ => trigger },
                    new ControlTableCell() { Text = _ => state },
                    new ControlTableCellPanel().Add(new ControlLink() { Text = _ => deleteLabel, Uri = _ => deleteUri })
                );
            }

            visualTree.Content.MainPanel.AddPrimary(rules.Count == 0
                ? new ControlText() { Text = _ => "kleenestar.automation:automation.rule.empty", TextColor = _ => new PropertyColorText(TypeColorText.Secondary) }
                : ruleTable);

            visualTree.Content.MainPanel.AddPrimary(new ControlText()
            {
                Text = _ => "kleenestar.automation:automation.job.heading",
                Format = _ => TypeFormatText.H5,
                Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.Four, PropertySpacing.Space.Two)
            });

            // the failed jobs first: they wait for somebody, the others only report
            var jobs = AutomationHub.AutomationManager.GetJobs(workspace.Id, null, 100)
                .OrderBy(x => x.State == AutomationJobState.Failed ? 0 : 1)
                .ThenByDescending(x => x.Updated)
                .ToList();

            var jobTable = new ControlTable() { Striped = _ => TypeStripedTable.Row }
                .AddColumn(Translate("kleenestar.automation:automation.job.rule"))
                .AddColumn(Translate("kleenestar.automation:automation.job.object"))
                .AddColumn(Translate("kleenestar.automation:automation.job.state"))
                .AddColumn(Translate("kleenestar.automation:automation.job.updated"))
                .AddColumn(Translate("kleenestar.automation:automation.job.error"))
                .AddColumn("");

            foreach (var job in jobs)
            {
                var rule = names.GetValueOrDefault(job.RuleId) ?? job.RuleId.ToString();
                var key = ObjectKeyOf(job.ObjectId);
                var state = $"{job.State} ({job.Attempts})";
                var updated = job.Updated.ToLocalTime().ToString("g", renderContext.Request.Culture ?? CultureInfo.InvariantCulture);
                var error = job.LastError ?? string.Empty;
                var retryUri = CoreHub.GetUri<Retry>()?.BindParameters(keyParameter)?.Add(new UriQuery("job", job.Id.ToString()));
                var retryLabel = Translate("kleenestar.automation:automation.job.retry");
                var actions = new ControlTableCellPanel();

                if (job.State == AutomationJobState.Failed)
                {
                    actions.Add(new ControlLink() { Text = _ => retryLabel, Uri = _ => retryUri });
                }

                jobTable.AddRow
                (
                    new ControlTableCell() { Text = _ => rule },
                    new ControlTableCell() { Text = _ => key },
                    new ControlTableCell() { Text = _ => state },
                    new ControlTableCell() { Text = _ => updated },
                    new ControlTableCell() { Text = _ => error },
                    actions
                );
            }

            visualTree.Content.MainPanel.AddPrimary(jobs.Count == 0
                ? new ControlText() { Text = _ => "kleenestar.automation:automation.job.empty", TextColor = _ => new PropertyColorText(TypeColorText.Secondary) }
                : jobTable);
        }

        /// <summary>
        /// Returns the key of a job's object as the caller may see it: an object their grants or
        /// clearance keep from them is not named here either.
        /// </summary>
        /// <param name="objectId">The object.</param>
        /// <returns>The key, or a dash for an object the caller may not see or that is gone.</returns>
        private static string ObjectKeyOf(Guid objectId)
        {
            return CoreHub.ObjectManager.GetObject(objectId)?.Key ?? "-";
        }
    }
}
