using System.Globalization;

namespace Adaptive.ReactiveTrader.Client.Configuration
{
    class UserProvider : IUserProvider
    {
        public string Username
        {
            get
            {
                var userName = System.Environment.UserName;

                return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(userName);
            }
        }
    }
}