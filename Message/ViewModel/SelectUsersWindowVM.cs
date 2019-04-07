﻿using Message.Interfaces;
using Message.Model;
using Message.UserServiceReference;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Message.ViewModel
{
    internal class SelectUsersWindowVM : BaseViewModel
    {
        private IView _view;
        private ChatGroup _group;

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

        private bool _isAddEnabled;

        public bool IsAddEnabled
        {
            get
            {
                return _isAddEnabled;
            }
            set
            {
                SetProperty(ref _isAddEnabled, value);
            }
        }

        public SelectUsersWindowVM(IView view, ChatGroup group)
        {
            _view = view;
            _group = group;
            IsAddEnabled = false;

            var tempContactList = new List<UiInfo>();
            Task.Run(() =>
            {
                tempContactList = UserServiceClient.GetAllContactsUiInfo(GlobalBase.CurrentUser.Id).Where(ui => ui is UserUiInfo).ToList();
                var part = UserServiceClient.GetGroupParticipants(_group.Id);
                foreach (var item in part)
                {
                    tempContactList.Remove(tempContactList.FirstOrDefault(x => (x as UserUiInfo).UserId == (item as UserUiInfo).UserId));
                }
            }).ContinueWith(task =>
            {
                ContactList = tempContactList;
                ContactList.ForEach(c => c.IsSelected = false);
            });
        }

        private DelegateCommand _addMembers;

        public DelegateCommand AddMembers =>
            _addMembers ?? (_addMembers = new DelegateCommand(ExecuteOnAddMembers));

        private DelegateCommand _checked;

        public DelegateCommand Checked =>
            _checked ?? (_checked = new DelegateCommand(ExecuteOnChecked));

        private void ExecuteOnAddMembers()
        {
            var membersToAdd = ContactList.Where(x => x.IsSelected).ToList();
            ((_view as Window).Owner.DataContext as EditGroupWindowVM).Update(membersToAdd);
            _view.CloseWindow();
        }

        private bool CanExecuteAdd()
        {
            if (ContactList != null && ContactList.First(x => x.IsSelected) != null)
            {
                return true;
            }
            return false;
        }

        private void ExecuteOnChecked()
        {
            IsAddEnabled = CanExecuteAdd();
        }
    }
}