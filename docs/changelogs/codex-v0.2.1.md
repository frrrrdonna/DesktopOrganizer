# codex v0.2.1 - 2026-07-03

## Version overview

This version defines the transparent desktop mapping scope for the next desktop-layer milestone and documents how other agents should implement it.

## Added

| File | Description |
|------|-------------|
| `docs/v0.2.1-transparent-desktop-spec.md` | Detailed implementation spec for the transparent wallpaper-preserving desktop mapping version. |

## Changed

None.

## Removed

None.

## Known Issues

- The current codebase still uses an opaque desktop host visual and has not yet been updated to the transparent overlay look described in this document.
- Desktop session snapshot persistence is specified here but not yet implemented in code.
- This documentation does not itself change runtime behavior; follow-up implementation work is still required.

## Test results

- Documentation-only change: no build/test run required
