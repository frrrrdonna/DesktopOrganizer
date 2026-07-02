# Release Checklist

## Current Readiness

This project is ready for an internal alpha package, not a public stable release yet.

## Alpha Gate

Complete all items below before creating an alpha package:

- Run `dotnet test DesktopOrganizer.slnx`
- Run `dotnet build DesktopOrganizer.slnx -c Release`
- Launch the Release build and verify the window opens normally
- Create a group and rename it
- Collapse and expand a group
- Drag a file into a group
- Drag the same file again and confirm it is skipped
- Drag an invalid or missing path and confirm it is skipped
- Open an item from a group
- Remove an item and confirm the delete dialog appears
- Remove a group and confirm the delete dialog appears
- Close and reopen the app and confirm workspace persistence still works

## Do Not Package As Stable Until

- the main user flows have been manually verified on a clean machine;
- app metadata is finalized, including icon, version, and product name;
- release notes are written;
- at least one Release build has been smoke tested end to end.

## Suggested Packaging Timing

- Package `alpha` after the alpha gate passes.
- Package `beta` after the app has broader manual testing and the remaining major UX gaps are reduced.
- Package `stable` only after installation, upgrade, and persistence behavior are all proven.
