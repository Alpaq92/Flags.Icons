# GitHub Rulesets

## How to import

1. Push the repo to GitHub.
2. Go to **Settings → Rules → Rulesets → New ruleset → Import a ruleset**.
3. Upload [`main-branch-protection.json`](main-branch-protection.json).
4. Click **Create**.

## What `main-branch-protection.json` enforces on `main`

**Branch integrity**
- Block branch deletion.
- Block force pushes (`non_fast_forward`).
- Require linear history (squash-only merges, no merge commits).

**Pull request**
- 1 approving review required.
- Stale reviews dismissed on new pushes.
- All review threads must be resolved.
- Last push must be approved (no sneaking changes in after approval).
- Allowed merge methods: **squash** only.

**CI must pass**
- `Build core + Avalonia (linux)`
- `Build WPF / MAUI (windows)`
- `Build cross-platform demos (macos)`
- `strict_required_status_checks_policy: true` — branch must be up to date with `main` before merge.

**Code scanning**
- CodeQL (configured in [`../workflows/codeql.yml`](../workflows/codeql.yml)) must run and blocks on high-or-higher security alerts and any error-level finding.

## Bypass actors — how the bots fit in

This repo runs three automated paths that would otherwise be blocked by the PR / approval rules. Each one is enabled by a targeted bypass entry rather than by weakening the rule itself.

| Actor | `actor_id` | `bypass_mode` | Why it's needed |
|---|---|---|---|
| Repository admin (you) | `5` (`RepositoryRole`) | `pull_request` | Merge your own PRs on a solo repo without a second-account approval. **Direct push is still blocked** — even as admin you must go through a PR. |
| `github-actions[bot]` | `15368` (`Integration`) | `always` | (1) `flagkit-refresh.yml` pushes the monthly submodule bump directly to `main`. (2) `dependabot-auto-merge.yml` calls `gh pr merge --auto --squash`; GitHub records the merge as `github-actions[bot]`, and auto-merge fires for that actor as soon as required status checks pass even though no human approval exists. |
| `dependabot[bot]` | `29110` (`Integration`) | `pull_request` | Belt-and-suspenders for cases where Dependabot itself performs the merge (e.g. if you switch to `@dependabot squash and merge` comments). Has no effect on the current `gh pr merge --auto` path since github-actions is the merge actor there. |

### Trust model this assumes

- **Only the repo owner adds workflows**, so granting `github-actions[bot]` `always` is the same trust level as the owner. On a multi-contributor repo, tighten this — `pull_request` mode would still allow auto-merge of Dependabot PRs but block direct pushes (you'd then need `flagkit-refresh.yml` to open a PR instead of pushing).
- **Dependabot auto-merge is scoped to NuGet packages** ([`dependabot-auto-merge.yml`](../workflows/dependabot-auto-merge.yml) gates on `package-ecosystem == 'nuget'`). GitHub Actions bumps use `chore(deps):` and require manual review.
- **CI + CodeQL still gate every Dependabot PR.** Bypass lets the merge fire without an approving review; it does NOT skip status checks (auto-merge waits for them).
- **`require_last_push_approval` is harmless for bots** because their PRs never need an approval in the first place under this bypass model.

## Companion repo settings

Two protections live outside this ruleset:

- **Auto-request Copilot on new PRs** — Settings → Code & automation → Code review → *"Automatically request Copilot's review on new pull requests"*. Has no effect on the Free Copilot tier (Copilot code review requires Pro+ / Business / Enterprise).
- **Allow GitHub Actions to create and approve pull requests** — Settings → Actions → General → Workflow permissions. Required for `release-please` to open the Release PR unless you set up `RELEASE_PLEASE_PAT` (recommended — see the top-level [README](../../README.md)).

## Optional follow-ups

Two protections are intentionally left out — both would break the project until matching setup exists:

- **`required_signatures`** — re-add once contributors have `git config commit.gpgsign true` + a configured GPG/SSH signing key. Without that, every commit gets rejected (including bot commits, which would also need verified signing wired up).
- **Required Copilot code review** — requires Copilot Pro+ / Business / Enterprise. To enable: add a `.github/CODEOWNERS` with `* @Copilot`, flip `require_code_owner_review` from `false` to `true` in this JSON, and re-import.
