![KleeneStar](https://raw.githubusercontent.com/kleenestar-project/.github/main/docs/assets/img/banner.png)

# KleeneStar.Automation - Rules That Do the Routine

**KleeneStar.Automation** is the automation platform of **KleeneStar**. Rules react to what happens in a workspace, to conditions and to points in time, and perform a script of actions on the objects - reliably, through a persistent queue that retries and resumes.

Built on the **WebExpress-Framework** and **KleeneStar.Core**, it is a plugin of its own: the core knows no automation, it announces what happens in its own events, and this plugin listens.

## What It Powers

**KleeneStar.Automation** turns the routine of a workspace into rules an administrator writes once.

- **Triggers** - an object created or changed, a workflow transition, a comment, an approval requested, completed or overdue, a service level target at risk or missed, a schedule. A plugin adds its own: **KleeneStar.Integration** the events other systems report through webhooks.
- **Conditions** - a WQL expression narrows a rule to the objects it is meant for.
- **Actions** - `assign`, `transition`, `set`, `comment`, `notify`, `subtask`, `escalate`, `postfunction`, `approval`, `archive`; a plugin adds its own, such as `webhook <name>`.
- **A reliable queue** - every match is a job: deduplicated per occurrence, leased per worker and renewed while it runs, retried with a growing delay, parked after the last attempt and resumable after the actions already done. A job runs the script it was queued with, so an edited rule never shifts a retry.
- **Safe by design** - jobs act as the rule's identity and are audited as automation; a rule does not trigger itself, and chains stop after three steps.
- **Administration per workspace** - the page *Automation* in the workspace's settings, with its rules and the jobs that ran, for the workspace's administrators.

## How It Works

1. When the host starts, the plugin connects its manager to the core's events - commits, transitions, comments, approvals - and starts the worker that runs the queue.
2. An event that a rule's trigger, scope and condition accept queues a job; the same occurrence delivered again queues nothing.
3. The worker claims each due job right before it runs it, executes its actions one by one and records the progress after each, so a job interrupted anywhere continues where it stopped.

The triggers and actions are open catalogs: every **public sealed** trigger or action of any plugin is found by the plugin's managers, no registration needed. **KleeneStar.Templates** sets up the rules its workspace templates bring through this plugin.

## For Developers

The repository contains the plugin and its tests:

| Path                                       | Content
|--------------------------------------------|--------------------------------------------------------------
| `src/KleeneStar.Automation/KleeneStarPlugin.cs` | Plugin entry; connects the manager, starts the worker
| `src/KleeneStar.Automation/AutomationHub.cs`    | The plugin's managers, for this plugin and the ones extending it
| `src/KleeneStar.Automation/WebManager`     | `AutomationManager` (rules, queue), the trigger and action catalogs
| `src/KleeneStar.Automation/WebAutomation`  | Script, worker, the standard triggers and actions
| `src/KleeneStar.Automation/WWW`            | The page `/automation/{workspacekey}` and the REST endpoints `/api/1/automation/{workspacekey}`
| `src/KleeneStar.Automation.Test`           | Tests against an in-memory database
| `docs/kleenestar.automation.md`            | Concept and behaviour in detail

The plugin references **KleeneStar.Core** by relative path, so the repositories are cloned side by side; the host references this plugin. A plugin's routes sit under its id: `/kleenestar/kleenestar.automation/…`.

**Building**

```
dotnet build KleeneStar.Automation/src/KleeneStar.Automation/KleeneStar.Automation.csproj
```

**Testing**

```
dotnet build KleeneStar.Automation/src/KleeneStar.Automation.Test
KleeneStar.Automation/src/KleeneStar.Automation.Test/bin/Debug/net10.0/KleeneStar.Automation.Test.exe
```

The load test (`UnitTestAutomationLoad`) is opt-in: `KLEENESTAR_LOAD=1`.

## Legal & Licensing

**KleeneStar.Automation** is released under the `MIT License`, a permissive open-source license that allows reuse, modification, and distribution with minimal restrictions. You're free to use **KleeneStar** in personal, academic, or commercial projects, just include the original copyright notice.

The system is designed to be GDPR-compliant:
- No tracking
- No monetization
- No hidden dependencies
- Full transparency and infrastructure control

**KleeneStar** respects your data and your autonomy. It's built for clarity, not surveillance.

## Contributing

We welcome contributions in many areas:
- Triggers and actions (C#)
- Automation patterns for workspace templates
- Documentation and onboarding flows

Feel free to fork the repository, open issues, or submit pull requests. For larger contributions, please reach out via kleenestar.project@gmail.com.

# AI transparency notice

Parts of this software, its documentation, and its assets were created with the assistance of AI-based tools, including large language models. AI-assisted contributions are reviewed by the project maintainer before they are included.

---

Become part of the **KleeneStar** community and contribute to a modular, open-source future.
