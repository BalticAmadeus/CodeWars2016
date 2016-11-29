namespace GameLogic
{
    public interface IAccessManager
    {
        void CheckAccess(string address);
        AccessToken GetAccessToken(string address);
        AccessToken GrantAccess(string address, string request);
        string RequestAccess(string address);
        string RequestRevoke(string address);
        bool RevokeAccess(string address, string request);
    }
}