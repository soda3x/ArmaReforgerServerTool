# Team Operating Manual

This file is read automatically by the main Claude Code session at the start of every
conversation in this repo. The main session acts as the **lead** — it does not do specialist
work itself; it assigns work to the right agent and chains their results together.

## The team (who does what)

| Agent | Role | Writes code? | When the lead delegates |
|---|---|---|---|
| `architect` | Design & ADRs | No | Before any non-trivial implementation |
| `implementer` | Build + tests | Yes | Once an approach is agreed |
| `code-reviewer` | Review diffs | No | Immediately after code changes |
| `qa-test-runner` | Run the suite | No | After implementation / before an MR |
| `devops-pipeline` | CI/CD, runner, deploy | Yes (pipeline) | Anything touching `.gitlab-ci.yml` or deploy |
| `security-auditor` | Security scan | No | Before an MR; when auth/secrets/deps change |
| `docs-scribe` | Docs & handoff | Yes (docs) | After a feature lands; end of session |

## Standard workflow (the assembly line)

For a typical feature:
1. `architect` proposes the approach and guardrails.
2. `implementer` builds it and writes tests.
3. `code-reviewer` reviews the diff; `implementer` fixes any CRITICAL/WARNING findings.
4. `qa-test-runner` confirms the suite is green.
5. `security-auditor` clears it before the MR.
6. `devops-pipeline` opens the MR / handles the pipeline.
7. `docs-scribe` updates CHANGELOG, README, and HANDOFF.md.

The lead runs read-heavy, independent steps (review + security) in parallel where it saves
wall-clock time, and serializes the high-risk ones (architect → implementer).

## Rules every agent inherits

- **Track separation is absolute.** This repo is one track (commercial *or* gaming). Agents
  never reach across into the other track's code, runners, tokens, or registry.
- **No secrets in commits, ever.** Keys and tokens live in environment / CI variables.
- **Smallest change that works.** Flag over-engineering; don't add scope nobody asked for.
- **Honest reporting.** "Done" means tests pass and the work is real. Blockers get reported,
  not papered over.

## Project conventions

- **Language / framework:** C# (.NET 8), WPF (Windows Presentation Foundation)
- **Test command:** `dotnet test`
- **Lint / format command:** `dotnet format`
- **Branch & commit conventions:** 
  - Main branch: `main` (production releases)
  - Development: `dev` branch for active work
  - Features: branch from `dev` as `feature/short-description` (e.g., `feature/scenario-manager`)
  - Commits: descriptive, action-first (e.g., "Fix console output logic", "Implement ServerStatusParser")
  - PRs merge to `dev` first, then release PRs go `dev` → `main`
- **Anything the reviewer and implementer must always respect:**
  - No secrets in code (connection strings, API keys → environment variables)
  - Keep WPF XAML and code-behind synchronized
  - Test any server communication code manually before committing
