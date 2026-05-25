# Security policy

## Supported versions

Only the latest released version of each `Flags.Icons.*` NuGet package receives security fixes. Older versions are not patched. See [CHANGELOG.md](CHANGELOG.md) for the current release history.

## Reporting a vulnerability

**Please do not open a public GitHub issue for security problems.** Instead, use one of these private channels:

1. **GitHub Private vulnerability reporting** (preferred): [open a new advisory](https://github.com/Alpaq92/Flags.Icons/security/advisories/new). Only repo maintainers see it until you publish.
2. Or contact the maintainer directly via the email on the [`@Alpaq92` GitHub profile](https://github.com/Alpaq92).

Helpful details to include:

- A clear description of the issue and its impact
- Steps to reproduce or a minimal proof-of-concept
- Affected package(s) and version(s)
- Whether you've shared the finding anywhere else

## Response expectations

This is a solo-maintained OSS project; there is no formal SLA. Realistic timelines:

- **Acknowledgement**: within ~1 week of receipt
- **Triage and first fix attempt**: within ~30 days for confirmed issues
- **Public disclosure**: coordinated with the reporter once a fix is released to nuget.org

You will be credited in the published GitHub Security Advisory and `CHANGELOG.md` unless you ask to remain anonymous.

## Scope

`Flags.Icons.*` embeds PNG/SVG flag assets and provides thin platform-wrapper controls. Most realistic security concerns:

- Malicious SVG that triggers a parser issue in the underlying renderer (`Svg.Skia`, `SharpVectors`, or framework-native `SvgImageSource`). Upstream-reported issues belong to those projects; we'll bump as fixes ship.
- Build-time supply chain (Dependabot + release-please pipeline) — see [README.md](README.md) for the flow.

Functional bugs (NREs on bad input, resource leaks, broken bindings, etc.) are not security issues — please file those as regular GitHub issues.
