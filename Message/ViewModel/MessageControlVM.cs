using Message.AdditionalItems;
using Message.Model;
using Message.UserServiceReference;
using Prism.Commands;
using System;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Message.PhotoServiceReference;
using Application = System.Windows.Application;
using Clipboard = System.Windows.Clipboard;
using System.IO;

namespace Message.ViewModel
{
    internal class MessageControlVM : BaseViewModel
    {
        private BaseMessage Message { get; set; }

        private string MessageText
        {
            get
            {
                return GetContent();
                //return GlobalBase.Base64Decode(Message.Content);
            }
            set { }
        }

        private string GetContent()
        {
            switch (Message.Type)
            {
                case "TEXT":
                    return GlobalBase.Base64Decode(Message.Content);
                case "DATA":
                    //
                    break;
            }

            return string.Empty;
        }

        public MessageControlVM(BaseMessage message) : base()
        {
            Message = message;
        }

        private DelegateCommand _onCopy;

        public DelegateCommand Copy =>
            _onCopy ?? (_onCopy = new DelegateCommand(OnCopy));

        private DelegateCommand _onDelete;

        public DelegateCommand Delete =>
            _onDelete ?? (_onDelete = new DelegateCommand(OnDelete));

        private DelegateCommand _onEdit;

        public DelegateCommand Edit =>
            _onEdit ?? (_onEdit = new DelegateCommand(OnEdit));

        private DelegateCommand _onForward;

        public DelegateCommand Forward =>
            _onForward ?? (_onForward = new DelegateCommand(OnForward));

        private DelegateCommand _downloadData;

        public DelegateCommand DownloadData =>
            _downloadData ?? (_downloadData = new DelegateCommand(OnDownloadData));

        private void OnDownloadData()
        {
            var file = GlobalBase.PhotoServiceClient.GetFileByMessageId(Message.Id);
            //TODO change to SaveFileDialog
            var fileDialog = new FolderBrowserDialog();
            var res = fileDialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                var savePath = fileDialog.SelectedPath;
                if (!string.IsNullOrEmpty(savePath))
                {
                    using (Stream fileStr = File.OpenWrite(savePath + "\\" + Message.AdditionalInfo))
                    {
                        fileStr.Write(file, 0, file.Length);

                        //File.Open(savePath + Message.AdditionalInfo, FileMode.Open);
                    }
                }
            }
        }

        private void OnForward()
        {
            var wnd = new ForwardMessageWindow(Message);
            wnd.ShowDialog();

            GlobalBase.UpdateUI.Invoke();
        }

        private void OnEdit()
        {
            var wnd = new MessageEditWindow(Message);
            wnd.ShowDialog();

            GlobalBase.UpdateUI.Invoke();
        }

        private void OnDelete()
        {
            var canDelete = CustomMessageBox.Show("", Application.Current.Resources.MergedDictionaries[4]["AskForDelMessage"].ToString(), MessageBoxButton.YesNo);

            switch (canDelete)
            {
                case MessageBoxResult.Yes:
                    UserServiceClient.RemoveMessage(Message);

                    GlobalBase.UpdateUI.Invoke();
                    break;

                case MessageBoxResult.No:
                    break;
            }
        }

        private void OnCopy()
        {
            Clipboard.SetText(MessageText);
        }
    }
}