using KleeneStar.Automation.WebAutomation;
using KleeneStar.Core;
using System;
using WebExpress.WebCore;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebCore.WebPlugin;

namespace KleeneStar.Automation
{
    /// <summary>
    /// Plugin entry point of the automation platform: rules that react to events, conditions and
    /// points in time, and the persistent queue that runs them.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The core knows no automation: it announces what happens in its own events (commits,
    /// transitions, comments, <c>IApprovalManager.ApprovalOccurred</c>), and this plugin listens.
    /// When it runs, it subscribes its manager to the events the triggers are raised from,
    /// audits its rules and failed jobs, starts the
    /// worker that runs the queue. Its pages refuse a caller themselves
    /// (<see cref="AutomationAuthorization"/>, a WebExpress <c>ForbiddenException</c> from
    /// <c>Process</c>), and its entry of the
    /// workspace's administration menu is a fragment of its own
    /// (<see cref="WebFragment.AutomationWorkspaceSettingFragment"/>).
    /// </para>
    /// <para>
    /// The rule and job tables stay in the model (<c>AutomationRule</c>, <c>AutomationJob</c>),
    /// because the installation has one database and one migration history; without the plugin
    /// they are simply not read.
    /// </para>
    /// </remarks>
    [Name("kleenestar.automation:plugin.name")]
    [Description("kleenestar.automation:plugin.description")]
    [Icon("/assets/img/kleenestar.automation.svg")]
    [Application<KleeneStarApplication>()]
    [Dependency("webexpress.webapp")]
    [Dependency("kleenestar.core")]
    public sealed class KleeneStarPlugin : IPlugin
    {
        private readonly IHttpServerContext _httpServerContext;
        private AutomationWorker _worker;

        /// <summary>
        /// Initializes a new instance of the class. Invoked by WebExpress via reflection.
        /// </summary>
        /// <param name="httpServerContext">The server context, for the log.</param>
        /// <param name="componentHub">The component hub the plugin's manager is registered with.</param>
        public KleeneStarPlugin(IHttpServerContext httpServerContext, IComponentHub componentHub)
        {
            _httpServerContext = httpServerContext;
            AutomationHub.ComponentHub = componentHub;
        }

        /// <summary>
        /// Connects the automation platform to the core and starts its queue.
        /// </summary>
        /// <remarks>
        /// The queue is in the database, so the jobs an earlier run left - pending, or held by a
        /// worker that died with it - are picked up as soon as the worker starts.
        /// </remarks>
        public void Run()
        {
            try
            {
                var manager = AutomationHub.AutomationManager
                    ?? throw new InvalidOperationException("The automation manager is not registered.");

                manager.Connect();

                _worker = new AutomationWorker();
            }
            catch (Exception ex)
            {
                _httpServerContext?.Log?.Exception(ex);
            }
        }

        /// <summary>
        /// Stops the queue. A job the worker was
        /// running finishes; one it had not started stays queued.
        /// </summary>
        public void Dispose()
        {
            _worker?.Dispose();
            _worker = null;
        }
    }
}
