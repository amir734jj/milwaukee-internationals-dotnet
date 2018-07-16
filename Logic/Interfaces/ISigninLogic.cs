namespace Logic.Interfaces
{
    public interface ISigninLogic
    {        
        void TryLogin(string username, string password, out bool result);

        void TryLogout(string username, string password, out bool resul);

        bool IsAuthenticated(string username, string password);
    }
}