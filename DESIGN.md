# BWUI design spec

BWUI is a small design system for always-on Windows desktop widgets. It sets out how a widget should look and behave, and the `BWUI/` folder implements it for WPF.

The spec is the minimum bar that Apple (WidgetKit HIG), Android (App widget quality tiers) and Microsoft (Windows widgets, Fluent) agree on, adapted to a widget that lives on the Windows desktop. Where those platforms publish no number, BWUI picks one and says so.

Each rule is marked **MUST** or **SHOULD**. A widget that breaks a MUST is not a BWUI widget.

---

## 1. Principles

1. **Glanceable.** The key fact is readable in about one second. Details come on a second look.
2. **One job.** A widget answers one question. If the answer needs pages, tabs or scrolling, it belongs in an app.
3. **Calm.** No ambient animation and no attention-seeking color. Color goes to the data, not the chrome.
4. **Honest data.** Show data as soon as it exists. Say how old it is when it is old. Never hide stale data behind a spinner.
5. **Light footprint.** Do short pieces of work, then sleep. Don't refresh what nobody can see change.

---

## 2. Minimum checklist

Run through this before you publish a widget.

| # | Rule | Level | Why (source) |
|---|---|---|---|
| C1 | Uses the BWUI tokens for spacing, radius, type and color; no ad-hoc values | MUST | Consistency across widgets |
| C2 | Text contrast is at least 4.5:1, and graphics such as bars, rings and status glyphs at least 3:1, in every theme | MUST | WCAG AA; Android tier 3 fails this; Apple, Microsoft |
| C3 | Status is never shown by color alone; caution and critical values carry a glyph | MUST | Apple HIG, One UI |
| C4 | Supports Light, Dark and High Contrast; follows the system theme by default | MUST | All three platforms |
| C5 | Smallest text is 11px | MUST | Apple 11pt, Windows Caption 12px |
| C6 | Has a loading, empty, error and stale state (§5); none of them is a blank card | MUST | Android WT-1, WS-3; Windows; Apple |
| C7 | Stale data stays visible, with its age ("as of 12 min ago") | MUST | Apple, Windows |
| C8 | The widget's own name or service logo is visible in the header | MUST | Android WD-4.2 |
| C9 | Click targets are at least 28px (BWUI picks 28px; Apple's macOS minimum is 20pt) | MUST | Apple macOS HIG, Android |
| C10 | Never takes focus, never shows in the taskbar or Alt+Tab, and stays below normal windows | MUST | Win32 rules; "Predictable & Calm" |
| C11 | Per-monitor DPI aware (PerMonitorV2) | MUST | Microsoft high-DPI guidance |
| C12 | Network or process fetches run at most every 5 minutes unless the user asks; manual refresh is offered | MUST | Apple ≥5 min between entries; Android tier 2 WT-2 |
| C13 | Remembers its position and moves back on-screen if a monitor is gone | MUST | — |
| C14 | One instance per user | MUST | — |
| C15 | A preview image (`--snapshot`) for light and dark | SHOULD | Android WD-4; store listings |
| C16 | Refreshes half as often on battery | SHOULD | Apple, Android, Microsoft battery guidance |
| C17 | Offers more than one size if a larger size shows more, not just bigger text | SHOULD | All three |
| C18 | Interactive elements have an accessible name | SHOULD | Apple, Android |
| C19 | Published widgets update from GitHub Releases: notify by default, auto install opt-in, SHA-256 checked (§6.10) | SHOULD | Keeps BWUI fixes reaching users |

---

## 3. Tokens

All values are in device-independent pixels (DIP) on a **4px grid**. The code has the same names in `BWUI/Bwui.Theme.cs` (`Tokens`, `Palette`).

### 3.1 Spacing

| Token | Value | Use |
|---|---|---|
| `Space1` | 4 | Icon to text inside a control |
| `Space2` | 8 | Header icon to title; label to bar |
| `Space3` | 12 | Between rows; above and below a divider |
| `Space4` | 16 | Card padding (all sides) |
| `Space5` | 20 | — |
| `Space6` | 24 | Between large blocks |

### 3.2 Shape

| Token | Value | Notes |
|---|---|---|
| `CardRadius` | 24 | Outer corner of every widget card |
| `CardPadding` | 16 | The same on all four sides |
| `InnerRadius` | 8 | `CardRadius − CardPadding`, so inner shapes are concentric (Apple, Android) |
| Pill | height / 2 | Meters, round buttons, text buttons |
| `ShadowRoom` | 16 | Transparent margin around the card so the shadow isn't clipped |

Platforms publish no widget radius. BWUI uses 24 because it reads as "soft", and with 16 padding it leaves a clean 8 for inner shapes.

### 3.3 Sizes

A widget has a fixed content width; height follows the content.

| Size | Content width | Card width (+32) |
|---|---|---|
| Compact | 200 | 232 |
| Regular (default) | 264 | 296 |
| Wide | 352 | 384 |

A widget lists the sizes it supports. More than one size makes **Size** appear in the menu. A larger size MUST show more or clearer information. It must not only scale things up (C17).

### 3.4 Typography

The font stack is `Segoe UI Variable Text, Segoe UI, Malgun Gothic`. Icons use `Segoe Fluent Icons, Segoe MDL2 Assets`.

| Style | Size | Weight | Default color | Use |
|---|---|---|---|---|
| `Caption` | 11 | Regular | TextTertiary | Timestamps, reset times, notes. The minimum size. |
| `Label` | 12 | Regular | TextSecondary | Metric names |
| `Body` | 13 | Regular | TextPrimary | Sentences, list items |
| `Title` | 14 | SemiBold | TextPrimary | Header title |
| `Value` | 15 | SemiBold | TextPrimary | Numbers next to a label |
| `Display` | 22 | SemiBold | TextPrimary | One hero number, at most one per widget |

Two values on the same level of importance (for example 5-hour and weekly) use the **same** style. Hierarchy comes from position, not size.

### 3.5 Color

The palette is warm gray. Status colors are the Fluent semantic colors, so they look at home on Windows. Numbers are contrast against `Surface`; run `--contrast` to re-check them.

| Token | Light | Dark | Use |
|---|---|---|---|
| `Surface` | `#FFFFFF` | `#2B2A28` | Card |
| `SurfaceSubtle` | `#F4F3EF` | `#383734` | Button fill, skeleton |
| `SurfaceHover` | `#E9E7E0` | `#45433F` | Hover fill |
| `Track` | `#F1EFE8` | `#3D3C38` | Meter/ring background |
| `Divider` | `#EFEDE7` | `#3A3936` | 1px separators |
| `TextPrimary` | `#1F1E1C` (16.7) | `#F3F2EE` (12.8) | Titles, values |
| `TextSecondary` | `#6B6962` (5.5) | `#BDBBB4` (7.5) | Labels |
| `TextTertiary` | `#767470` (4.7) | `#A3A19A` (5.6) | Captions |
| `Caution` | `#9D5D00` (5.3) | `#FCE100` (10.9) | ≤ caution threshold |
| `Critical` | `#C42B1C` (5.7) | `#FF99A4` (7.1) | ≤ critical threshold, errors |
| `Success` | `#0F7B0F` (5.4) | `#6CCB5F` (7.1) | Positive status (rare) |

**High Contrast** uses Windows system colors (`Window`, `WindowText`, `Highlight`, `GrayText`). It drops the shadow and adds a 1px `WindowText` border. Accent and status colors all become `Highlight`, so the glyphs (C3) carry the meaning.

**Accent.** Each widget brings one accent, usually its service's brand color, as a light/dark pair. Accent is for graphics: meter fills, ring strokes, the header icon fallback. Text stays `TextPrimary` unless it shows a status. Every accent MUST reach 3:1 against `Surface` in its theme.

**Status thresholds.** For "remaining" values the defaults are caution at 20% or less and critical at 5% or less. A widget may change the numbers, but not the meaning.

### 3.6 Elevation

There is one shadow, on the card only: blur 24, offset 4 down, color `#1E283C` at 16% in light and `#000000` at 45% in dark. Nothing inside the card has a shadow.

### 3.7 Iconography

- **Header icon:** 20×20, the service's own logo. If there is no logo, use an accent-filled rounded square (radius 6) with a white Fluent glyph.
- **Glyphs:** Fluent icon font, 12–14px, `TextSecondary`.
- **Images:** load with `BitmapCacheOption.OnLoad` and freeze them.

---

## 4. Components

All components are factory methods on `Bwui.Ui`.

| Component | Spec |
|---|---|
| **Card** | Provided by `BwuiWindow`: `Surface`, radius 24, padding 16, shadow. One card per widget. |
| **Header** | Height 24. 20px icon, 8 gap, `Title`. An optional trailing element (caption or icon button) is right-aligned. Every widget has one (C8). |
| **Divider** | 1px `Divider`, 12 above and below. It separates sections, never rows. |
| **IconButton** | 28px circle, `SurfaceSubtle` fill, `SurfaceHover` on hover, 14px glyph in `TextSecondary`. It MUST have a tooltip, which is also its accessible name. A header button sits on the header line. |
| **TextButton** | Pill, height 28, padding 12 horizontally, `SurfaceSubtle`, `Label` text in `TextPrimary`. Used for actions in a message ("Retry", "Open settings"). |
| **MeterRow** | Line 1: `Label` + space + `Value`, with a `Caption` aligned right. Line 2: an 8px pill meter, 8 below. Fill is the accent, or the status color. A non-zero fill is at least as wide as it is tall. Rows are 12 apart. |
| **Ring** | 64px, 8px stroke, round caps. `Value` sits centered, with `Label` and `Caption` centered below. Rings sit 2 or 3 per row in a uniform grid. |
| **StatusValue** | `Value` text. At caution or critical it turns the status color and gets a 12px glyph before it: ⚠ `E7BA` for caution, ⛔ `EA39` for critical (C3). |
| **ListRow** | A clickable row: transparent, `SurfaceSubtle` on hover, radius 8, padding 8 horizontally. It needs an accessible name. Used for lists that open something (a patch note, an item). |
| **CountBadge** | A 20px pill in a tile's top-right corner: accent fill, SemiBold `Caption` number in `Surface`, and a 2px `Surface` ring. The number is the cue (C3). Past 99 it shows "99+". |
| **StatusMark** | A 20px `Surface` disc in a tile's bottom-right corner, holding the 12px status glyph |
| **Disabled** | A disabled control drops to 45% opacity and isn't clickable. This applies to TextButton and menu items. |
| **Message** | `Caption` text, wrapping, with an optional TextButton below. Used for the empty, error and setup states. |
| **Skeleton** | `SurfaceSubtle` rounded bars (height 8, radius 4) at the size of the content they stand in for. Used for loading. |

**Second windows.** A transient window such as a popover with details uses the same card (`Ui.StyleCard`), follows the theme, and closes on Esc or on a click outside it. It may take focus while open, since it opens only on a user click. Use the Wide content width.

Widgets may build their own components from the tokens. If a component is reused by a second widget, it moves into `BWUI/`.

---

## 5. States

Every data source in a widget is in exactly one of these states.

| State | When | What to show |
|---|---|---|
| **Loading** | First start, no cached data yet | Header plus Skeleton rows. Never a blank card or a lone spinner. |
| **Fresh** | Data younger than `staleAfter` | Data only, without a timestamp. The widget is "live". |
| **Stale** | Age ≥ `staleAfter` (default: 2 refresh intervals + 2 min) | Data, plus `Caption` "as of 12 min ago" in the header trailing slot |
| **Expired** | Age ≥ `expireAfter` (widget-defined) | As Stale, but the body is dimmed to 50% opacity. Still visible (principle 4). |
| **Empty** | The source answered, but there is nothing to show (for example the plan has no limit) | Message: say plainly what is absent ("No usage limit"). |
| **Error** | The last fetch failed and no cache exists | Message with a reason a user can act on ("agy CLI not found") and a Retry button where a retry can help. With a cache, show the cache as Stale instead. |
| **Setup** | Signed out, not configured, or the tool is missing | Message saying what setting it up unlocks, plus an action if there is one |
| **Hidden** | The user turned the section off | Nothing at all, and the section is not fetched |

When every section is hidden, the widget shows a Message telling the user how to bring one back ("Right-click to show a service").

---

## 6. Behavior

### 6.1 Desktop layer (C10)
- The window is borderless and transparent, owned by the desktop (`Progman`), and has `WS_EX_TOOLWINDOW`. It doesn't show in the taskbar or Alt+Tab.
- Z-order is pinned to `HWND_BOTTOM` by handling `WM_WINDOWPOSCHANGING`.
- `ShowActivated = false`, and the window never calls `Activate()`.

### 6.2 Placement (C13)
- The first run places the widget at the bottom-right of the work area.
- Dragging anywhere on the card moves it, except on buttons. Position is saved on drop.
- At startup, a saved position that is no longer on any monitor falls back to the first-run placement.
- **Anchoring.** When content changes height, the anchored edges stay put. The default is the bottom-right anchor, so the widget grows up and to the left.

### 6.3 Context menu
The menu has the BWUI style: `Surface`, radius 8, `Body` text, a check column. There are no submenus. The order is fixed:

1. Widget items (layout toggle, per-section visibility, …)
2. separator
3. **Refresh now** (if the widget fetches anything)
4. **Size: …** (if more than one size; clicking cycles through them)
5. **Theme: System / Light / Dark** (clicking cycles through them)
6. **Start with Windows** (checkable; `HKCU\…\Run`, value = AppId)
7. separator, then **Check for updates** / **Install update (vX.Y.Z)** and **Update automatically** (only if the widget has an update source, §6.10)
8. separator
9. **Exit**

### 6.4 Refresh (C12, C16)
- Local reads, such as a file or `DriveInfo`, can run on any timer of 30s or longer.
- Network or child-process fetches run at most every **5 min**. The first fetch runs a few seconds after start, never during construction.
- On battery (`SystemParameters.PowerLineStatus == Offline`), intervals double; `App.Interval()` does this.
- A hidden section is not fetched.
- Fetch on a background thread and render on the UI thread. One fetch per source at a time.
- Cache the last good result to disk with its timestamp, so the next start shows data immediately (Stale, not Loading).

### 6.5 Settings
`%LOCALAPPDATA%\<AppId>\settings.txt` holds `key=value` lines in UTF-8. BWUI owns `left`, `top`, `theme` and `size`; widgets add their own keys. Caches live in the same folder.

### 6.6 Theme (C4)
The default is `System`, which follows `AppsUseLightTheme`. The widget re-renders when the system theme or High Contrast changes.

### 6.7 Resources
- Use software rendering (`RenderMode.SoftwareOnly`). A small window doesn't need a GPU device, and this cuts memory to about a third.
- Don't keep work running between refreshes; no polling loops.
- There is no memory cap; keep it in the tens of MB.

### 6.8 DPI (C11)
The app manifest declares `PerMonitorV2` (`BWUI/bwui.manifest`), and the assembly targets .NET Framework 4.8, so WPF rescales the window when it moves between monitors.

### 6.9 Snapshot (C15)
`<exe> --snapshot <file.png> [light|dark]` renders the card on a desk-colored background at 2× and exits. Widgets feed it sample or cached data, so the image looks real.

### 6.10 Updates
A widget that sets `Updates` (an `UpdateSource`: owner, repo, files) updates from its GitHub Releases.

- **Version.** The repo's `VERSION` file is the app version. `build.ps1` compiles it into `Bwui.AppInfo.Version`, and release tags are `v` + that version.
- **Releases.** Pushing a tag runs `.github/workflows/release.yml`, which builds on a Windows runner and publishes every file with a `.sha256` next to it.
- **Where it shows.** Update state appears **only in the right-click menu**. The widget face never shows an update bar, badge or dot (MUST).
- **Manual (default).** Nothing runs in the background. The menu shows **Check for updates (current vX.Y.Z)**, and clicking it makes one unauthenticated request to `api.github.com/repos/<owner>/<repo>/releases/latest`. The menu stays open while the same item changes:
  - **Checking…**, then
  - **Install update (vX.Y.Z)**, which installs when clicked, or
  - **Up to date (vX.Y.Z)**, or
  - **Couldn't update · Retry**, with the reason as a tooltip.
- **Auto (opt-in).** Only while **Update automatically** is checked, the widget checks a minute after start and then once a day, and installs a newer release without asking. Turning the option on checks right away. A failed background check stays silent.
- **Privacy.** The User-Agent is the generic `BWUI-Updater`; nothing about the user is sent.
- **Install.**
  1. Download each file over HTTPS from `github.com` and check its SHA-256, taken from GitHub's asset digest or the `.sha256` asset.
  2. A missing or wrong hash means nothing is installed.
  3. Rename the running files to `*.old`, move the new ones in, and restart with `--after-update <pid>`. The new process waits for the old one to exit, then deletes the `*.old` files.
  4. If a move fails, the old files are put back and the menu item says why.
- **Scope.** Only the installed copy (`%LOCALAPPDATA%\<AppId>`) updates itself; a build run from `bin\` never does. Settings, caches and position are untouched.
- **Limits.** The hash proves the file wasn't corrupted or swapped in transit, not who built it. That trust comes from the GitHub repo. The exes are unsigned, so SmartScreen may warn the first time a downloaded copy runs.

The menu's updates block sits after **Start with Windows**: **Check for updates** (or **Install update**), then **Update automatically**.

---

## 7. Accessibility (C2, C3, C18)
- Use real `TextBlock`s for text, so screen readers can read it. Don't put text into images.
- IconButtons get `AutomationProperties.Name` from their tooltip. Meters and rings get "Label, 72%".
- Meaning never comes from color alone (C3).
- Known limitation: desktop widgets are not keyboard-focusable, because they never activate (C10).

---

## 8. Writing
- Use the system language. Korean and English are built in (`Bwui.Text`); other languages fall back to English.
- Keep text short, with no trailing period on labels. Sentences in messages end politely ("…할 수 없어요").
- Durations use the two largest units: "2시간 10분", "3일 4시간" / "2h 10m", "3d 4h". Ages use one unit: "12분 전" / "12 min ago".
- Times are relative, never clock times, unless the clock time is the point.

---

## 9. Versioning
The `BWUI/VERSION` file holds the spec and code version. A widget vendors the whole `BWUI/` folder and records the version it copied. Breaking a token value or a MUST is a minor-version bump while BWUI is 0.x.
