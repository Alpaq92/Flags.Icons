# GitHub Rulesets

## How to import

1. Push the repo to GitHub.
2. Go to **Settings → Rules → Rulesets → New ruleset → Import a ruleset**.
3. Upload [`main-branch-protection.json`](main-branch-protection.json).
4. Click **Create**.

No manual UI follow-up required — the JSON is fully self-contained.

## What `main-branch-protection.json` enforces on `main`

**Branch integrity**
- Block branch deletion.
- Block force pushes (`non_fast_forward`).
- Require linear history (squash-only merges, no merge commits).

**Pull request**
- **0 approving reviews required** (see *Why no human approval* below).
- Stale reviews dismissed on new pushes.
- All review threads must be resolved.
- Last push must be approved before merge (no sneaking changes in after approval).
- Allowed merge methods: **squash** only.

**CI must pass**
- `Build core + Avalonia (linux)`
- `Build WPF / MAUI (windows)`
- `Build cross-platform demos (macos)`
- `strict_required_status_checks_policy: true` — branch must be up to date with `main` before merge.

**Code scanning**
- CodeQL (configured in [`../workflows/codeql.yml`](../workflows/codeql.yml)) must run and blocks on high-or-higher security alerts and any error-level finding.

## Bypass actors

| Actor | Bypass mode | Why |
|---|---|---|
| Repository admin (you) | `always` | (a) Lets `flagkit-refresh.yml` push the monthly submodule bump directly to `main` (the workflow uses `RELEASE_PLEASE_PAT`, which acts as you). (b) Lets `dependabot-auto-merge.yml`'s `gh pr merge --auto` fire — when called with the PAT, GitHub records the merge as you, and the admin bypass lets it through without a separate bot bypass entry. |

That's the only bypass. No bot Integration entries.

## Why no human approval / why no bot bypass

This config is shaped by two constraints of free/personal GitHub accounts:

1. **No second reviewer to satisfy a 1-approval requirement.** On a solo repo, requiring 1 approval means every PR needs either (a) a second account to approve or (b) a manual admin bypass click. Setting it to **0** lets auto-merge fire as soon as the *real* gates (CI, CodeQL, threads, last-push-approval) are green. The protection that matters — *no broken code on `main`* — is preserved.

2. **`github-actions[bot]` and `dependabot[bot]` cannot be added as bypass actors on this tier.** Neither the JSON importer accepts them (`"invalid actor"`) nor does the UI's bypass picker show them. Bot bypass via the `Integration` actor_type is effectively a GitHub Teams/Enterprise feature. Workaround: have the workflows act as *you* (via `RELEASE_PLEASE_PAT`) so the existing admin bypass covers them.

If you ever upgrade to a Teams/Enterprise plan, you can tighten this by:
- Re-adding a 1-approval requirement
- Adding `github-actions[bot]` (mode `always`) and `dependabot[bot]` (mode `pull_request`) via the UI picker
- Switching admin bypass back to `pull_request` mode

## Companion repo settings

Two protections live outside this ruleset:

- **Allow auto-merge** — Settings → General → Pull Requests → *"Allow auto-merge"*. Required for `release-please`'s `gh pr merge --auto` to actually queue the merge.
- **Auto-request Copilot on new PRs** — Settings → Code & automation → Code review → *"Automatically request Copilot's review on new pull requests"*. No effect on the Free Copilot tier.

## Optional follow-ups

Two protections are intentionally left out — both would break the project until matching setup exists:

- **`required_signatures`** — re-add once contributors have `git config commit.gpgsign true` + a configured GPG/SSH signing key. Without that, every commit gets rejected.
- **Required Copilot code review** — requires Copilot Pro+ / Business / Enterprise. To enable: add a `.github/CODEOWNERS` with `* @Copilot`, flip `require_code_owner_review` from `false` to `true` in this JSON, and re-import.
