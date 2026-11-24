// Sample BWUI widget: free space on fixed drives. Shows the header button, meter/ring layouts,
// sizes, status values and the loading/empty states. `--snapshot x.png light gallery` renders every component.
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Bwui;

namespace BwuiSample
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Bwui.App.Run("BwuiDisk", args, () =>
            {
                if (Bwui.App.IsSnapshot && Array.IndexOf(Bwui.App.SnapshotArgs, "gallery") >= 0) return new GalleryWidget();
                return new DiskWidget();
            });
        }
    }

    class DiskInfo { public string Name; public double FreePct; public long FreeBytes; }

    class DiskWidget : BwuiWindow
    {
        static readonly Color AccentLight = Palette.Hex("#0F6CBD"), AccentDark = Palette.Hex("#4CC2FF");
        List<DiskInfo> disks;
        DateTime? at;
        bool rings;

        public DiskWidget() : base("BwuiDisk", "Disk space")
        {
            rings = Settings.GetBool("rings", false) || Array.IndexOf(Bwui.App.SnapshotArgs ?? new string[0], "ring") >= 0;
            Load();
            Render();
            if (Bwui.App.IsSnapshot) return;
            // Local read: cheap, but there is no reason to do it more often than this (DESIGN.md §6.4).
            var timer = new DispatcherTimer { Interval = Bwui.App.Interval(TimeSpan.FromMinutes(1)) };
            timer.Tick += (s, e) => { Load(); Render(); timer.Interval = Bwui.App.Interval(TimeSpan.FromMinutes(1)); };
            timer.Start();
            Closed += (s, e) => timer.Stop();
        }

        protected override WidgetSize[] SupportedSizes { get { return new[] { WidgetSize.Regular, WidgetSize.Compact, WidgetSize.Wide }; } }
        protected override bool CanRefresh { get { return true; } }
        protected override void RefreshNow() { Load(); Render(); }

        protected override void AddMenuItems(ContextMenu menu)
        {
            menu.Items.Add(Item(Bwui.Text.T("디자인 전환 (막대 / 원형)", "Switch layout (bars / rings)"), ToggleLayout));
        }

        void ToggleLayout()
        {
            rings = !rings;
            Settings.SetBool("rings", rings);
            Render();
        }

        void Load()
        {
            var list = new List<DiskInfo>();
            try
            {
                foreach (var d in DriveInfo.GetDrives())
                {
                    if (d.DriveType != DriveType.Fixed || !d.IsReady || d.TotalSize <= 0) continue;
                    list.Add(new DiskInfo { Name = d.Name.TrimEnd('\\'), FreeBytes = d.AvailableFreeSpace, FreePct = 100.0 * d.AvailableFreeSpace / d.TotalSize });
                }
            }
            catch { }
            disks = list;
            at = DateTime.UtcNow;
        }

        protected override UIElement Build()
        {
            var p = Theme.Current;
            var accent = p.Accent(AccentLight, AccentDark);
            var sp = new StackPanel();
            var toggle = Ui.IconButton(LayoutGlyph(), Bwui.Text.T("디자인 전환 (막대 / 원형)", "Switch layout (bars / rings)"), ToggleLayout);
            sp.Children.Add(Ui.Header(Ui.Icon(null, accent, Glyphs.HardDrive), Bwui.Text.T("디스크 여유 공간", "Free disk space"), toggle));

            if (disks == null) { sp.Children.Add(Ui.Skeleton(ContentWidth, 2)); return sp; }
            if (disks.Count == 0) { sp.Children.Add(Ui.Message(Bwui.Text.T("표시할 드라이브가 없어요", "No fixed drives found"))); return sp; }

            int max = CurrentSize == WidgetSize.Compact ? 2 : CurrentSize == WidgetSize.Wide ? 6 : 4;
            var shown = disks.GetRange(0, Math.Min(max, disks.Count));
            if (rings)
            {
                int cols = CurrentSize == WidgetSize.Wide ? 3 : 2;
                for (int i = 0; i < shown.Count; i += cols)
                {
                    var row = new System.Windows.Controls.Primitives.UniformGrid { Columns = cols, Margin = new Thickness(0, Tokens.Space3, 0, 0) };
                    for (int j = i; j < Math.Min(i + cols, shown.Count); j++)
                    {
                        var d = shown[j];
                        row.Children.Add(Ui.Ring(d.FreePct / 100, Math.Round(d.FreePct) + "%", d.Name, Gb(d.FreeBytes), accent, Ui.StatusForRemaining(d.FreePct, 15, 5)));
                    }
                    sp.Children.Add(row);
                }
            }
            else
            {
                foreach (var d in shown)
                {
                    var r = Ui.MeterRow(d.Name, Math.Round(d.FreePct) + "%", d.FreePct / 100, Gb(d.FreeBytes) + Bwui.Text.T(" 남음", " free"), accent, Ui.StatusForRemaining(d.FreePct, 15, 5), ContentWidth);
                    ((FrameworkElement)r).Margin = new Thickness(0, Tokens.Space3, 0, 0);
                    sp.Children.Add(r);
                }
            }
            return sp;
        }

        static string Gb(long bytes) { return (bytes / 1073741824.0).ToString(bytes >= 100L * 1073741824 ? "0" : "0.0") + " GB"; }

        UIElement LayoutGlyph()
        {
            var c = Ui.Br(Theme.Current.TextSecondary);
            if (!rings) return new Ellipse { Width = 12, Height = 12, Stroke = c, StrokeThickness = 2 };
            var sp = new StackPanel { VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
            for (int i = 0; i < 2; i++)
                sp.Children.Add(new Border { Width = i == 0 ? 12 : 8, Height = 2, CornerRadius = new CornerRadius(1), Background = c, Margin = new Thickness(0, 2, 0, 2), HorizontalAlignment = HorizontalAlignment.Left });
            return sp;
        }
    }

    /// <summary>Every BWUI component and state on one card, for docs and visual checks.</summary>
    class GalleryWidget : BwuiWindow
    {
        public GalleryWidget() : base("BwuiGallery", "BWUI gallery") { }

        protected override UIElement Build()
        {
            var p = Theme.Current;
            var accent = p.Accent(Palette.Hex("#D97757"), Palette.Hex("#E38B6B"));
            double w = ContentWidth;
            var sp = new StackPanel();
            sp.Children.Add(Ui.Header(Ui.Icon(null, accent, Glyphs.Info), "Header title", Ui.IconButton(Glyphs.Refresh, "Refresh", null)));
            AddRow(sp, Ui.MeterRow("Normal", "72%", 0.72, "2h 10m", accent, Status.Normal, w));
            AddRow(sp, Ui.MeterRow("Caution", "18%", 0.18, "3d 4h", accent, Status.Caution, w));
            AddRow(sp, Ui.MeterRow("Critical", "4%", 0.04, "<1 min", accent, Status.Critical, w));
            var rr = Ui.RingRow(
                Ui.Ring(0.72, "72%", "Normal", "2h 10m", accent, Status.Normal),
                Ui.Ring(0.18, "18%", "Caution", "3d 4h", accent, Status.Caution),
                Ui.Ring(0.04, "4%", "Critical", "", accent, Status.Critical));
            rr.Margin = new Thickness(0, Tokens.Space3, 0, 0);
            sp.Children.Add(rr);

            sp.Children.Add(Ui.Divider());
            var stale = Freshness.Of(DateTime.UtcNow.AddMinutes(-42), TimeSpan.FromMinutes(12), TimeSpan.FromHours(6));
            sp.Children.Add(Ui.Header(Ui.Icon(null, accent, Glyphs.Info), "Stale", Ui.Text(stale.Caption, TextStyle.Caption)));
            var expired = Freshness.Of(DateTime.UtcNow.AddHours(-7), TimeSpan.FromMinutes(12), TimeSpan.FromHours(6));
            var body = Ui.MeterRow("Expired (50%)", "55%", 0.55, expired.Caption, accent, Status.Normal, w);
            body.Opacity = expired.BodyOpacity;
            AddRow(sp, body);

            sp.Children.Add(Ui.Divider());
            sp.Children.Add(Ui.Header(Ui.Icon(null, accent, Glyphs.Info), "Loading"));
            sp.Children.Add(Ui.Skeleton(w, 1));

            sp.Children.Add(Ui.Divider());
            sp.Children.Add(Ui.Header(Ui.Icon(null, accent, Glyphs.Info), "Error"));
            sp.Children.Add(Ui.Message("agy CLI를 찾을 수 없어요", Bwui.Text.Retry, () => { }));

            sp.Children.Add(Ui.Divider());
            var t = new TextBlock();
            foreach (TextStyle s in Enum.GetValues(typeof(TextStyle)))
                t.Inlines.Add(Ui.Span(s + " ", s, Ui.DefaultColor(s)));
            t.TextWrapping = TextWrapping.Wrap;
            sp.Children.Add(t);
            return sp;
        }

        static void AddRow(StackPanel sp, FrameworkElement e)
        {
            e.Margin = new Thickness(0, Tokens.Space3, 0, 0);
            sp.Children.Add(e);
        }
    }
}
