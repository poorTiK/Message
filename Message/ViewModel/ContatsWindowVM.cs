﻿using Message.Interfaces;
using Message.Model;
using Message.UserServiceReference;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;
using Message.PhotoServiceReference;

namespace Message.ViewModel
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    internal class ContatsWindowVM : Prism.Mvvm.BindableBase, IUserServiceCallback
    {
        private InstanceContext usersSite;
        private UserServiceClient UserServiceClient;
        private IUserServiceCallback _userServiceCallback;

        private IView view;

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
                    //ContactsList = UserServiceClient.FindUsersByLogin(ContactsSearch);
                    using (UserServiceClient = new UserServiceClient(usersSite))
                    {
                        ContactsList = UserServiceClient.FindUsersUiUnfoByLogin(ContactsSearch);
                    }
                }
                else
                {
                    UpdateContacts();
                }

                ManageControls();
            }
        }

        public ContatsWindowVM(IView iview)
        {
            view = iview;

            _userServiceCallback = this;
            usersSite = new InstanceContext(_userServiceCallback);
            using (UserServiceClient = new UserServiceClient(usersSite))
            {
                //ContactsList = UserServiceClient.GetAllContacts(GlobalBase.CurrentUser.Id);
                ContactsList = UserServiceClient.GetAllContactsUiInfo(GlobalBase.CurrentUser.Id);
            }

            using (var proxy = new PhotoServiceClient())
            {
                foreach (var item in ContactsList)
                {
                    if (item is UserUiInfo)
                    {
                        UserUiInfo userUiInfo = item as UserUiInfo;
                        item.Avatar = proxy.GetPhotoById(userUiInfo.UserId);
                    }
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
            User user = null;
            if (SelectedContact is UserUiInfo) {
                using (UserServiceClient = new UserServiceClient(usersSite))
                {
                    UserUiInfo userUiInfo = SelectedContact as UserUiInfo;
                    user = UserServiceClient.GetUserById(userUiInfo.UserId);
                }
            }

            var wnd = new ContactProfileWindow(user);
            wnd.Owner = (Window)view;
            wnd.ShowDialog();

            UpdateContacts();
        }

        private void ExecuteOnCloseCommand()
        {
            view.CloseWindow();
        }

        private void ExecuteOnAddContactCommand()
        {
            if (SelectedContact != null)
            {
                User user = null;
                using (UserServiceClient = new UserServiceClient(usersSite))
                {
                    UserUiInfo userUiInfo = SelectedContact as UserUiInfo;
                    user = UserServiceClient.GetUserById(userUiInfo.UserId);
                    UserServiceClient.AddContact(GlobalBase.CurrentUser, user);
                }

                UpdateContacts();
            }

            ManageControls();
        }

        private void UpdateContacts()
        {
            //ContactsList = UserServiceClient.GetAllContacts(GlobalBase.CurrentUser.Id);

            using (UserServiceClient = new UserServiceClient(usersSite))
            {
                ContactsList = UserServiceClient.GetAllContactsUiInfo(GlobalBase.CurrentUser.Id);
            }

            using (var proxy = new PhotoServiceClient())
            {
                foreach (var item in ContactsList)
                {
                    if (item is UserUiInfo)
                    {
                        UserUiInfo userUiInfo = item as UserUiInfo;
                        item.Avatar = proxy.GetPhotoById(userUiInfo.UserId);
                    }
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

        public void UserLeave(User user)
        {
            //int test = 10;
        }

        public void UserCame(User user)
        {
            //int test = 20;
        }

        public void ReceiveMessage(BaseMessage message)
        {
            //throw new NotImplementedException();
        }

        public void OnMessageRemoved(BaseMessage message)
        {
            //throw new NotImplementedException();
        }

        public void OnMessageEdited(BaseMessage message)
        {
            //throw new NotImplementedException();
        }
    }
}