using Message.Interfaces;
using Message.Model;
using Message.UserServiceReference;
using Microsoft.Win32;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using Message.PhotoServiceReference;

namespace Message.ViewModel
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    internal class MessageMainVM : BaseViewModel
    {

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


        public User CurrentUser { get; set; }
       //private byte[] _currentUserPhoto;

       // public byte[] CurrentUserPhoto
       // {
       //     get { return GlobalBase.CurrentUser.Avatar; }
       //     set { SetProperty(ref _currentUserPhoto, value); }
       // }
        private string _searchContactStr;

        public string SearchContactStr 
        {
            get { return _searchContactStr; }
            set
            {


                if (value == string.Empty)
                {
                    ContactsList.Clear();
                    ContactsList.AddRange(UserServiceClient.GetAllContactsUiInfo(GlobalBase.CurrentUser.Id));

                        foreach (var item in ContactsList)
                        {
                            if (item is UserUiInfo)
                            {
                                UserUiInfo userUiInfo = item as UserUiInfo;
                                item.Avatar = GlobalBase.PhotoServiceClient.GetPhotoById(userUiInfo.UserId);
                            }
                        }

                }
                else
                {
                    ContactsList.Clear();
                    ContactsList.AddRange(UserServiceClient
                        .GetAllContacts(GlobalBase.CurrentUser.Id)
                        .Where(i => i.FirstName.Contains(value) 
                        || i.LastName.Contains(value) 
                        || i.Login.Contains(value))
                                  .Select(u => new UserUiInfo {
                                      Name = u.FirstName + " " + u.LastName,
                                      UniqueName =  u.Login,
                                      UserId = u.Id, Avatar = u.Avatar,
                                      Status = u.Status}) );


                        foreach (var item in ContactsList)
                        {
                            UserUiInfo userUiInfo = item as UserUiInfo;
                            item.Avatar = GlobalBase.PhotoServiceClient.GetPhotoById(userUiInfo.UserId);
                        }
                    }

                SetProperty(ref _searchContactStr, value);
            }
        }
        private string _currentUserName;

        public string CurrentUserName
        {
            get { return CurrentUser.FirstName + " " + CurrentUser.LastName; }
            set { SetProperty(ref _currentUserName, value); }
        }

        private string _currentUserLogin;

        public string CurrentUserLogin
        {
            get { return "@" + CurrentUser.Login; }
            set { SetProperty(ref _currentUserLogin, value); }
        }

        private bool _isDialogSearchVisible;

        public bool IsDialogSearchVisible
        {
            get { return _isDialogSearchVisible; }
            set { SetProperty(ref _isDialogSearchVisible, value); }
        }

        private List<UiInfo> _contactsList;

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
                SetProperty(ref _selectedContact, value, () => {SelectedContactChanged();});
            }
        }

        private string _dialogSearchStr;

        public string DialogSearchStr
        {
            get { return _dialogSearchStr; }
            set
            {
                SetProperty(ref _dialogSearchStr, value);
                OnDialogSearch();
            }
        }

        private string _messageText;

        public string MessageText
        {
            get { return _messageText; }
            set { SetProperty(ref _messageText, value); }
        }

        private string _filePath;

        public string FilePath
        {
            get { return _filePath; }
            set { SetProperty(ref _filePath, value); }
        }

        private bool _isMenuEnabled;

        public bool IsMenuEnabled
        {
            get { return _isMenuEnabled; }
            set { SetProperty(ref _isMenuEnabled, value); }
        }

        public MessageMainVM(IMessaging View, User user) : base()
        {
            _view = View;

            CurrentUser = user;
            GlobalBase.CurrentUser = user;
            GlobalBase.UpdateUI += () =>
            {
                Update();
            };

            SetAvatarForUI();

            ContactsList = UserServiceClient.GetAllContactsUiInfo(GlobalBase.CurrentUser.Id);
            foreach (var item in ContactsList)
            {
                if (item.Avatar != null)
                {
                    MemoryStream memstr = new MemoryStream(item.Avatar);
                    Dispatcher.CurrentDispatcher.Invoke(() => { item.Images = Image.FromStream(memstr); });
                    ContactsList = ContactsList.ToList();
                }
            }
            SelectedContact = ContactsList.FirstOrDefault();
            IsMenuEnabled = true;

            UserServiceClient.OnUserCame(user.Id);
        }

        private void SelectedContactChanged(object sender = null, PropertyChangedEventArgs e = null)
        {
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
                _view.MessageList.Clear();
             
                if (SelectedContact is UserUiInfo) {
                    UserUiInfo userUiInfo = SelectedContact as UserUiInfo;
                    User user = UserServiceClient.GetUserById(userUiInfo.UserId);
                    List<UserMessage> res = null; /* userServiceClient.GetUserMessages(GlobalBase.CurrentUser.Id, user.Id, 50);*/

                    res = UserServiceClient.GetUserMessages(GlobalBase.CurrentUser.Id, (SelectedContact as UserUiInfo).UserId, 50);

                    if (res != null)
                    {
                        foreach (var mes in res)
                        {
                            _view.MessageList.Add(new UserMessage()
                            {
                                Id = mes.Id,
                                Content = mes.Content,
                                DateOfSending = mes.DateOfSending,
                                ReceiverId = mes.ReceiverId,
                                SenderId = mes.SenderId,
                                Type = mes.Type
                            });
                        }
                        _view.UpdateMessageList();
                    }
                }
            }
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

        private void ExecuteOnAddFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            FilePath = openFileDialog.FileName;
        }

        private void ExecuteOnExit()
        {
           UserServiceClient.OnUserLeave(GlobalBase.CurrentUser.Id);
            _view.CloseWindow();
        }

        private void ExecuteOnViewProfile()
        {
            if (SelectedContact is UserUiInfo) {
                UserUiInfo userUiInfo = SelectedContact as UserUiInfo;
                User user = UserServiceClient.GetUserById(userUiInfo.UserId);
                var wnd = new ContactProfileWindow(user);
                wnd.Owner = (Window)_view;
                wnd.ShowDialog();

                Update();
            }
        }

        private void ExecuteOnSendMessage()
        {
            if (SelectedContact != null && !string.IsNullOrWhiteSpace(MessageText) && SelectedContact is UserUiInfo)
            {
                UserUiInfo userUiInfo = SelectedContact as UserUiInfo;

                var message = new UserMessage()
                {
                    Content = Encoding.UTF8.GetBytes(MessageText),
                    DateOfSending = DateTime.Now,
                    SenderId = GlobalBase.CurrentUser.Id,
                    ReceiverId = userUiInfo.UserId,
                    Type = "TEXT",
                };

                    UserServiceClient.SendMessageAsync(message);


                Debug.WriteLine("Send Message");
                _view.MessageList.Add(message);

                _view.UpdateMessageList();

                MessageText = string.Empty;
            }
        }

        private void SetAvatarForUI()
        {
            if (GlobalBase.CurrentUser?.Avatar?.Length > 0)
            {
                MemoryStream memstr = new MemoryStream(GlobalBase.CurrentUser.Avatar);
                Dispatcher.CurrentDispatcher.Invoke(() => { Images = Image.FromStream(memstr); });
            }
            else
            {
                Dispatcher.CurrentDispatcher.Invoke(() => { Images = null; });
            }
        }

        private void ExecuteOnSettingsCommand()
        {
            _view.SetOpacity(0.5);

            var wnd = new SettingsWindow();
            wnd.Owner = (Window)_view;
            wnd.ShowDialog();

            Update();

            _view.SetOpacity(1);
        }

        private void ExecuteOnContacts()
        {
            _view.SetOpacity(0.5);

            var wnd = new Contacts();
            wnd.Owner = (Window)_view;
            wnd.ShowDialog();

            _view.SetOpacity(1);

            Update();
        }

        private void OnDialogSearch()
        {
            if (IsDialogSearchVisible)
            {
                if (SelectedContact != null && SelectedContact is UserUiInfo)
                {
                    _view.MessageList.Clear();
                    List<UserMessage> res;
                    UserUiInfo userUiInfo = null;

                         userUiInfo  = SelectedContact as UserUiInfo;
                         res = UserServiceClient.GetUserMessages(GlobalBase.CurrentUser.Id, userUiInfo.UserId, 50);

                    if (res != null)
                    {
                        foreach (var mes in res)
                        {
                            var message = new UserMessage()
                            {
                                Content = mes.Content,
                                DateOfSending = mes.DateOfSending,
                                ReceiverId = mes.ReceiverId,
                                SenderId = mes.SenderId,
                                Type = mes.Type
                            };
                            if (GlobalBase.Base64Decode(mes.Content).Contains(DialogSearchStr))
                            {
                                _view.MessageList.Add(message);
                            }
                        }
                        _view.UpdateMessageList();
                    }
                }
            }
            else
            {
                SelectedContactChanged();
            }
        }

        public void Update()
        {
            //make all update modular and put here plz
            UpdateContactList();
            SetAvatarForUI();
        }

        public void UpdateContactList()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                UserUiInfo temp = null;
                if (SelectedContact is UserUiInfo)
                {
                    temp = SelectedContact as UserUiInfo;
                }


                ContactsList = UserServiceClient.GetAllContactsUiInfo(GlobalBase.CurrentUser.Id);


                    foreach (var item in ContactsList)
                    {
                        if (SelectedContact is UserUiInfo)
                        {
                            UserUiInfo userUiInfo = SelectedContact as UserUiInfo;
                            item.Avatar = GlobalBase.PhotoServiceClient.GetPhotoById(userUiInfo.UserId);
                        }
                    }
               

                if (ContactsList.Any(x => temp != null && ((x as UserUiInfo).UserId == temp.UserId)))
                {
                    SelectedContact = temp;
                }
            }));
        }

        public override void ReceiveMessage(BaseMessage message)
        {
            User user = UserServiceClient.GetAllUsers().FirstOrDefault(x => x.Id == message.SenderId);

            var mes = "New message from  @" + user.Login + "\n" + "\"" + GlobalBase.Base64Decode(message.Content) +
                      "\"";
            GlobalBase.ShowNotify("New message", mes);

            Debug.WriteLine("Receave Message from - ", user.Login);

            if (!ContactsList.Contains(ContactsList.FirstOrDefault(x => (x as UserUiInfo).UserId == user.Id)))
            {
                UserServiceClient.AddContactAsync(GlobalBase.CurrentUser.Id, user.Id).ContinueWith(task =>
                {
                    UpdateContactList();
                });
            }
            else if ((SelectedContact as UserUiInfo).UserId == user.Id)
            {
                SelectedContactChanged();
            }
            else if ((SelectedContact as UserUiInfo).UserId == user.Id)
            {
                SelectedContactChanged();
            }
        }

        public override void UserLeave(User user)
        {
            UpdateContactList();
            Debug.WriteLine("Works(Leave) - " + user.FirstName + " - (currentUser - " + GlobalBase.CurrentUser.FirstName + ")");
        }

        public override void UserCame(User user)
        {
            UpdateContactList();
            Debug.WriteLine("Works(Came) - " + user.FirstName + " - (currentUser - " + GlobalBase.CurrentUser.FirstName + ")");
        }

        public override void OnMessageRemoved(BaseMessage message)
        {
            //throw new NotImplementedException();
        }

        public override void OnMessageEdited(BaseMessage message)
        {
            //throw new NotImplementedException();
        }
    }
}