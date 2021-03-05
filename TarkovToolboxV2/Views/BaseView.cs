using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TarkovToolboxV2.Views
{
    public class BaseView : UserControl
    {
        private string titleText;
        private string titleIcon;
        protected Border Title { get; set; }
        public Canvas canvas { get; set; }
        public Point StartMousePosition { get; private set; }

        public string TitleText { get => titleText; set => SetField(ref titleText, value, "TitleText"); }

        public string TitleIcon { get => titleIcon; set => SetField(ref titleIcon, value, "TitleIcon"); }

        public BaseView() : base()
        {
            this.RenderTransform = new TranslateTransform();
        }

        public void Title_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            StartMousePosition = e.GetPosition(this.canvas);
            if (!Title.IsMouseCaptured)
                Title.CaptureMouse();
        }

        public void Title_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Title.IsMouseCaptured)
            {
                Point mousePosition = e.GetPosition(this.canvas);
                Canvas.SetLeft(this, Canvas.GetLeft(this) + mousePosition.X - StartMousePosition.X);
                Canvas.SetTop(this, Canvas.GetTop(this) + mousePosition.Y - StartMousePosition.Y);
                Title.ReleaseMouseCapture();
            }
        }

        public void Title_MouseMove(object sender, MouseEventArgs e, UIElement element)
        {
            if (Title.IsMouseCaptured && e.LeftButton == MouseButtonState.Pressed)
            {
                Point mousePosition = e.GetPosition(this.canvas);
                Console.WriteLine(mousePosition);



                double L = Canvas.GetLeft(this);
                double T = Canvas.GetTop(this);
                Console.WriteLine($"Transform Left: {L}\nTransform Top: {T}");

                double mouseL = mousePosition.X - StartMousePosition.X;
                double mouseT = mousePosition.Y - StartMousePosition.Y;
                Console.WriteLine($"Mouse Left: {mouseL}\nMouse Top: {mouseT}");

                double LeftPos = L + mouseL;
                double TopPos = T + mouseT;
                Console.WriteLine($"Computed Left: {LeftPos}\nComputed Top: {TopPos}");

                if(LeftPos > 0.000 && LeftPos < canvas.ActualWidth - this.ActualWidth)
                    Canvas.SetLeft(this, LeftPos);

                if(TopPos > 0.000 && TopPos < canvas.ActualHeight - this.ActualHeight)
                    Canvas.SetTop(this, TopPos);

                StartMousePosition = mousePosition;
            }
        }


        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion


    }
}
