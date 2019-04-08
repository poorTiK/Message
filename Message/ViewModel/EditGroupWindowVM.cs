using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using Message.AdditionalItems;
using Message.Compression;
using Message.Interfaces;
using Message.Model;
using Message.UserServiceReference;
using Prism.Commands;
using Application = System.Windows.Application;

namespace Message.ViewModel
{
    internal class EditGroupWindowVM : BaseViewModel
    {
        private IView _view;
        private ChatGroupUiInfo _groupUiInfo;
        private ChatGroup _group;
        private List<UiInfo> _membersToAdd;

        private byte[] _newAvatar;
        private Image _image;

        public Image Images
        {
            get { return _image; }
            set { _image = value; OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Images")); }
        }

        private string _currentGroupName;

        public string CurrentGroupName
        {
            get { return _currentGroupName; }
            set { SetProperty(ref _currentGroupName, value); }
        }

        private string _groupName;

        public string GroupName
        {
            get { return _groupName; }
            set
            {
                SetProperty(ref _groupName, value);
                CheckChanges();
            }
        }

        private string groupMembersAmout;

        public string GroupMembersAmout
        {
            get { return groupMembersAmout; }
            set { SetProperty(ref groupMembersAmout, value); }
        }

        private List<UiInfo> _groupMemberList;

        public List<UiInfo> GroupMemberList
        {
            get { return _groupMemberList; }
            set { SetProperty(ref _groupMemberList, value); }
        }

        private UiInfo _selectedMember;

        public UiInfo SelectedMember
        {
            get { return _selectedMember; }
            set { SetProperty(ref _selectedMember, value); }
        }

        private bool _isNewChanges;

        public bool IsNewChanges
        {
            get { return _isNewChanges; }
            set { SetProperty(ref _isNewChanges, value); }
        }

        private bool _isSavingProgress;

        public bool IsSavingProgress
        {
            get { return _isSavingProgress; }
            set { SetProperty(ref _isSavingProgress, value); }
        }

        public EditGroupWindowVM(IView view, ChatGroupUiInfo group)
        {
            _view = view;
            _groupUiInfo = group;
            _membersToAdd = new List<UiInfo>();

            Init();

            IsNewChanges = IsSavingProgress = false;
        }

        private void SetAvatarForUI()
        {
            Task.Run(() =>
            {
                var chatFile = GlobalBase.FileServiceClient.getChatFileById(_group.ImageId);

                if (chatFile?.Source?.Length > 0)
                {
                    var memstr = new MemoryStream(chatFile.Source);
                    Dispatcher.CurrentDispatcher.Invoke(() => { Images = Image.FromStream(memstr); });
                }
                else
                {
                    Dispatcher.CurrentDispatcher.Invoke(() => { Images = ImageHelper.GetDefGroupImage(); });
                }
            });
        }

        public void Init()
        {
            var tempUiInfos = new List<UiInfo>();
            Task.Run((() =>
            {
                _group = UserServiceClient.GetChatGroup(_groupUiInfo.UniqueName);

                tempUiInfos = UserServiceClient.GetGroupParticipants(_group.Id);
                var defImage = Image.FromFile("../../Resources/DefaultPicture.jpg");

                foreach (var item in tempUiInfos)
                {
                    if (item is UserUiInfo)
                    {
                        var userUiInfo = item as UserUiInfo;
                        var user = UserServiceClient.GetUserById(userUiInfo.UserId);
                        var chatFile = GlobalBase.FileServiceClient.getChatFileById(user.Id);

                        if (chatFile?.Source != null && chatFile?.Source?.Length != 0)
                        {
                            var memstr = new MemoryStream(chatFile.Source);
                            Dispatcher.CurrentDispatcher.Invoke(() => { item.UiImage = Image.FromStream(memstr); });
                        }
                        else
                        {
                            Dispatcher.CurrentDispatcher.Invoke(() => { item.UiImage = defImage; });
                        }
                    }
                }
            })).ContinueWith(task =>
            {
                GroupMemberList = tempUiInfos;

                GroupName = _group.Name;
                CurrentGroupName = _group.Name;
                GroupMembersAmout = GroupMemberList.Count.ToString();

                SetAvatarForUI();
            });
        }

        private DelegateCommand _onApply;

        public DelegateCommand ApplyChanges =>
            _onApply ?? (_onApply = new DelegateCommand(ExecuteOnApplyChanges));

        private DelegateCommand _onBack;

        public DelegateCommand Back =>
            _onBack ?? (_onBack = new DelegateCommand(ExecuteOnBack));

        private DelegateCommand _openProfile;

        public DelegateCommand OpenProfile =>
            _openProfile ?? (_openProfile = new DelegateCommand(ExecuteOnOpenProfile));

        private DelegateCommand _loadPhoto;

        public DelegateCommand LoadPhoto =>
            _loadPhoto ?? (_loadPhoto = new DelegateCommand(ExecuteOnLoadPhoto));

        private DelegateCommand _addMembers;

        public DelegateCommand AddMembers =>
            _addMembers ?? (_addMembers = new DelegateCommand(ExecuteOnAddMembers));

        private void ExecuteOnApplyChanges()
        {
            if (Validate())
            {
                IsSavingProgress = true;
                var res = string.Empty;

                Task.Run(() =>
                {
                    _group.Name = GroupName;

                    var chatFile = GlobalBase.FileServiceClient.getChatFileById(_group.ImageId);

                    if (_newAvatar != null)
                    {
                        if (chatFile == null)
                        {
                            _group.ImageId = GlobalBase.FileServiceClient.UploadFile(new FileService.ChatFile() { Source = CompressionHelper.CompressImage(_newAvatar) });
                        }
                        else
                        {
                            GlobalBase.FileServiceClient.UpdateFileSource(chatFile.Id, CompressionHelper.CompressFile(_newAvatar));
                        }
                    }

                    if (_membersToAdd?.Count != 0)
                    {
                        foreach (var uiInfo in _membersToAdd)
                        {
                            UserServiceClient.AddUserToChatGroupContact(_group.Id, (uiInfo as UserUiInfo).UserId);
                        }
                    }

                    res = UserServiceClient.AddOrUpdateChatGroup(_group);

                    SetAvatarForUI();

                    if (res == string.Empty)
                    {
                        Application.Current.Dispatcher.Invoke(new Action((() =>
                    {
                        CustomMessageBox.Show(Translations.GetTranslation()["ChangesSaved"].ToString());
                    })));
                    }
                }).ContinueWith(task =>
                {
                    IsSavingProgress = false;
                    IsNewChanges = false;

                    GlobalBase.UpdateContactList();
                });
            }
        }

        private void ExecuteOnBack()
        {
            _view.CloseWindow();
        }

        private void ExecuteOnOpenProfile()
        {
            if (SelectedMember is UserUiInfo userUiInfo)
            {
                var user = UserServiceClient.GetUserById(userUiInfo.UserId);
                var wnd = new ContactProfileWindow(user);
                wnd.Owner = (Window)_view;
                wnd.ShowDialog();
            }
        }

        private void ExecuteOnLoadPhoto()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            var FilePath = openFileDialog.FileName;

            if (FilePath != string.Empty)
            {
                _newAvatar = File.ReadAllBytes(FilePath);
                var memstr = new MemoryStream(_newAvatar);
                Dispatcher.CurrentDispatcher.Invoke(() => { Images = Image.FromStream(memstr); });
                IsNewChanges = true;
            }
        }

