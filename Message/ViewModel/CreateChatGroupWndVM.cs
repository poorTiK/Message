using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Message.Interfaces;
using Message.Compression;
using Message.UserServiceReference;
using Prism.Commands;
using Message.Model;

namespace Message.ViewModel
{
    class CreateChatGroupWndVM : BaseViewModel
    {
        private IView _view;

        private byte[] _newAvatar;

        private List<UiInfo> _contactList;

        public List<UiInfo> ContactList
        {
            get
            {
                return _contactList;
            }
            set
            {
                SetProperty(ref _contactList, value);
            }
        }

        private Image _image;

        public Image Images
        {
            get { return _image; }
            set { _image = value; OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Images")); }

        }

        private string _name;
        
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private DelegateCommand _createGroup;

        public DelegateCommand CreateGroup =>
            _createGroup ?? (_createGroup = new DelegateCommand(ExecuteOnCreate));

        private DelegateCommand _loadPhoto;

        public DelegateCommand LoadPhoto =>
            _loadPhoto ?? (_loadPhoto = new DelegateCommand(ExecuteOnLoadPhoto));

        public CreateChatGroupWndVM(IView view)
        {
            _view = view;
            ContactList = UserServiceClient.GetAllContactsUiInfo(GlobalBase.CurrentUser.Id).Where(ui => ui is UserUiInfo).ToList();
            ContactList.ForEach(c => c.IsSelected = false);

            SetAvatarForUI();
        }

        public CreateChatGroupWndVM(IView view, List<UiInfo> ContactsList) : this(view)
        {
            this.ContactList = ContactsList;

            SetAvatarForUI();
        }

        private void SetAvatarForUI()
        {
            Task.Run(() =>
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => {
                    Images = ImageHelper.GetDefGroupImage();
                });
            });
        }

        private bool _isCreating;
        public bool IsCreating
        {
            get { return _isCreating; }
            set { SetProperty(ref _isCreating, value); }
        }

        private void ExecuteOnCreate()
        {
            IsCreating = true;
            Task.Run(() =>
            {
                List<UserUiInfo> selectedContacts = ContactList.Where(c => c.IsSelected).Select(ui => ui as UserUiInfo).ToList();

                ChatGroup chatGroup = new ChatGroup();
                //TODO add validate
                chatGroup.Name = Name;

                UserServiceClient.AddOrUpdateChatGroup(chatGroup);
                chatGroup = UserServiceClient.GetChatGroup(chatGroup.Name);

                foreach (UserUiInfo uiInfo in selectedContacts)
                {
                    UserServiceClient.AddUserToChatGroupContact(chatGroup.Id, uiInfo.UserId);
                }

                UserServiceClient.AddUserToChatGroupContact(chatGroup.Id, GlobalBase.CurrentUser.Id);

                if (_newAvatar != null)
                {
                    var updatedChat = UserServiceClient.GetChatGroup(Name);
                    updatedChat.ImageId = GlobalBase.FileServiceClient.UploadFile(new FileService.ChatFile() { Source = CompressionHelper.CompressImage(_newAvatar) });
                    UserServiceClient.AddOrUpdateChatGroup(updatedChat);
                }
            }).ContinueWith(task =>
            {
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    IsCreating = false;
                    _view.CloseWindow();
                }));
            });
        }

        private void ExecuteOnLoadPhoto()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            var FilePath = openFileDialog.FileName;

            if (FilePath != string.Empty)
            {
                _newAvatar = File.ReadAllBytes(FilePath);
                MemoryStream memstr = new MemoryStream(_newAvatar);
                System.Windows.Application.Current.Dispatcher.Invoke(() => { Images = Image.FromStream(memstr); });
            }
        }
    }
}
