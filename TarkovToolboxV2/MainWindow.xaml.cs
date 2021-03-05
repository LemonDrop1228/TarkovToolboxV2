using NonInvasiveKeyboardHookLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TarkovToolboxV2.Utils;
using System.Timers;
using System.Windows.Forms;
using Newtonsoft.Json;
using CefSharp.Wpf;
using System.Windows.Interop;
using System.Threading;
using TarkovToolboxV2.Views;

namespace TarkovToolboxV2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Drawing.Color trayContextBG = System.Drawing.Color.FromArgb(40, 51, 69);
        private System.Drawing.Color BackgroundBase = (System.Drawing.Color)System.Drawing.ColorTranslator.FromHtml("#FF3C4555");

        public KeyboardHook keyboardListener { get; set; }
        public System.Timers.Timer orphanedTimer { get; set; }
        NotifyIcon TrayIcon { get; set; }
        ContextMenuStrip TrayContextMenu { get; set; }

        public ChromiumWebBrowser MapBrowser { get; set; }
        public ChromiumWebBrowser MarketBrowser { get; set; }
        public ChromiumWebBrowser WikiBrowser { get; set; }

        public bool isOverlayShown => (new[] { MapView.Visibility, MarketView.Visibility, WikiView.Visibility }.Contains(Visibility.Visible));

        public List<ViewState> ViewState { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            ViewState = new List<ViewState>() {
                new ViewState( MapView, false),
                new ViewState( MarketView, false),
                new ViewState( WikiView, false),

            };

            MapView.ViewClosed += View_ViewClosed; ;
            MarketView.ViewClosed += View_ViewClosed;
            WikiView.ViewClosed += View_ViewClosed;

            MapView.cBrowser = MapBrowser = InitBrowser("Maps");
            MarketView.cBrowser = MarketBrowser = InitBrowser("Market");
            WikiView.cBrowser = WikiBrowser = InitBrowser("Wiki");


            keyboardListener = new KeyboardHook();
            keyboardListener.KeyDown += KeyboardListener_KeyDown; ;


            ProcessWatcher.StartWatcher();

            orphanedTimer = new System.Timers.Timer();
            orphanedTimer.Elapsed += OrphanedTimer_Elapsed; ;
            orphanedTimer.Enabled = true;
            orphanedTimer.Interval = 250;
            orphanedTimer.Start();


            PostInit();
        }

        private void View_ViewClosed(BaseView senderView)
        {
            ViewState.FirstOrDefault(v => v.View == senderView).Visible = false;
        }

        private void KeyboardListener_KeyDown(object sender, HookEventArgs e)
        {
            Console.WriteLine(JsonConvert.SerializeObject(e));
            if (e.Control && e.Shift && e.Key != Keys.O)
            {
                if (e.Key == Keys.M && (TarkovIsFocused() || OverlayIsFocused()))
                    MapView.ToggleVisibility();
                if (e.Key == Keys.P && (TarkovIsFocused() || OverlayIsFocused()))
                    MarketView.ToggleVisibility();
                if (e.Key == Keys.K && (TarkovIsFocused() || OverlayIsFocused()))
                    WikiView.ToggleVisibility();

                ViewState.ForEach(v => {
                    v.Visible = v.View.Visibility == Visibility.Visible;
                });

            }
            else if (e.Alt && e.Key == Keys.Tab)
                HideOverlays();
            else if (e.Control && e.Shift && e.Key == Keys.O && (TarkovIsFocused() || OverlayIsFocused()))
                ToggleOverlays();

        }

        private ChromiumWebBrowser InitBrowser(string title)
        {
            var cBrowser = new ChromiumWebBrowser();
            Window visual = System.Windows.Application.Current.Windows[System.Windows.Application.Current.Windows.Count - 1];
            HwndSource parentWindowHwndSource = (HwndSource)HwndSource.FromVisual(visual);
            cBrowser.CreateBrowser(parentWindowHwndSource, new Size(100, 100));
            cBrowser.Name = $"browser_{title}";
            return cBrowser;
        }


        private void ToggleOverlays()
        {
            if (isOverlayShown)
                MapView.Visibility = MarketView.Visibility = WikiView.Visibility = Visibility.Hidden;
            else
            {
                ViewState.ForEach(v => {
                    v.View.Visibility = (v.Visible ? Visibility.Visible : Visibility.Hidden);
                });
            }
        }

        private void PostInit()
        {
            TrayContextMenu = new ContextMenuStrip();
            TrayContextMenu.ShowCheckMargin = TrayContextMenu.ShowImageMargin = false;
            TrayContextMenu.Items.AddRange(GetTrayMenuItems());
            TrayContextMenu.BackColor = trayContextBG;
            TrayContextMenu.ForeColor = System.Drawing.Color.WhiteSmoke;


            TrayIcon = new NotifyIcon()
            {
                Icon = Properties.Resources.Toolsimage,
                Visible = true,
                Text = "Tarkov ToolBox",
                ContextMenuStrip = TrayContextMenu
            };
        }

        private ToolStripItem[] GetTrayMenuItems()
        {
            var mList = new List<ToolStripItem>();
            ToolStripItem menuItemTemplate;

            menuItemTemplate = new ToolStripLabel();
            menuItemTemplate.Text = "App";
            menuItemTemplate.ForeColor = System.Drawing.Color.DarkGoldenrod;
            mList.Add(menuItemTemplate);
            mList.Add(new ToolStripSeparator());
            menuItemTemplate = new ToolStripButton();
            menuItemTemplate.Click += (s, e) => { this.Close(); };
            menuItemTemplate.Text = "Exit";
            mList.Add(menuItemTemplate);
            mList.Add(new ToolStripSeparator());


            return mList.ToArray();
        }




        private void OrphanedTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //this.Dispatcher.Invoke((Action)(() =>
            //{
            //    if (!TarkovIsFocused() && (!ProcessWatcher.LastActiveWindowWasTarkov() || !ProcessWatcher.LastActiveWindowWasToolbox()))
            //        HideOverlays();
            //}));
        }

        private void HideOverlays()
        {
            WikiView.Visibility = MarketView.Visibility = MapView.Visibility = Visibility.Hidden;
        }

        private void CleanUp()
        {
            Console.WriteLine("Cleaning Keyboard Listener...");
            keyboardListener.Uninstall();
            Console.WriteLine("Cleaning Process Watcher...");
            ProcessWatcher.StopWatcher();
            Console.WriteLine("Cleaning Orphan Timer...");
            orphanedTimer.Stop();
            orphanedTimer.Dispose();
        }

        private bool TarkovIsFocused()
        {
            return TarkovStateChecker.IsTarkovActive();
        }

        private bool OverlayIsFocused()
        {
            return TarkovStateChecker.IsOverlayActive();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Properties.Settings.Default.FirstRun)
            {
                Properties.Settings.Default.FirstRun = false;
                Properties.Settings.Default.Reload();
            }

            if (TrayIcon != null)
            {
                TrayIcon.Visible = false;
                TrayIcon.Dispose();
            }
            CleanUp();
        }

        private void MainWindow1_Loaded(object sender, RoutedEventArgs e)
        {
            AltTabHelper.RemoveFromAltTab(this);
        }
    }

    public class ViewState
    {
        public ViewState(TestView view, bool visible)
        {
            View = view;
            Visible = visible;
        }

        public BaseView View { get; set; }
        public bool Visible { get; set; }
    }
}
