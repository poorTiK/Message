using System.Windows;

namespace Message.AdditionalItems
{
    /// <summary>
    /// Interaction logic for CustomMessageBox.xaml
    /// </summary>
    ///

    public enum MessageBoxType
    {
        ConfirmationWithYesNo = 0,
        ConfirmationWithYesNoCancel,
        Information,
        Error,
        Warning
    }

    public partial class CustomMessageBox : Window
    {
        public CustomMessageBox()
        {
            InitializeComponent();

            CloseButton.Click += (s, e) => Close();
        }

        private static CustomMessageBox _messageBox;
        private static MessageBoxResult _result = MessageBoxResult.No;

        public static MessageBoxResult Show(string caption, string msg, MessageBoxType type)
        {
            switch (type)
            {
                case MessageBoxType.ConfirmationWithYesNo:
                    return Show(caption, msg, MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                case MessageBoxType.ConfirmationWithYesNoCancel:
                    return Show(caption, msg, MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);

                case MessageBoxType.Information:
                    return Show(caption, msg, MessageBoxButton.OK,
                    MessageBoxImage.Information);

                case MessageBoxType.Error:
                    return Show(caption, msg, MessageBoxButton.OK,
                    MessageBoxImage.Error);

                case MessageBoxType.Warning:
                    return Show(caption, msg, MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                default:
                    return MessageBoxResult.No;
            }
        }

        public static MessageBoxResult Show(string msg, MessageBoxType type)
        {
            return Show(string.Empty, msg, type);
        }

        public static MessageBoxResult Show(string msg)
        {
            return Show(string.Empty, msg,
            MessageBoxButton.OK, MessageBoxImage.None);
        }

        public static MessageBoxResult Show
        (string caption, string text)
        {
            return Show(caption, text,
            MessageBoxButton.OK, MessageBoxImage.None);
        }

        public static MessageBoxResult Show
        (string caption, string text, MessageBoxButton button)
        {
            return Show(caption, text, button,
            MessageBoxImage.None);
        }

        public static MessageBoxResult Show
        (string caption, string text,
        MessageBoxButton button, MessageBoxImage image)
        {
            _messageBox = new CustomMessageBox
            { txtMsg = { Text = text }, MessageTitle = { Text = caption } };
            SetVisibilityOfButtons(button);
            _messageBox.ShowDialog();
            return _result;
        }

        private static void SetVisibilityOfButtons(MessageBoxButton button)
        {
            switch (button)
            {
                case MessageBoxButton.OK:
                    _messageBox.Cancel.Visibility = Visibility.Collapsed;
                    _messageBox.No.Visibility = Visibility.Collapsed;
                    _messageBox.Yes.Visibility = Visibility.Collapsed;
                    _messageBox.Ok.Focus();
                    break;

                case MessageBoxButton.OKCancel:
                    _messageBox.No.Visibility = Visibility.Collapsed;
                    _messageBox.Yes.Visibility = Visibility.Collapsed;
                    _messageBox.Cancel.Focus();
                    break;

                case MessageBoxButton.YesNo:
                    _messageBox.Ok.Visibility = Visibility.Collapsed;
                    _messageBox.Cancel.Visibility = Visibility.Collapsed;
                    _messageBox.No.Focus();
                    break;

                case MessageBoxButton.YesNoCancel:
                    _messageBox.Ok.Visibility = Visibility.Collapsed;
                    _messageBox.Cancel.Focus();
                    break;

                default:
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == Ok)
            {
                _result = MessageBoxResult.OK;
            }
            else if (sender == Yes)
            {
                _result = MessageBoxResult.Yes;
            }
            else if (sender == No)
            {
                _result = MessageBoxResult.No;
            }
            else if (sender == Cancel)
            {
                _result = MessageBoxResult.Cancel;
            }
            else
            {
                _result = MessageBoxResult.None;
            }

            _messageBox.Close();
            _messageBox = null;
        }
    }
}