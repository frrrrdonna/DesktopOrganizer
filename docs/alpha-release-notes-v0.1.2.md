# Desktop Organizer v0.1.2-alpha

## Summary

This alpha build is the first internally packageable version of Desktop Organizer.

## Included

- create groups;
- rename groups;
- collapse and expand groups;
- drag files, folders, and shortcuts into groups;
- open items from groups;
- remove items and groups with confirmation;
- JSON workspace persistence;
- debounced auto-save.

## Known Limitations

- group layout is still wrap-based, not absolute-positioned;
- workspace schema migration is not implemented;
- WPF interaction paths are validated by manual testing, not automated UI tests;
- the app does not yet include an installer or custom app icon.

## Validation Performed

- `dotnet test DesktopOrganizer.slnx`
- `dotnet build DesktopOrganizer.slnx -c Release`
- Release GUI startup smoke test

## Intended Use

This package is for internal alpha testing only.
