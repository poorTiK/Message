namespace Message.Interfaces
{
    public interface IPasswordSupplier
    {
        string GetPasswordForLogin();

        string GetPasswordForRegistration();

        string GetRepPasswordForRegistration();

        void ClearPassword();
    }
}