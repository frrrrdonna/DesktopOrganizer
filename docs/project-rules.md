# Project Rules

This file contains project-specific rules that extend the high-level engineering guidance in `AGENT.md`.

## 1. Versioning

The project version is defined in `src/DesktopOrganizer.App/DesktopOrganizer.App.csproj` as the `<Version>` property. This is the single source of truth. Changelogs, backups, and release tags all use this version number.

Follow Semantic Versioning (`MAJOR.MINOR.PATCH`) strictly.

When to bump:

- MAJOR (`X.0.0`): breaking architectural change. Requires explicit user approval.
- MINOR (`0.X.0`): a new spec-defined milestone or a new feature group.
- PATCH (`0.0.X`): bug fixes, small UX tweaks, refactors without behavior changes, or documentation-only changes recorded under the current version.

Rules:

- At the start of every conversation, check whether `AGENT.md`, `docs/project-rules.md`, or other active rule files have changed, and confirm the current project version from `src/DesktopOrganizer.App/DesktopOrganizer.App.csproj` before doing further work.
- Do not bump the version just because a file changed.
- Documentation-only changes stay in the current version.
- When the user explicitly says "new version" or points to a new milestone, bump MINOR.
- Update `<Version>` in `.csproj` before writing a version changelog.

## 2. Changelogs

Write a changelog only when a version is cut. One version equals one changelog file.

Location:

- `docs/changelogs/v<version>.md`

Required sections:

- Version overview
- Added
- Changed
- Removed
- Known Issues
- Test results

Rules:

- Append to the existing changelog if the version already exists.
- Every touched file must appear in exactly one of `Added`, `Changed`, or `Removed`.

## 3. Backups

Create a source backup only when cutting a version, after build and test verification pass.

Command:

```powershell
tar -czf "backups/v<version>.tar.gz" --exclude='bin' --exclude='obj' --exclude='.vs' --exclude='.git' --exclude='backups' .
```

Rules:

- Use the exact version from `.csproj`.
- Verify the archive exists and has non-zero size.

## 4. Bug Tracking

After fixing a non-trivial bug, document it in `docs/bug-tracker.md`.

Each entry should include:

- Title with severity
- Symptom
- Root Cause
- Solution
- Key Files
- Lesson

If the lesson applies project-wide, add a matching rule to this file.

## 5. WPF-Specific Rules

- Never add `<UseWindowsForms>true</UseWindowsForms>` to a WPF project.
- Any `WindowStyle="None"` window must provide an alternative exit path.
- Every call that hides desktop icons must have a reliable restore mechanism.
- Any close-to-tray behavior must provide an explicit bypass for real application shutdown.
- WPF `Button` does not support a `CornerRadius` property directly.
- `AllowsTransparency="True"` requires `WindowStyle="None"` and `ResizeMode="NoResize"`.

## 6. File Encoding And Line Endings

- Use UTF-8 without BOM for `.cs`, `.xaml`, `.csproj`, `.md`, `.json`.
- Use LF for `.md`, `.json`, `.slnx`.
- Use CRLF for `.cs`, `.xaml`, `.csproj`.
- Every text file must end with a single trailing newline.

On Windows, verify encoding with PowerShell or an editor that exposes encoding metadata.

## 7. Naming Conventions

Follow standard C# naming conventions.

| Scope | Convention | Example |
|---|---|---|
| Namespace | PascalCase | `DesktopOrganizer.App.ViewModels` |
| Class / Struct / Record | PascalCase | `BasketViewModel` |
| Interface | `I` + PascalCase | `IDesktopScanner` |
| Method | PascalCase | `LoadAsync` |
| Property | PascalCase | `IsCollapsed` |
| Private field | `_camelCase` | `_saveDebounceCts` |
| Local variable | camelCase | `staleSnapshot` |
| Parameter | camelCase | `cancellationToken` |
| XAML file | PascalCase | `BasketView.xaml` |
| Git branch | kebab-case, optional category prefix | `fix-tray-exit`, `feature/drag-baskets` |

Rules:

- Async methods must end with `Async`.
- ViewModel commands use `[RelayCommand]` on private methods.

## 8. Project Structure

```text
src/
  DesktopOrganizer.Core/
    Models/
    Abstractions/
  DesktopOrganizer.App/
    Views/
    ViewModels/
    Services/
  DesktopOrganizer.Infrastructure/
    Desktop/
    Persistence/
    Shell/
tests/
  DesktopOrganizer.Core.Tests/
```

Rules:

- Models stay in `Core/Models/`.
- Services stay in `App/Services/`.
- Platform-specific Win32/PInvoke code stays in `Infrastructure/`.
- One ViewModel per View.
- `DesktopOrganizer.Core` must not reference `System.Windows`.

## 9. MVVM And XAML Patterns

This project uses `CommunityToolkit.Mvvm`.

Rules:

- Use `[ObservableProperty]` for simple observable fields.
- Use `partial void On<PropertyName>Changed` for change reactions.
- Use `OnPropertyChanged(nameof(...))` for dependent computed properties.
- All ViewModels inherit from `BaseViewModel`.
- Keep WPF-specific interaction in `.xaml.cs` when necessary.
- Keep `WindowStyle="None"` windows wired to a visible or tray-based exit path.

## 10. Documentation Language

- `AGENT.md` and project-level rules are in English.
- Changelogs must be internally consistent within each file.
- Code comments are in English.
- Release-facing notes for users may be written in Chinese.
