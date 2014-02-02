namespace Dto
{
    public static class ServiceConstants
    {
        public static class Server
        {
            public const string TradingServiceHub = "TradingServiceHub";
            public const string SubscribePriceStream = "SubscribePriceStream";
            public const string UnsubscribePriceStream = "UnsubscribePriceStream";
            public const string SubscribeTrades = "SubscribeTrades";
            public const string UnsubscribeTrades = "UnsubscribeTrades";
            public const string GetCurrencyPairs = "GetCurrencyPairs";
            public const string UsernameHeader = "User";
            public const string Execute = "Execute";
        }

        public static class Client
        {
            public const string OnNewPrice = "OnNewPrice";
            public const string OnNewTrade = "OnNewTrade"; 
        }
    }
}