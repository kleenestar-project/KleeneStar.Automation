using KleeneStar.Core.WebFragment.Workspace;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Automation.WebFragment
{
    /// <summary>
    /// The entry of the workspace's administration menu (the cog of the workspace sidebar) that
    /// opens the workspace's automation: its rules and jobs.
    /// </summary>
    /// <remarks>
    /// It stands where the core's entry stood before the platform became a plugin - after the
    /// object templates, before the integrations (<c>[Order(60)]</c>).
    /// </remarks>
    [Section<SectionWorkspaceSettingPrimary>]
    [Scope<IScopeGeneral>]
    [Order(60)]
    public sealed class AutomationWorkspaceSettingFragment : WorkspaceSettingItemFragment
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context of the fragment.</param>
        public AutomationWorkspaceSettingFragment(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Text = _ => "kleenestar.automation:automation.manage.label";
            Icon = _ => new IconBolt();
        }

        /// <inheritdoc/>
        protected override IUri Destination(IRenderControlContext renderContext)
        {
            return UriOf<global::KleeneStar.Automation.WWW.Automation._workspacekey_.Index>(renderContext);
        }
    }
}
