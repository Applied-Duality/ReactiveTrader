namespace Adaptive.ReactiveTrader.Shared
{
    public static class ServiceConstants
    {
        public static class Server
        {
            public const string UsernameHeader = "User";

            // pricing
            public const string PricingHub = "PricingHub";
            public const string SubscribePriceStream = "SubscribePriceStream";
            public const string UnsubscribePriceStream = "UnsubscribePriceStream";

            // blotter
            public const string BlotterHub = "BlotterHub";
            public const string SubscribeTrades = "SubscribeTrades";
            public const string UnsubscribeTrades = "UnsubscribeTrades";
            
            // reference data
            public const string ReferenceDataHub = "ReferenceDataHub";
            public const string GetCurrencyPairs = "GetCurrencyPairs";

            // executution
            public const string ExecutionHub = "ExecutionHub";
            public const string Execute = "Execute";
        }

        public static class Client
        {
            public const string OnNewPrice = "OnNewPrice";
            public const string OnNewTrade = "OnNewTrade"; 
        }
    }
}