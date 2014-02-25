namespace Adaptive.ReactiveTrader.Client.Configuration
{
    class UserProvider : IUserProvider
    {
        public string Username
        {
            get { return System.Environment.UserName; }
            
            // TODO Olivier: I've disabled that code as it is super slow (2-5sec on my laptop). Not sure what we should use here..
            //get { return System.DirectoryServices.AccountManagement.UserPrincipal.Current.DisplayName; }
        }
    }
}