using Message.AdditionalItems;
using Message.Compression;
using Message.Interfaces;
using Message.Model;
using Message.UserServiceReference;
using Microsoft.Win32;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using User = Message.UserServiceReference.User;

namespace Message.ViewModel
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    internal class MessageMainVM : BaseViewModel
    {
        private int messageLimit = 50;

        private IMessaging _view;

        private Image _image;

        public Image Images
        {
            get { return _image; }
            set
            {
                _image = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Images"));
            }
        }

        private string _searchContactStr;

        public string SearchContactStr
        {
            get { return _searchContactStr; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    UpdateContactList();
                }
                else
                {
                    ContactsList.Clear();
                    List<UiInfo> temp = UserServiceClient
                        .GetAllContactsUiInfo(GlobalBase.CurrentUser.Id)
                        .Where(i => i.UniqueName.Contains(value))
                        .ToList();

                    ContactsList = temp;

                    GlobalBase.loadPictures(UserServiceClient, ContactsList);
                }

                SetProperty(ref _searchContactStr, value);
            }
        }

        private string _currentUserName;

        public string CurrentUserName
        {
            get { return GlobalBase.CurrentUser.FirstName + " " + GlobalBase.CurrentUser.LastName; }
            set { SetProperty(ref _currentUserName, value); }
        }

        private string _currentUserLogin;

        public string CurrentUserLogin
        {
            get { return "@" + GlobalBase.CurrentUser.Login; }
            set { SetProperty(ref _currentUserLogin, value); }
        }

        private bool _isDialogSearchVisible;

        public bool IsDialogSearchVisible
        {
            get { return _isDialogSearchVisible; }
            set
            {
                SetProperty(ref _isDialogSearchVisible, value);
            }
        }

        private List<UiInfo> _contactsList = new List<UiInfo>();

        public List<UiInfo> ContactsList
        {
            get { return _contactsList; }
            set { SetProperty(ref _contactsList, value); }
        }

        private UiInfo _selectedContact;

        public UiInfo SelectedContact
        {
            get { return _selectedContact; }
            set
            {
                SetProperty(ref _selectedContact, value, () => { SelectedContactChanged(); });
                GlobalBase.SelectedContact = value;
            }
        }

        private string _dialogSearchStr;

        public string DialogSearchStr
        {
            get { return _dialogSearchStr; }
            set
            {
                if (value != "" && IsDialogSearchVisible)
                {
                    IsMenuEnabled = false;
                }
                else
                {
                    IsMenuEnabled = true;
                }

                SetProperty(ref _dialogSearchStr, value);
                ExecuteOnDialogSearch();
            }
        }

        private string _messageText;

        public string MessageText
        {
            get { return _messageText; }
            set { SetProperty(ref _messageText, value); }
        }

        private string[] _filesPath;

        public string[] FilesPath
        {
            get { return _filesPath; }
            set { SetProperty(ref _filesPath, value); }
        }

        private bool _isMenuEnabled = false;

        public bool IsMenuEnabled
        {
            get { return _isMenuEnabled; }
            set
            {
                GlobalBase.IsMenuEnabled = value;
                SetProperty(ref _isMenuEnabled, value);
            }
        }

        private int? _fileAmount;

        public int? FileAmount
        {
            get
            {
                if (_fileAmount == 0)
                {
                    return null;
                }

                return _fileAmount;
            }
            set { SetProperty(ref _fileAmount, value); }
        }

        private DelegateCommand _onContactsCommand;

        public DelegateCommand ContactsCommand =>
            _onContactsCommand ?? (_onContactsCommand = new DelegateCommand(ExecuteOnContacts));

        private DelegateCommand _onSettingsCommand;

        public DelegateCommand SettingsCommand =>
            _onSettingsCommand ?? (_onSettingsCommand = new DelegateCommand(ExecuteOnSettingsCommand));

        private DelegateCommand _dialogSearchCommand;

        public DelegateCommand DialogSearchCommand =>
            _dialogSearchCommand ?? (_dialogSearchCommand = new DelegateCommand(() =>
            {
                IsDialogSearchVisible = !IsDialogSearchVisible;
                if (!IsDialogSearchVisible)
                {
                    SelectedContactChanged();
                }
            }));

        private DelegateCommand _onSendMessage;

        public DelegateCommand OnSendMessage =>
            _onSendMessage ?? (_onSendMessage = new DelegateCommand(ExecuteOnSendMessage));

        private DelegateCommand _onViewProfile;

        public DelegateCommand ViewProfile =>
            _onViewProfile ?? (_onViewProfile = new DelegateCommand(ExecuteOnViewProfile));

        private DelegateCommand _exit;

        public DelegateCommand Exit =>
            _exit ?? (_exit = new DelegateCommand(ExecuteOnExit));

        private DelegateCommand _addFile;

        public DelegateCommand AddFile =>
            _addFile ?? (_addFile = new DelegateCommand(ExecuteOnAddFile));

        private DelegateCommand _createChatGroup;

        public DelegateCommand CreateChatGroup =>
            _createChatGroup ?? (_createChatGroup = new DelegateCommand(ExecuteOnCreateChatGroup));

        public MessageMainVM(IMessaging View, User user) : base()
        {
            _view = View;
            _view.ScrolledToTop += OnScrolledToTop;

            GlobalBase.CurrentUser = user;

            GlobalBase.UpdateMessagesOnUI += () =>
            {
                _view.UpdateMessageList();
            };
            GlobalBase.AddMessageOnUi += AddMessageOnUI;
            GlobalBase.UpdateContactList += UpdateContactList;
            GlobalBase.RemoveMessageOnUI += DeleteMessageOnUI;
            GlobalBase.UpdateProfileUi += SetAvatarForUI;
            GlobalBase.ExitProgramm += ExecuteOnExit;

            SetAvatarForUI();
            UpdateContactList();
            UserServiceClient.OnUserCame(user.Id);

            IsMenuEnabled = false;
            FileAmount = 0;
        }

        //update data using server
        private void UpdateContactList()
        {
            try
            {
                Task.Run(() =>
                    {
                        try
                        {
                            var temp = SelectedContact;

                            var tempUiInfos = UserServiceClient.GetAllContactsUiInfo(GlobalBase.CurrentUser.Id);
                            GlobalBase.loadPictures(UserServiceClient, tempUiInfos);

                            Dispatcher.CurrentDispatcher.Invoke(() =>
                            {
                                lock (GlobalBase.contactsMonitor)
                                {
                                    ContactsList.Clear();
                                    ContactsList = tempUiInfos;
                                }
                            });

                            if (temp != null)
                            {
                                Dispatcher.CurrentDispatcher.Invoke(() =>
                                {
                                    lock (GlobalBase.contactsMonitor)
                                    {
                                        SelectedContact = ContactsList.FirstOrDefault(ct => ct.UniqueName == temp.UniqueName);
                                    }
                                });
                            }
                        }
                        catch (Exception)
                        {

                        }
                    });
            }
            catch (Exception)
            {
            }
        }

        private void SelectedContactChanged(object sender = null, PropertyChangedEventArgs e = null)
        {
            try
            {
                ResetDialogSearchOnUi();

                if (SelectedContact != null)
                {
                    IsMenuEnabled = true;
                }
                else
                {
                    IsMenuEnabled = false;
                }

                if (_view.MessageList != null)
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            lock (_view.MessageList)
                            {
                                _view.MessageList.Clear();
                            }

                            GlobalBase.loadPictures(UserServiceClient, ContactsList);

                            var res = new List<BaseMessage>();

                            if (SelectedContact is UserUiInfo)
                            {
                                res.AddRange(UserServiceClient.GetUserMessages(GlobalBase.CurrentUser.Id,
                                    (SelectedContact as UserUiInfo).UserId, messageLimit));
                            }
                            else if (SelectedContact is ChatGroupUiInfo)
                            {
                                res.AddRange(
                                    UserServiceClient.GetGroupMessages((SelectedContact as ChatGroupUiInfo).ChatGroupId, messageLimit));
                            }

                            if (res.Count != 0)
                            {
                                Application.Current.Dispatcher.Invoke(new Action(() =>
                                {
                                    lock (_view.MessageList)
                                    {
                                        _view.MessageList.AddRange(res);
                                    }
                                }));
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }).ContinueWith((task =>
                    {
                        GlobalBase.UpdateMessagesOnUI();
                    }));
                }
            }
            catch (Exception)
            {

            }
        }

        private void UpdateMessage(BaseMessage message, Func<BaseMessage, bool> UiUpdateStrategy)
        {
            try
            {
                var mes = "New message from  @";
                var sender = UserServiceClient.GetUserById(message.SenderId);

                if (message is GroupMessage gMes)
                {
                    if (SelectedContact is ChatGroupUiInfo)
                    {
                        if (gMes.ChatGroupId != (SelectedContact as ChatGroupUiInfo).ChatGroupId)
                        {
                            gMes.SenderName = sender.FirstName;
                            ChatGroup chatGroup = UserServiceClient.GetChatGroupById(gMes.ChatGroupId);

                            mes += chatGroup.Name;
                            mes += " group \n";
                            mes += "\"" + GlobalBase.Base64Decode(message.Text) + "\"";
                            GlobalBase.ShowNotify("New message", mes);
                        }
                        else
                        {
                            UiUpdateStrategy(message);
                        }
                    }
                }
                else if (message is UserMessage uMes)
                {
                    if (SelectedContact is UserUiInfo)
                    {
                        if (uMes.SenderId != (SelectedContact as UserUiInfo).UserId)
                        {
                            mes += sender.FirstName + sender.LastName + "\n";
                            mes += "\"" + GlobalBase.Base64Decode(message.Text) + "\"";
                            GlobalBase.ShowNotify("New message", mes);
                        }
                        else
                        {
                            UiUpdateStrategy(message);
                        }
                    }
                }

                Debug.WriteLine("Receave Message from - ", sender.Login);
            }
            catch (Exception)
            {

            }
        }

        private void OnScrolledToTop()
        {
            //TODO
        }

        //UI update
        private bool AddMessageOnUI(BaseMessage message)
        {
            try
            {
                Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                lock (_view.MessageList)
                {
                    _view.MessageList.Add(message);
                }
            });

                _view.UpdateMessageList();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private bool UppdateTextOfMessageOnUI(BaseMessage message)
        {
            try
            {
                var mes = _view.MessageList.FirstOrDefault(x => x.Id == message.Id);
                if (mes != null)
                {
                    mes.Text = message.Text;
                    _view.UpdateMessageList();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }

        }

        private bool DeleteMessageOnUI(BaseMessage message)
        {
            try
            {
                var mes = _view.MessageList.FirstOrDefault(x => x.Id == message.Id);
                if (mes != null)
                {
                    _view.MessageList.Remove(mes);
                    GlobalBase.UpdateMessagesOnUI.Invoke();
                    return true;
                }

                return false;
            }
            catch (Exception)
            {

                return false;
            }
        }

        private void SetAvatarForUI()
        {
            Task.Run(() =>
            {
                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    try
                    {
                        Images = GlobalBase.getUsersAvatar(GlobalBase.CurrentUser);
                    }
                    catch (Exception)
                    {

                    }
                });
            });
        }

        private void ResetDialogSearchOnUi()
        {
            try
            {
                IsDialogSearchVisible = false;
            }
            catch (Exception)
            {

            }
        }

        //executes for commands
        private void ExecuteOnAddFile()
        {
            try
            {
                var openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = true;
                openFileDialog.ShowDialog();
                string[] tempFileNames = openFileDialog.FileNames;

                foreach (string fileName in tempFileNames)
                {
                    FileInfo fileInfo = new FileInfo(fileName);
                    if (fileInfo.Length > GlobalBase.fileSizeConstraint)
                    {
                        CustomMessageBox.Show(Translations.GetTranslation()["FileSizeExcesedShort"].ToString(), Translations.GetTranslation()["FileSizeExcesed"].ToString(), MessageBoxType.Warning);
                        return;
                    }
                }

                FilesPath = tempFileNames;
                FileAmount = FilesPath.Count();
            }
            catch (Exception)
            {

            }
        }

        private void ExecuteOnExit()
        {
            try
            {
                UserServiceClient.OnUserLeave(GlobalBase.CurrentUser.Id);
                _view.CloseWindow();
            }
            catch (Exception)
            {

            }
        }

        private void ExecuteOnViewProfile()
        {
            try
            {
                Window wnd = null;

                if (SelectedContact is UserUiInfo userUiInfo)
                {
                    var user = UserServiceClient.GetUserById(userUiInfo.UserId);
                    wnd = new ContactProfileWindow(user);
                }
                else if (SelectedContact is ChatGroupUiInfo chatGroupUiInfo)
                {
                    wnd = new EditGroupWindow(chatGroupUiInfo);
                }

                wnd.Owner = (Window)_view;
                wnd.Show();
            }
            catch (Exception)
            {

            }
        }

        private void ExecuteOnSendMessage()
        {
            try
            {
                if (SelectedContact != null && (!string.IsNullOrWhiteSpace(MessageText) || FilesPath != null))
                {
                    BaseMessage messageTemplate = null;

                    if (SelectedContact is UserUiInfo)
                    {
                        messageTemplate = new UserMessage();
                    }
                    else if (SelectedContact is ChatGroupUiInfo)
                    {
                        messageTemplate = new GroupMessage();
                    }

                    messageTemplate.DateOfSending = DateTime.Now;
                    messageTemplate.SenderId = GlobalBase.CurrentUser.Id;
                    messageTemplate.Sender = GlobalBase.CurrentUser;

                    if (SelectedContact is UserUiInfo)
                    {
                        (messageTemplate as UserMessage).ReceiverId = (SelectedContact as UserUiInfo).UserId;
                    }
                    else if (SelectedContact is ChatGroupUiInfo)
                    {
                        (messageTemplate as GroupMessage).ChatGroupId = (SelectedContact as ChatGroupUiInfo).ChatGroupId;
                    }

                    if (MessageText != null && MessageText != string.Empty)
                    {
                        messageTemplate.Text = Encoding.UTF8.GetBytes(MessageText);
                        UserServiceClient.SendMessageAsync(messageTemplate).ContinueWith(task => GlobalBase.AddMessageOnUi(UserServiceClient.GetLastMessage()));
                    }

                    if (FilesPath != null)
                    {
                        foreach (var file in FilesPath)
                        {
                            var chatFile = new FileService.ChatFile() { Source = CompressionHelper.CompressFile(GlobalBase.FileToByte(file)), Name = GlobalBase.GetShortName(file) };

                            messageTemplate.Text = Encoding.UTF8.GetBytes(chatFile.Name);
                            messageTemplate.ChatFileId = GlobalBase.FileServiceClient.UploadFile(chatFile);

                            UserServiceClient.SendMessageAsync(messageTemplate).ContinueWith(task => GlobalBase.AddMessageOnUi(UserServiceClient.GetLastMessage()));
                        }
                    }

                    FilesPath = null;
                    FileAmount = 0;

                    MessageText = string.Empty;

                    Debug.WriteLine("Send Message");
                }
            }
            catch (Exception)
            {

            }
        }

        private void ExecuteOnSettingsCommand()
        {
            try
            {
                _view.SetOpacity(0.5);

                var wnd = new SettingsWindow();
                wnd.Owner = (Window)_view;
                wnd.ShowDialog();

                UpdateContactList();

                _view.SetOpacity(1);
            }
            catch (Exception)
            {

            }
        }

        private void ExecuteOnContacts()
        {
            try
            {
                _view.SetOpacity(0.5);

                var wnd = new Contacts();
                wnd.Owner = (Window)_view;
                wnd.ShowDialog();

                _view.SetOpacity(1);

                UpdateContactList();
            }
            catch (Exception)
            {

            }
        }

        private void ExecuteOnCreateChatGroup()
        {
            try
            {
                _view.SetOpacity(0.5);

                var wnd = new CreateChatGroupWnd();
                wnd.Owner = (Window)_view;
                wnd.ShowDialog();

                _view.SetOpacity(1);

                UpdateContactList();
            }
            catch (Exception)
            {

            }
        }

        private void ExecuteOnDialogSearch()
        {
            try
            {
                Task.Run(() =>
                {
                    try
                    {
                        if (IsDialogSearchVisible)
                        {
                            if (SelectedContact != null)
                            {
                                _view.MessageList.Clear();
                                var res = new List<BaseMessage>();

                                if (SelectedContact is UserUiInfo)
                                {
                                    var userUiInfo = SelectedContact as UserUiInfo;
                                    res.AddRange(UserServiceClient.GetUserMessages(GlobalBase.CurrentUser.Id, userUiInfo.UserId, messageLimit));
                                }
                                else if (SelectedContact is ChatGroupUiInfo)
                                {
                                    var chatGroupUiInfo = SelectedContact as ChatGroupUiInfo;
                                    res.AddRange(UserServiceClient.GetGroupMessages(chatGroupUiInfo.ChatGroupId, messageLimit));
                                }

                                if (res.Count != 0)
                                {
                                    foreach (var mes in res)
                                    {
                                        if (GlobalBase.Base64Decode(mes.Text).Contains(DialogSearchStr))
                                        {
                                            GlobalBase.AddMessageOnUi.Invoke(mes);
                                        }
                                    }
                                }

                                GlobalBase.UpdateMessagesOnUI();
                            }
                        }
                        else
                        {
                            SelectedContactChanged();
                        }
                    }
                    catch (Exception)
                    {

                    }
                });
            }
            catch (Exception)
            {

            }
        }

        //service callbacks
        public override void ReceiveMessage(BaseMessage message)
        {
            try
            {
                UpdateMessage(message, AddMessageOnUI);
            }
            catch (Exception)
            {

            }
        }

        public override void UserLeave(User user)
        {
            try
            {
                UpdateContactList();
                Debug.WriteLine("Works(Leave) - " + user.FirstName + " - (currentUser - " + GlobalBase.CurrentUser.FirstName + ")");
            }
            catch (Exception)
            {

            }
        }

        public override void UserCame(User user)
        {
            try
            {
                UpdateContactList();
                Debug.WriteLine("Works(Came) - " + user.FirstName + " - (currentUser - " + GlobalBase.CurrentUser.FirstName + ")");
            }
            catch (Exception)
            {

            }
        }

        public override void OnMessageRemoved(BaseMessage message)
        {
            try
            {
                DeleteMessageOnUI(message);
            }
            catch (Exception)
            {

            }
        }

        public override void OnMessageEdited(BaseMessage message)
        {
            try
            {
                UpdateMessage(message, UppdateTextOfMessageOnUI);
            }
            catch (Exception)
            {

            }
        }

        public override void OnNewContactAdded(UiInfo newContact)
        {
            try
            {
                var foundedUiInfo = ContactsList.FirstOrDefault(c => c.UniqueName == newContact.UniqueName);

                if (foundedUiInfo == null)
                {
                    var temp = new List<UiInfo>();
                    temp.AddRange(ContactsList);

                    GlobalBase.loadPicture(UserServiceClient, newContact);
                    temp.Add(newContact);

                    ContactsList = temp;
                }
            }
            catch (Exception)
            {

            }
        }

        public override void OnContactRemoved(UiInfo newContact)
        {
            try
            {
                var foundedUiInfo = ContactsList.FirstOrDefault(c => c.UniqueName == newContact.UniqueName);

                if (foundedUiInfo != null)
                {
                    var temp = new List<UiInfo>();
                    temp.AddRange(ContactsList);

                    ContactsList.Remove(foundedUiInfo);
                    ContactsList = temp;
                }
            }
            catch (Exception)
            {

            }
        }

        public override void OnEntityChanged(UiInfo changedEntity)
        {
            try
            {
                if (changedEntity.UniqueName == GlobalBase.CurrentUser.Login)
                {
                    GlobalBase.UpdateProfileUi.Invoke();
                }
                else
                {
                    foreach (var uiInfo in ContactsList)
                    {
                        if (uiInfo.UniqueName == changedEntity.UniqueName)
                        {
                            GlobalBase.UpdateContactList();
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }
    }
}