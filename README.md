# BWUI

BWUI is a small design system for always-on Windows desktop widgets. It is one spec ([DESIGN.md](DESIGN.md)) plus a few C# files (`BWUI/`) that implement it for WPF on the .NET Framework that ships with Windows.

| Light | Dark |
|---|---|
| ![Gallery, light](docs/gallery-light.png) | ![Gallery, dark](docs/gallery-dark.png) |

## What you get

- **Spec.** Principles, a minimum checklist, tokens (4px grid, radius, type ramp, warm-gray palette with Fluent status colors), components, data states and behavior. The checklist is the common bar of Apple's, Android's and Microsoft's widget guidelines.
- **`BwuiWindow`.** A borderless card that does what a desktop widget has to do:
  - lives on the desktop layer and never takes focus;
  - stays out of the taskbar and Alt+Tab;
  - remembers its position and keeps an anchored edge in place;
  - has the standard right-click menu (refresh, size, theme, start with Windows, exit);
  - follows the Windows theme and High Contrast;
  - is per-monitor DPI aware;
  - renders `--snapshot` previews;
  - updates itself from GitHub Releases, notifying by default or installing automatically if you opt in.
- **`Ui` components.** Header, icon button, text button, meter row, ring, status value with glyph, message, skeleton, divider.
- **Small footprint.** Software rendering, no dependencies, about 55 MB of RAM. It builds with `csc.exe` from `C:\Windows\Microsoft.NET` (C# 5), with no SDK or installer.
- **Korean and English** strings, plus relative-time formatting.

## Start a widget

1. Click **Use this template** on GitHub, or copy the `BWUI/` folder into your project.
2. Write a class that derives from `BwuiWindow`:

```csharp
using Bwui;

static class Program
{
    [System.STAThread]
    static void Main(string[] args) { Bwui.App.Run("MyWidget", args, () => new MyWidget()); }
}

class MyWidget : BwuiWindow
{
    public MyWidget() : base("MyWidget", "My widget") { Render(); }

    protected override System.Windows.UIElement Build()
    {
        var p = Theme.Current;
        var accent = p.Accent(Palette.Hex("#0F6CBD"), Palette.Hex("#4CC2FF"));
        var sp = new System.Windows.Controls.StackPanel();
        sp.Children.Add(Ui.Header(Ui.Icon(null, accent, Glyphs.Info), "My widget"));
        var row = Ui.MeterRow("Quota", "72%", 0.72, "2h 10m", accent, Ui.StatusForRemaining(72), ContentWidth);
        row.Margin = new System.Windows.Thickness(0, Tokens.Space3, 0, 0);
        sp.Children.Add(row);
        return sp;
    }
}
```

3. Add a `VERSION` file (for example `1.0.0`) and compile with the manifest, which turns on per-monitor DPI. The build writes the version into a one-line `AppInfo.g.cs`; see [`build.ps1`](build.ps1):

```powershell
Set-Content bin\AppInfo.g.cs "namespace Bwui { public static class AppInfo { public const string Version = `"$(Get-Content VERSION)`"; } }"
csc /target:winexe /codepage:65001 /win32manifest:BWUI\bwui.manifest /r:...WPF dlls... /r:System.Web.Extensions.dll BWUI\*.cs bin\AppInfo.g.cs MyWidget.cs
```

4. Go through the checklist in [DESIGN.md §2](DESIGN.md#2-minimum-checklist).

## Updates

A widget can update itself from its GitHub Releases. Override one property:

```csharp
protected override UpdateSource Updates
{
    get { return new UpdateSource("owner", "repo", "MyWidget.exe"); }
}
```

Then copy [`.github/workflows/release.yml`](.github/workflows/release.yml), set its `FILES`, and push a tag such as `v1.0.1` that matches `VERSION`. The workflow builds on a Windows runner and publishes each file with a `.sha256`.

Once a day the widget checks the latest release. If there's a newer one, it shows an update bar, and one click downloads it, checks the SHA-256, swaps the exe and restarts. Settings and position stay as they were. **Update automatically** in the right-click menu installs updates without asking. The details are in [DESIGN.md §6.10](DESIGN.md#610-updates).

![Update bar](docs/sample-update-light.png)

## Sample

`sample/DiskWidget.cs` shows free space on fixed drives. It uses the header button, bar and ring layouts, three sizes, and status values. Build it with:

```powershell
.\build.ps1 -Run          # build and start bin\BwuiDisk.exe
.\build.ps1 -Snapshots    # render docs\*.png and docs\contrast.txt
```

| Bars | Rings, dark |
|---|---|
| ![Sample](docs/sample-light.png) | ![Sample, rings, dark](docs/sample-rings-dark.png) |

## Command line

Every BWUI widget gets these:

| Argument | Does |
|---|---|
| `--snapshot <png> [light\|dark] [extra…]` | Renders the card at 2× and exits. Extra arguments go to the widget (`App.SnapshotArgs`). |
| `--contrast <txt>` | Writes the palette contrast report (DESIGN.md C2) and exits |
| `--debug-menu` | Opens the context menu, saves it as `debug-menu.png` in the widget's data folder, and exits |

## Widgets built with BWUI

- [windows-ai-usage-widget](https://github.com/byeonggwan/windows-ai-usage-widget): Claude, Codex and Antigravity usage limits
- [windows-game-patchnote-widget](https://github.com/byeonggwan/windows-game-patchnote-widget): game patch notes

## Versioning

`BWUI/VERSION` is the spec and code version. Widgets vendor the folder and record the version they copied. While BWUI is 0.x, changing a token value or a MUST rule is a minor-version bump.

## License

MIT
