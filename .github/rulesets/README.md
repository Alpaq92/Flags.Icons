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
- Allowed merge methods: **squash** only (forced by linear-history requirement — `merge` would be blocked, `rebase` is left off to keep history simple).

**CI must pass**
- `Build core + Avalonia (linux)`
- `Build WPF / MAUI (windows)`
- `Build cross-platform demos (macos)`
- `strict_required_status_checks_policy: true` — branch must be up to date with `main` before merge.

**Code scanning**
- CodeQL (configured in [`../workflows/codeql.yml`](../workflows/codeql.yml)) must run and blocks on high-or-higher security alerts and any error-level finding.

**Bypass**
- Repo admins (role ID `5`) can bypass *only via PR*, never on direct push.

## Optional follow-ups

Two protections are intentionally left out — both would break the project until matching setup exists:

- **`required_signatures`** — re-add once contributors have `git config commit.gpgsign true` + a configured GPG/SSH signing key. Without that, every commit gets rejected.
- **Required Copilot code review** — requires Copilot Pro+ / Business / Enterprise (not available on the Free tier). To enable: add a `.github/CODEOWNERS` with `* @Copilot`, flip `require_code_owner_review` from `false` to `true` in this JSON, and re-import. Also flip **Settings → Code & automation → Code review → "Automatically request Copilot's review on new pull requests"** so the review starts immediately when the PR opens.

Either can be re-added by editing this JSON and re-importing, or by adding the corresponding rule in the GitHub UI.
