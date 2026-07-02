# codex v0.1.1 - 2026-07-02

## Version overview

This version hardens the desktop organizer alpha build by fixing high-priority interaction and persistence usability issues.

## Added

| File | Description |
|------|-------------|
| `src/DesktopOrganizer.App/Services/UserPrompts.cs` | Centralized confirmation dialogs for deleting groups and items. |
| `docs/release-checklist.md` | Alpha packaging and release readiness checklist. |

## Changed

| File | Description |
|------|-------------|
| `src/DesktopOrganizer.App/ViewModels/MainWindowViewModel.cs` | Replaced immediate auto-save with debounced saving and converted callback flow to `Task`-based async handlers. |
| `src/DesktopOrganizer.App/ViewModels/FenceGroupViewModel.cs` | Added path validation, duplicate prevention, trimmed rename commits, reset-on-cancel behavior, and confirmation-gated removals. |
| `src/DesktopOrganizer.App/Views/FenceGroupView.xaml` | Reworked button labels to plain text, added rename textbox lost-focus support, and removed broken symbol-based UI labels. |
| `src/DesktopOrganizer.App/Views/FenceGroupView.xaml.cs` | Added rename lost-focus commit behavior, skip feedback for invalid/duplicate drag-drop items, and reused stable border brushes. |

## Removed

None.

## Known Issues

- Workspace schema versioning is still missing, so future JSON shape changes may need migration support.
- Group layout is still driven by `WrapPanel`, not persisted absolute positioning.
- Delete confirmations are synchronous message boxes and may later need styling or localization.
- App-level automated tests still do not cover WPF interaction logic.

## Test results

- `dotnet test DesktopOrganizer.slnx`: passed, 5 passed / 0 failed
- `dotnet build DesktopOrganizer.slnx -c Release`: passed, 0 warnings / 0 errors
- Release GUI startup smoke test: passed (`GUI_START_OK`)
