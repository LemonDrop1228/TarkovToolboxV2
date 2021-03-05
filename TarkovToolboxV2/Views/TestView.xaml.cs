using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace TarkovToolboxV2.Views
{
    /// <summary>
    /// Interaction logic for TestView.xaml
    /// </summary>
    public partial class TestView : BaseView, INotifyPropertyChanged
    {
        private string contentUrl;

        public string ContentUrl { get => contentUrl; set => SetField(ref contentUrl, value, "ContentUrl"); }

        [JsonIgnore()]
        public ChromiumWebBrowser cBrowser { get; set; }
        public bool CanSpecialResize { get; set; } = true;
        public bool isBrowserBasedView { get; set; } = true;

        public delegate void ViewClosedEventHandler(BaseView senderView);
        public event ViewClosedEventHandler ViewClosed;
        Image ImageMessage { get; set; } = new Image();
        public bool UsesMessage { get; set; }
        public bool StartCentered { get; set; }
        public bool ShowOnce { get; set; }


        public ImageSource MessageSource { get => ImageMessage.Source; set => ImageMessage.Source = value; }

        public TestView()
        {
            this.DataContext = this;
            InitializeComponent();

            this.Visibility = Visibility.Hidden;
        }

        private void PostInit()
        {
            if (ShowOnce && Properties.Settings.Default.FirstRun)
                this.Visibility = Visibility.Visible;

            if (UsesMessage)
                MainContainerBorder.Child = ImageMessage;
            if (StartCentered)
            {
                double Left = (((this.Parent as Canvas).ActualWidth / 2) - this.ActualWidth / 2);
                Canvas.SetLeft(this, Left);

                double Top = (((this.Parent as Canvas).ActualHeight / 2) - this.ActualHeight / 2);
                Canvas.SetTop(this, Top);
            }
        }

        #region BaseCalls
        private void Title_MouseLeftButtonDownPortal(object sender, MouseButtonEventArgs e)
        {
            base.Title_MouseLeftButtonDown(sender, e);
        }

        private void Title_MouseLeftButtonUpPortal(object sender, MouseButtonEventArgs e)
        {
            base.Title_MouseLeftButtonUp(sender, e);
        }

        private void Title_MouseMovePortal(object sender, MouseEventArgs e)
        {
            base.Title_MouseMove(sender, e, this);
        }

        private void BaseView_Loaded(object sender, RoutedEventArgs e)
        {
            if (Parent != null)
                this.canvas = Parent as Canvas;
            if (TitleBorder != null)
                this.Title = TitleBorder;

            if(isBrowserBasedView)
                cBrowser.IsBrowserInitializedChanged += CBrowser_IsBrowserInitializedChanged;


            if (!CanSpecialResize)
                ViewWindowControlsStackPanel.Children.Remove(VisibilityBorder);
            
            PostInit();
        }

        private void CBrowser_IsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            MainContainerBorder.Child = cBrowser;
            if (cBrowser.IsBrowserInitialized)
                cBrowser.Load(ContentUrl);
        }
        #endregion

        private void OpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double newOpacity = Math.Round(e.NewValue, 0) / 100;
            Console.WriteLine(newOpacity);
            Opacity = newOpacity;
        }

        private void VisibilityBorderButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            LayoutGrid.Visibility = LayoutGrid.Visibility switch
            {
                Visibility.Visible => Visibility.Hidden,
                Visibility.Hidden => Visibility.Visible,
                _ => Visibility.Hidden
            };
        }

        public void ToggleVisibility()
        {
            this.Visibility = this.Visibility switch
            {
                Visibility.Visible => Visibility.Hidden,
                Visibility.Hidden => Visibility.Visible,
                _ => Visibility.Hidden
            };

        }

                


        private void SettingsBorderButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void HideWindowBorderButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Visibility = Visibility.Hidden;

            if(!UsesMessage)
                this.ViewClosed(this);
        }
    }
}
