# Desktop Organizer v0.3.3

Desktop Organizer `v0.3.3` combines the local `v0.3.1`, `v0.3.2`, and `v0.3.3` iterations into one GitHub release.

## Highlights

- Fixed the tray exit path so the app can shut down cleanly and restore desktop icons correctly.
- Added localized desktop-host UI, tray menu text, and a language selector in settings.
- Added double-click collapse on basket title bars and unified basket panel height behavior.
- Added persisted desktop-host state so basket layout, snap edge, collapsed state, and host settings survive restarts.
- Preserved saved basket layout across desktop refresh operations.

## Included In This Release

- `v0.3.1`: tray-exit shutdown bypass and desktop icon restore fix
- `v0.3.2`: localization support and basket interaction polish
- `v0.3.3`: desktop-host state persistence

## Verification

- `dotnet build DesktopOrganizer.slnx -c Release`
- `dotnet test DesktopOrganizer.slnx`
- Backup verified at `backups/v0.3.3.tar.gz`

## Known Limitations

- Language changes still require restart for full host-window refresh.
- Basket restore is not yet multi-monitor-aware.
- Basket identity is still limited to classifier-defined default keys.
- Desktop scan results are still generated at runtime rather than persisted.
