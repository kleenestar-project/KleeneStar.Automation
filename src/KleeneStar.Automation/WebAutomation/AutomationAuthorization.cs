using KleeneStar.Core.WebPermission;
using KleeneStar.Core.WebRestApi;
using KleeneStar.Model.Entities;
using WebExpress.WebCore.WebMessage;

namespace KleeneStar.Automation.WebAutomation
{
    /// <summary>
    /// Who may see and change the automation of a workspace: its administrators. The one rule the
    /// automation pages and endpoints ask, so the page that offers a rule and the endpoint that
    /// stores it agree.
    /// </summary>
    /// <remarks>
    /// Rules act on every object of a workspace on its administrators' authority, so this is
    /// workspace administration - <see cref="ContentAuthorization.MayAdministerWorkspace"/>, the
    /// question <see cref="JsonApi.Authorize"/> asks the endpoints. A page refuses through
    /// WebExpress itself: <see cref="Demand"/> throws its <see cref="ForbiddenException"/> from
    /// <c>Process</c>, which the server answers in place - the access-denied page for a signed-in
    /// caller, the sign-in prompt for an anonymous one - before anything of the page is rendered.
    /// </remarks>
    public static class AutomationAuthorization
    {
        /// <summary>
        /// Determines whether the caller of a request may administer a workspace's automation.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        /// <param name="request">The request.</param>
        /// <returns><see langword="true"/> when the caller may.</returns>
        public static bool MayAdminister(Workspace workspace, IRequest request)
        {
            return workspace is not null && ContentAuthorization.MayAdministerWorkspace(workspace.Id, request);
        }

        /// <summary>
        /// Refuses the caller of a page who may not administer the workspace's automation. A
        /// workspace that does not exist is no permission question; the page shows nothing for it.
        /// </summary>
        /// <param name="workspace">The workspace the route names, or <see langword="null"/>.</param>
        /// <param name="request">The request.</param>
        /// <exception cref="ForbiddenException">The caller is refused.</exception>
        public static void Demand(Workspace workspace, IRequest request)
        {
            if (workspace is not null && !MayAdminister(workspace, request))
            {
                throw PageAuthorization.Refuse();
            }
        }
    }
}
