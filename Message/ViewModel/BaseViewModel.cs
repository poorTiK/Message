using Message.UserServiceReference;
using System.ServiceModel;

namespace Message.ViewModel
{
    public abstract class BaseViewModel : Prism.Mvvm.BindableBase, IUserServiceCallback
    {
        protected InstanceContext usersSite;
        protected UserServiceClient UserServiceClient;
        protected IUserServiceCallback _userServiceCallback;

        public BaseViewModel()
        {
            _userServiceCallback = this;
            usersSite = new InstanceContext(_userServiceCallback);
            UserServiceClient = new UserServiceClient(usersSite);
        }

        public virtual void OnMessageEdited(BaseMessage message)
        {
            //throw new NotImplementedException();
        }

        public virtual void OnMessageRemoved(BaseMessage message)
        {
            //throw new NotImplementedException();
        }

        public virtual void ReceiveMessage(BaseMessage message)
        {
            //throw new NotImplementedException();
        }

        public virtual void UserCame(User user)
        {
            //throw new NotImplementedException();
        }

        public virtual void UserLeave(User user)
        {
            //throw new NotImplementedException();
        }
    }
}