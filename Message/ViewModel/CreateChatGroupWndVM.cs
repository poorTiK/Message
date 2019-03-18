using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Message.Interfaces;
using Message.Model;
using Message.UserServiceReference;
using Prism.Commands;

namespace Message.ViewModel
{
    class CreateChatGroupWndVM : BaseViewModel
    {
        private IView _view;

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

        private string _name;
        
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private DelegateCommand _createGroup;

        public DelegateCommand CreateGroup =>
            _createGroup ?? (_createGroup = new DelegateCommand(ExecuteOnCreate));

        public CreateChatGroupWndVM(IView view)
        {
            _view = view;
            ContactList = UserServiceClient.GetAllContactsUiInfo(GlobalBase.CurrentUser.Id).Where(ui => ui is UserUiInfo).ToList();
            ContactList.ForEach(c => c.IsSelected = false);
        }

        public CreateChatGroupWndVM(IView view, List<UiInfo> ContactsList) : this(view)
        {
            this.ContactList = ContactsList;
        }

        private void ExecuteOnCreate()
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

            _view.CloseWindow();
        }
    }
}