        private void ExecuteOnAddMembers()
        {
            _view.Hide(false);
            var wnd = new SelectUsersWindow(_group);
            wnd.Owner = (Window)_view;
            wnd.ShowDialog();
            _view.Hide(true);
        }

        private void CheckChanges()
        {
            if (GroupName != _group.Name && !string.IsNullOrWhiteSpace(GroupName))
            {
                IsNewChanges = true;
                return;
            }
            IsNewChanges = false;
        }

        private bool Validate()
        {
            var message = string.Empty;

            if (string.IsNullOrWhiteSpace(GroupName))
            {
                message = Application.Current.Resources.MergedDictionaries[4]["GroupNameValid"].ToString();
            }

            if (message != string.Empty)
            {
                Application.Current.Dispatcher.Invoke(new Action((() => { CustomMessageBox.Show(Application.Current.Resources.MergedDictionaries[4]["Error"].ToString(), message); })));
                return false;
            }

            return true;
        }

        public void Update(List<UiInfo> membersToAdd)
        {
            if (membersToAdd?.Count != 0)
            {
                IsNewChanges = true;
                _membersToAdd = membersToAdd;

                var temp = new List<UiInfo>();
                temp.AddRange(_membersToAdd);
                temp.AddRange(GroupMemberList);
                var defImage = Image.FromFile("../../Resources/DefaultPicture.jpg");

                Task.Run(() =>
                {
                    foreach (var item in temp)
                    {
                        if (item is UserUiInfo)
                        {
                            var userUiInfo = item as UserUiInfo;
                            var user = UserServiceClient.GetUserById(userUiInfo.UserId);
                            var chatFile = GlobalBase.FileServiceClient.getChatFileById(user.Id);

                            if (chatFile?.Source != null && chatFile?.Source?.Length != 0)
                            {
                                var memstr = new MemoryStream(chatFile.Source);
                                Dispatcher.CurrentDispatcher.Invoke(() => { item.UiImage = Image.FromStream(memstr); });
                            }
                            else
                            {
                                Dispatcher.CurrentDispatcher.Invoke(() => { item.UiImage = defImage; });
                            }
                        }
                    }
                }).ContinueWith(Task =>
                {
                    GroupMemberList = temp;
                });
            }
        }
    }
}