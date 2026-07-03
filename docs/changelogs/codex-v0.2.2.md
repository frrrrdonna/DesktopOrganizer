# codex v0.2.2 - 2026-07-03

## Version overview

This version prepares the current desktop-layer codebase for the v0.2.1-alpha package, release, and GitHub upload flow.

## Added

| File | Description |
|------|-------------|
| `docs/alpha-package-notes-v0.2.1.md` | Package notes for the v0.2.1 desktop-layer alpha build. |
| `docs/github-release-v0.2.1.md` | UTF-8 GitHub release body source for the v0.2.1 alpha release. |

## Changed

| File | Description |
|------|-------------|
| `src/DesktopOrganizer.App/DesktopOrganizer.App.csproj` | Updated assembly and product version metadata from `0.1.3-alpha` to `0.2.1-alpha`. |

## Removed

None.

## Known Issues

- The current working tree represents the v0.2.1 desktop-layer state, not a separately restorable v0.2.0 commit snapshot.
- Packaging is still publish-folder/zip based rather than an installer.

## Test results

- `dotnet test DesktopOrganizer.slnx`: passed, 5 passed / 0 failed
- `dotnet build DesktopOrganizer.slnx -c Release`: passed, 0 warnings / 0 errors
- Desktop host startup smoke test: passed (`GUI_START_OK`)
