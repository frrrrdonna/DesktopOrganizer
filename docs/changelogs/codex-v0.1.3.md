# codex v0.1.3 - 2026-07-02

## Version overview

This version prepares and produces a fast-to-test packaged alpha build for Windows x64.

## Added

| File | Description |
|------|-------------|
| `docs/alpha-package-notes-v0.1.3.md` | Notes for the packaged alpha test build. |

## Changed

| File | Description |
|------|-------------|
| `src/DesktopOrganizer.App/DesktopOrganizer.App.csproj` | Updated version metadata and enabled self-contained single-file Windows x64 publishing. |

## Removed

None.

## Known Issues

- This is still a publish-folder/zip delivery, not an installer.
- The executable still uses the default app icon.
- First launch on some machines may be slower because of single-file extraction behavior.

## Test results

- `dotnet publish src/DesktopOrganizer.App/DesktopOrganizer.App.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -o publish/packages/v0.1.3-alpha`: passed
- Published executable startup smoke test: passed (`GUI_START_OK`)
- Packaged zip creation: passed (`publish/packages/DesktopOrganizer-v0.1.3-alpha-win-x64.zip`)
