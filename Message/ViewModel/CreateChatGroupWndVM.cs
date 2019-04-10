using Message.AdditionalItems;
using Message.Compression;
using Message.Interfaces;
using Message.Model;
using Message.UserServiceReference;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace Message.ViewModel
{
    internal class CreateChatGroupWndVM : BaseViewModel
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
            set
            {
                try
                {
                    SetProperty(ref _name, value, () =>
                           {
                               if (!string.IsNullOrWhiteSpace(value))
                               {
                                   IsCanCreate = true;
                               }
                               else
                               {
                                   IsCanCreate = false;
                               }
                           });
                }
                catch (Exception)
                {

                }
            }
        }

        private bool _isCanCreate;

        public bool IsCanCreate
        {
            get { return _isCanCreate; }
            set { SetProperty(ref _isCanCreate, value); }
        }

        private DelegateCommand _createGroup;

        public DelegateCommand CreateGroup =>
            _createGroup ?? (_createGroup = new DelegateCommand(ExecuteOnCreate));

        private DelegateCommand _loadPhoto;

        public DelegateCommand LoadPhoto =>
            _loadPhoto ?? (_loadPhoto = new DelegateCommand(ExecuteOnLoadPhoto));

        public CreateChatGroupWndVM(IView view)
        {
            try
            {
                _view = view;
                ContactList = UserServiceClient.GetAllContactsUiInfo(GlobalBase.CurrentUser.Id).Where(ui => ui is UserUiInfo).ToList();
                ContactList.ForEach(c => c.IsSelected = false);

                SetAvatarForUI();
            }
            catch (Exception)
            {

            }
        }

        public CreateChatGroupWndVM(IView view, List<UiInfo> ContactsList) : this(view)
        {
            try
            {
                this.ContactList = ContactsList;

                IsCanCreate = false;

                SetAvatarForUI();
            }
            catch (Exception)
            {

            }
        }

        private void SetAvatarForUI()
        {
            try
            {
                Task.Run(() =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Images = ImageHelper.GetDefGroupImage();
                    });
                });
            }
            catch (Exception)
            {

            }
        }

        private bool _isCreating;

        public bool IsCreating
        {
            get { return _isCreating; }
            set { SetProperty(ref _isCreating, value); }
        }

        private void ExecuteOnCreate()
        {
            try
            {
                if (Validate())
                {
                    IsCreating = true;
                    Task.Run(() =>
                    {
                        var selectedContacts = ContactList.Where(c => c.IsSelected).Select(ui => ui as UserUiInfo).ToList();

                        var chatGroup = new ChatGroup();

                        chatGroup.Name = Name;

                        UserServiceClient.AddOrUpdateChatGroup(chatGroup);
                        chatGroup = UserServiceClient.GetChatGroup(chatGroup.Name);

                        foreach (var uiInfo in selectedContacts)
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
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            IsCreating = false;
                            _view.CloseWindow();
                        }));
                    });
                }
            }
            catch (Exception)
            {

            }
        }

        private void ExecuteOnLoadPhoto()
        {
            try
            {
                var openFileDialog = new OpenFileDialog();
                openFileDialog.ShowDialog();
                var FilePath = openFileDialog.FileName;

                if (FilePath != string.Empty)
                {
                    _newAvatar = File.ReadAllBytes(FilePath);
                    var memstr = new MemoryStream(_newAvatar);
                    Application.Current.Dispatcher.Invoke(() => { Images = Image.FromStream(memstr); });
                }
            }
            catch (Exception)
            {

            }
        }

        private bool Validate()
        {
            try
            {
                var message = string.Empty;

                if (string.IsNullOrWhiteSpace(Name))
                {
                    message = Translations.GetTranslation()["FirstNameValid"].ToString();
                }
                else if (!ContactList.Any(x => x.IsSelected == true))
                {
                    message = Translations.GetTranslation()["NoMemmbersSelected"].ToString();
                }

                if (message != string.Empty)
                {
                    Application.Current.Dispatcher.Invoke(new Action((() => { CustomMessageBox.Show(Translations.GetTranslation()["Error"].ToString(), message); })));
                    return false;
                }
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
    }
}