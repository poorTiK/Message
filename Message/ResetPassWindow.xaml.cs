using Message.AdditionalItems;
using Message.Interfaces;
using Message.ViewModel;
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
using System.Windows.Shapes;

namespace Message
{
    /// <summary>
    /// Interaction logic for ResetPassWindow.xaml
    /// </summary>
    public partial class ResetPassWindow : Window, IView, IRestorePasswordSupplier
    {
        public ResetPassWindow()
        {
            InitializeComponent();

            DwmDropShadow.DropShadowToWindow(this);

            DataContext = new ResetPassWindowVM(this, this);
        }

        public void AnimatedResize(int h, int w)
        {
        }

        public void CloseWindow()
        {
            Close();
        }

        public void Hide(bool isVisible)
        {
        }

        public void SetOpacity(double opasity)
        {
        }

        public string GetCurrentPassword()
        {
            return PasswordBox.Password;
        }

        public string GetNewPassword()
        {
            return NewPasswordBox.Password;
        }
    }
}