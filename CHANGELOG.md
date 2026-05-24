# Changelog

## [1.1.0](https://github.com/Alpaq92/Flags.Icons/compare/v1.0.1...v1.1.0) (2026-05-24)

> **Note:** this release is infrastructure-only — no consumer-facing changes to any `Flags.Icons.*` package. The single entry below was a CI workflow change that was mistakenly prefixed `feat(ci):` instead of `ci:`; per the Conventional Commits table in [README](https://github.com/Alpaq92/Flags.Icons/blob/main/README.md#conventional-commits) it should have been a hidden, non-releasing commit.

### Features

* **ci:** auto-merge release-please PR once checks pass ([dae9890](https://github.com/Alpaq92/Flags.Icons/commit/dae9890fa2d39585289ccc60b00472827259868f))

## [1.0.1](https://github.com/Alpaq92/Flags.Icons/compare/v1.0.0...v1.0.1) (2026-05-24)


### Bug Fixes

* add macOS RuntimeIdentifiers to Eto demo for Mac64 bundling ([70d4221](https://github.com/Alpaq92/Flags.Icons/commit/70d42213aba7481d4ff64edd037439ba334ce562))
* Addressed MAUI build issue ([25971f4](https://github.com/Alpaq92/Flags.Icons/commit/25971f4eadb0a2745b6ca952cf21a1b4cae0eb94))
* align Avalonia compiled bindings + bump WinUI demo WindowsAppSDK to 2.1.3 ([e997958](https://github.com/Alpaq92/Flags.Icons/commit/e997958289e4b0a9da7aac288fef216394890e3c))
* correct app.manifest reference to match on-disk app.manifest.xml ([0d808ac](https://github.com/Alpaq92/Flags.Icons/commit/0d808ac94efaa4f280a62690e31d894e1fb36be1))

## 1.0.0 (2026-05-24)


### Features

* Drop-in FlagIcon controls for C#/.NET UIs, bundling FlagKit's PNG & SVG country flag assets ([2a738e8](https://github.com/Alpaq92/Flags.Icons/commit/2a738e8723b99a9582b6d8bd54a1251b58e8a8c3))
* Updated README ([30fbb1a](https://github.com/Alpaq92/Flags.Icons/commit/30fbb1ab38ffa4e65e0af25b73084bff95537649))

## Changelog

All notable changes to this project are documented in this file.

This changelog is managed automatically by [release-please](https://github.com/googleapis/release-please). New entries are derived from [Conventional Commits](https://www.conventionalcommits.org/) on `main` (`feat:`, `fix:`, `perf:`, `deps:`, `revert:`) and assembled into the release PR that release-please opens against `main`. Merging that PR cuts a tag and creates the matching GitHub Release.

Sections marked `docs:`, `chore:`, `refactor:`, `test:`, `build:`, `ci:` are intentionally omitted — they don't ship to NuGet consumers.
