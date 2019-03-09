using Message.UserServiceReference;

namespace Message.Interfaces
{
    public interface ISerializeUser
    {
        void SerializeUser(User user);
        void CleanCurrentUser();
        string GetCurrentUser();
    }
}
