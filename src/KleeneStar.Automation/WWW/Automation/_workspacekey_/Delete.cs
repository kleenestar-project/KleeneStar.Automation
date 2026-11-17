using KleeneStar.Core.WebControl;
using KleeneStar.Core.WebParameter;
using KleeneStar.Automation.WebAutomation;
using KleeneStar.Core;
using System;
using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebPage;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Automation.WWW.Automation._workspacekey_
{
    /// <summary>
    /// Confirms the removal of an automation rule (<c>?rule=&lt;id&gt;</c>) and its jobs.
    /// </summary>
    [WebIcon<IconTrash>]
    [Title("kleenestar.automation:automation.rule.delete.title")]
    [Scope<IScopeGeneral>]
    public sealed class Delete : IPage<VisualTreeWebApp>, IScopeGeneral
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Delete()
        {
        }

        /// <summary>
        /// Processing of the resource.
        /// </summary>
        /// <param name="renderContext">The context for rendering the page.</param>
        /// <param name="visualTree">The visual tree of the web application.</param>
        public void Process(IRenderContext renderContext, VisualTreeWebApp visualTree)
        {
            var keyParameter = renderContext.Request.GetParameter<WorkspaceKeyParameter>();
            var workspace = CoreHub.WorkspaceManager.GetWorkspaceByKey(keyParameter?.Value);

            // the page is workspace administration; a refusal is answered by WebExpress in place
            AutomationAuthorization.Demand(workspace, renderContext.Request);

            var rule = Guid.TryParse(renderContext.Request.GetParameter("rule")?.Value, out var ruleId)
                ? AutomationHub.AutomationManager.GetRule(ruleId)
                : null;

            visualTree.Content.MainPanel.Headline.Title = "kleenestar.automation:automation.rule.delete.title";

            if (workspace is null || rule is null || rule.WorkspaceId != workspace.Id)
            {
                return;
            }

            var name = rule.Name;

            visualTree.Content.MainPanel.AddPrimary(new ControlText()
            {
                Text = _ => name,
                Format = _ => TypeFormatText.H5
            });
            visualTree.Content.MainPanel.AddPrimary(new ControlText()
            {
                Text = _ => "kleenestar.automation:automation.rule.delete.description",
                TextColor = _ => new PropertyColorText(TypeColorText.Secondary)
            });

            var form = new ControlModalForm("automation-rule-delete-form")
            {
                Uri = _ => CoreHub.GetUri<global::KleeneStar.Automation.WWW.Api._1_.Automation._workspacekey_.Delete>()?.BindParameters(keyParameter),
                RedirectUri = _ => CoreHub.GetUri<Index>()?.BindParameters(keyParameter)
            };

            form.Add(new PresetHiddenInput("automation-rule-delete-id") { Name = _ => "id", Value = _ => rule.Id.ToString() });
            form.AddPrimaryButton(new ControlFormItemButtonSubmit() { Text = _ => "webexpress.webapp:delete.label" });
            visualTree.Content.MainPanel.AddPrimary(form);
        }
    }
}
