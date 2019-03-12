using Message.Interfaces;
using Message.Model;
using Message.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Message
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IView, IPasswordSupplier
    {
        public MainWindow()
        {
             

            InitializeComponent();

            MinimizeButton.Click += (s, e) => WindowState = WindowState.Minimized;
            CloseButton.Click += (s, e) => Close();

            DataContext = new MainWindowVM(this, this);
        }

        public void AnimatedResize(int h, int w)
        {
            Storyboard storyboard = new Storyboard();

            DoubleAnimation daWidth = new DoubleAnimation(ActualWidth, w, new Duration(TimeSpan.FromSeconds(0.3)));
            DoubleAnimation daHeight = new DoubleAnimation(ActualHeight, h, new Duration(TimeSpan.FromSeconds(0.2)));

            storyboard.Children.Add(daWidth);
            storyboard.Children.Add(daHeight);
            daHeight.BeginTime = new TimeSpan(0, 0, 0, 0, 300);

            Storyboard.SetTarget(daWidth, this);
            Storyboard.SetTarget(daHeight, this);

            Storyboard.SetTargetProperty(daWidth, new PropertyPath(WidthProperty));
            Storyboard.SetTargetProperty(daHeight, new PropertyPath(HeightProperty));

            BeginStoryboard(storyboard);
        }

        private void animateWidth(Window wnd, double to)
        {
            double w1 = wnd.ActualWidth;

            DoubleAnimation aninateWidth = new DoubleAnimation();
            aninateWidth.Duration = new Duration(TimeSpan.FromSeconds(1));
            aninateWidth.From = w1;
            aninateWidth.To = to;

            wnd.Dispatcher.Invoke(new Action(() =>
            {
                wnd.BeginAnimation(Window.WidthProperty, aninateWidth);
            }));
        }

        private void animateHeight(Window wnd, double to)
        {
            double h1 = wnd.ActualHeight;

            DoubleAnimation aninateHeight = new DoubleAnimation();
            aninateHeight.Duration = new Duration(TimeSpan.FromSeconds(1));
            aninateHeight.From = h1;
            aninateHeight.To = to;

            wnd.Dispatcher.Invoke(new Action(() =>
            {
                wnd.BeginAnimation(Window.HeightProperty, aninateHeight);
            }));
        }

        public void CloseWindow()
        {
            this.Close();
        }

        public string GetPasswordForLogin()
        {
            return PasswordBox.Password;
        }

        public string GetPasswordForRegistration()
        {
            return RPasswordBox.Password;
        }

        public string GetRepPasswordForRegistration()
        {
            return Rep_RPasswordBox.Password;
        }

        public void ClearPassword()
        {
            RPasswordBox.Password = string.Empty;
            Rep_RPasswordBox.Password = string.Empty;
        }

        public void SetOpacity(double opacity)
        {
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            LoginText.Focus();
        }

        public void Hide(bool isVisible)
        {
            if (isVisible)
            {
                Visibility = Visibility.Visible;
            }
            else
            {
                Visibility = Visibility.Hidden;
            }
        }
    }
}