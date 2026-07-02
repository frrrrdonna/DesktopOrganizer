# codex v0.1.2 - 2026-07-02

## Version overview

This version adds the minimum release metadata and documentation needed to produce an internal alpha package.

## Added

| File | Description |
|------|-------------|
| `docs/alpha-release-notes-v0.1.2.md` | Internal alpha release notes for the packageable build. |

## Changed

| File | Description |
|------|-------------|
| `src/DesktopOrganizer.App/DesktopOrganizer.App.csproj` | Added product identity and version metadata for release output. |

## Removed

None.

## Known Issues

- There is still no installer packaging format such as MSIX yet; this release is a publish-folder alpha.
- The application still uses the default executable icon.
- Manual smoke testing is still required after publish output is generated.

## Test results

- `dotnet publish src/DesktopOrganizer.App/DesktopOrganizer.App.csproj -c Release -o publish/alpha/v0.1.2`: passed
- Publish output GUI startup smoke test: passed (`GUI_START_OK`)
