namespace Adaptive.ReactiveTrader.Client.Configuration
{
    class UserProvider : IUserProvider
    {
        public string Username
        {
            //get { return System.Environment.UserName; }
            get { return System.DirectoryServices.AccountManagement.UserPrincipal.Current.DisplayName; }
        }
    }
}