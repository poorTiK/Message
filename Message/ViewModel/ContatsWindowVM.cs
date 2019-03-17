﻿using Message.Interfaces;
using Message.Model;
using Message.UserServiceReference;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;
using Message.PhotoServiceReference;
using System.Drawing;
using System.IO;
using System.Windows.Threading;

namespace Message.ViewModel
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    internal class ContatsWindowVM : BaseViewModel
    {
        private IView view;

        private Image _image;

        public Image Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Image"));
            }
        }

        private List<UiInfo> _contacts;

        public List<UiInfo> ContactsList
        {
            get { return _contacts; }
            set { SetProperty(ref _contacts, value); }
        }

        private string _caption;

        public string Caption
        {
            get { return _caption; }
            set { SetProperty(ref _caption, value); }
        }

        private UiInfo _selectedContact;

        public UiInfo SelectedContact
        {
            get { return _selectedContact; }
            set { SetProperty(ref _selectedContact, value); }
        }

        private string _contactsSearch;

        public string ContactsSearch
        {
            get { return _contactsSearch; }
            set
            {
                SetProperty(ref _contactsSearch, value);

                if (!string.IsNullOrEmpty(value))
                {
                        ContactsList = UserServiceClient.FindUsersUiUnfoByLogin(ContactsSearch);
                }
                else
                {
                    UpdateContacts();
                }

                ManageControls();
            }
        }

        public ContatsWindowVM(IView iview) : base()
        {
            view = iview;
            ContactsList = UserServiceClient.GetAllContactsUiInfo(GlobalBase.CurrentUser.Id);

            foreach (var item in ContactsList)
            {
                if (item is UserUiInfo)
                {
                    UserUiInfo userUiInfo = item as UserUiInfo;
                    item.Avatar = GlobalBase.PhotoServiceClient.GetPhotoById(userUiInfo.UserId);
                }
                else if (item is ChatGroupUiInfo)
                {

                }
            }

            ManageControls();

        }

        private DelegateCommand _onAddContact;

        public DelegateCommand AddContactCommand =>
            _onAddContact ?? (_onAddContact = new DelegateCommand(ExecuteOnAddContactCommand));

        private DelegateCommand _onClose;

        public DelegateCommand CloseCommand =>
            _onClose ?? (_onClose = new DelegateCommand(ExecuteOnCloseCommand));

        private DelegateCommand _openProfile;

        public DelegateCommand OpenProfile =>
            _openProfile ?? (_openProfile = new DelegateCommand(ExecuteOnOpenProfile));

        private void ExecuteOnOpenProfile()
        {
            if (SelectedContact is UserUiInfo) {

                UserUiInfo userUiInfo = SelectedContact as UserUiInfo;
                User user = UserServiceClient.GetUserById(userUiInfo.UserId);

                var wnd = new ContactProfileWindow(user);
                wnd.Owner = (Window)view;
                wnd.ShowDialog();

                UpdateContacts();
            }
        }

        private void ExecuteOnCloseCommand()
        {
            view.CloseWindow();
        }

        private void ExecuteOnAddContactCommand()
        {
            if (SelectedContact != null)
            {
                UserUiInfo userUiInfo = SelectedContact as UserUiInfo;
                User user = UserServiceClient.GetUserById(userUiInfo.UserId);
                UserServiceClient.AddUserToUserContact(GlobalBase.CurrentUser.Id, user.Id);

                UpdateContacts();
            }

            ManageControls();
        }

        private void UpdateContacts()
        {
            ContactsList = UserServiceClient.GetAllContactsUiInfo(GlobalBase.CurrentUser.Id);


                foreach (var item in ContactsList)
                {
                    if (item is UserUiInfo)
                    {
                        UserUiInfo userUiInfo = item as UserUiInfo;
                        item.Avatar = GlobalBase.PhotoServiceClient.GetPhotoById(userUiInfo.UserId);
                    }
                }


            ManageControls();
        }

        private void ManageControls()
        {
            if (string.IsNullOrEmpty(ContactsSearch))
            {
                Caption = "Contacts";
            }
            else
            {
                Caption = "Contacts search";
            }
        }

        private void SetAvatarForUI(User Profile)
        {
            using (var proxy = new PhotoServiceClient())
            {
                Profile.Avatar = proxy.GetPhotoById(Profile.Id);
            }

            if (Profile?.Avatar?.Length > 0)
            {
                MemoryStream memstr = new MemoryStream(GlobalBase.CurrentUser.Avatar);
                Dispatcher.CurrentDispatcher.Invoke(() => { Image = Image.FromStream(memstr); });
            }
            else
            {
                Dispatcher.CurrentDispatcher.Invoke(() => { Image = Image.FromFile(@"Resources\DefaultPicture.jpg"); });
            }
        }
    }
}