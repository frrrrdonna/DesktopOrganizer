# codex v0.1.5 - 2026-07-03

## Version overview

This version fixes the GitHub release description encoding by updating the release body from a UTF-8 local markdown source.

## Added

| File | Description |
|------|-------------|
| `docs/github-release-v0.1.3.md` | UTF-8 source for the GitHub release body of `v0.1.3-alpha`. |

## Changed

| File | Description |
|------|-------------|
| `GitHub release v0.1.3-alpha` | Replaced the garbled release body with a corrected Chinese description. |

## Removed

None.

## Known Issues

- The release asset itself is normal; the problem was limited to the release body text encoding.
- Future release creation should reuse a UTF-8 markdown file instead of embedding Chinese text directly in shell commands.

## Test results

- Backup creation: passed (`backups/codex-v0.1.5.tar.gz`)
- GitHub release body patch: passed
- GitHub release body UTF-8 verification: passed (`CURL_UTF8_BODY_MATCHES`)
