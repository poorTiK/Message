using Message.Interfaces;
using Message.Model;
using Message.UserServiceReference;
using Prism.Commands;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Message.ViewModel
{
    internal class EditGroupWindowVM : BaseViewModel
    {
        private IView _view;
        private ChatGroupUiInfo _groupUiInfo;
        private ChatGroup _group;

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
            set { SetProperty(ref _groupName, value); }
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

        public EditGroupWindowVM(IView view, ChatGroupUiInfo group)
        {
            _view = view;
            _groupUiInfo = group;

            Init();
            SetAvatarForUI();
        }

        private void SetAvatarForUI()
        {
            Task.Run(() =>
            {
                var chatFile = GlobalBase.FileServiceClient.getChatFileById(_groupUiInfo.ImageId);

                if (chatFile?.Source?.Length > 0)
                {
                    MemoryStream memstr = new MemoryStream(chatFile.Source);
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
            List<UiInfo> tempUiInfos = new List<UiInfo>();
            Task.Run((() =>
            {
                _group = UserServiceClient.GetChatGroup(_groupUiInfo.UniqueName);

                tempUiInfos = UserServiceClient.GetGroupParticipants(_group.Id);
                var defImage = Image.FromFile("../../Resources/DefaultPicture.jpg");

                foreach (var item in tempUiInfos)
                {
                    if (item is UserUiInfo)
                    {
                        UserUiInfo userUiInfo = item as UserUiInfo;
                        User user = UserServiceClient.GetUserById(userUiInfo.UserId);
                        FileService.ChatFile chatFile = GlobalBase.FileServiceClient.getChatFileById(user.Id);

                        if (chatFile?.Source != null && chatFile?.Source?.Length != 0)
                        {
                            MemoryStream memstr = new MemoryStream(chatFile.Source);
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

        private void ExecuteOnApplyChanges()
        {
        }

        private void ExecuteOnBack()
        {
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
    }
}