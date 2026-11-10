# KleeneStar.Automation
KleeneStar.Automation is a powerful automation plugin for the KleeneStar platform that enables organizations to streamline processes, reduce manual effort, and orchestrate workflows across applications, services, and business systems.

Rules react to events (an object created or changed, a transition, a comment, an approval, an incoming webhook), to conditions (WQL) and to points in time (schedules, service level targets at risk or missed), and perform a script of actions (`assign`, `transition`, `set`, `comment`, `notify`, `subtask`, `escalate`, `postfunction`, `approval`, `webhook`, `archive`). Every match is a job in a persistent, deduplicated, leased queue that is retried and resumable. See [`docs/kleenestar.automation.md`](docs/kleenestar.automation.md).

## Layout

- `src/KleeneStar.Automation/` - the plugin: `KleeneStarPlugin` (entry point), `AutomationHub`, `WebManager/AutomationManager`, `WebAutomation/` (trigger and action catalogs, script, worker, actions), the page `WWW/Automation/{workspacekey}` and the REST endpoints `WWW/Api/_1_/Automation/{workspacekey}`.
- `src/KleeneStar.Automation.Test/` - xUnit v3 tests (own fixture `AutomationHubFixture`, a copy of the core's).

The plugin references `KleeneStar.Core` by relative path, so the repositories are cloned side by side; the host (`KleeneStar`) references this plugin.

## Build and test

```
dotnet build src/KleeneStar.Automation/KleeneStar.Automation.csproj
dotnet build src/KleeneStar.Automation.Test/KleeneStar.Automation.Test.csproj
src/KleeneStar.Automation.Test/bin/Debug/net10.0/KleeneStar.Automation.Test.exe
```

The load test (`UnitTestAutomationLoad`) is opt-in: `KLEENESTAR_LOAD=1`.
