<Query Kind="Statements">
  <Reference Relative="Adaptive.ReactiveTrader.Client.Domain\bin\Debug\Adaptive.ReactiveTrader.Client.Domain.dll">&lt;MyDocuments&gt;\git\ReactiveTrader\App\src\Adaptive.ReactiveTrader.Client.Domain\bin\Debug\Adaptive.ReactiveTrader.Client.Domain.dll</Reference>
  <Reference Relative="Adaptive.ReactiveTrader.Client.Domain\bin\Debug\Adaptive.ReactiveTrader.Shared.dll">&lt;MyDocuments&gt;\git\ReactiveTrader\App\src\Adaptive.ReactiveTrader.Client.Domain\bin\Debug\Adaptive.ReactiveTrader.Shared.dll</Reference>
  <Reference Relative="Adaptive.ReactiveTrader.Client.Domain\bin\Debug\log4net.dll">&lt;MyDocuments&gt;\git\ReactiveTrader\App\src\Adaptive.ReactiveTrader.Client.Domain\bin\Debug\log4net.dll</Reference>
  <Reference Relative="Adaptive.ReactiveTrader.Client.Domain\bin\Debug\Microsoft.AspNet.SignalR.Client.dll">&lt;MyDocuments&gt;\git\ReactiveTrader\App\src\Adaptive.ReactiveTrader.Client.Domain\bin\Debug\Microsoft.AspNet.SignalR.Client.dll</Reference>
  <Reference Relative="Adaptive.ReactiveTrader.Client.Domain\bin\Debug\Newtonsoft.Json.dll">&lt;MyDocuments&gt;\git\ReactiveTrader\App\src\Adaptive.ReactiveTrader.Client.Domain\bin\Debug\Newtonsoft.Json.dll</Reference>
  <Reference Relative="Adaptive.ReactiveTrader.Client.Domain\bin\Debug\System.Reactive.Core.dll">&lt;MyDocuments&gt;\git\ReactiveTrader\App\src\Adaptive.ReactiveTrader.Client.Domain\bin\Debug\System.Reactive.Core.dll</Reference>
  <Reference Relative="Adaptive.ReactiveTrader.Client.Domain\bin\Debug\System.Reactive.Interfaces.dll">&lt;MyDocuments&gt;\git\ReactiveTrader\App\src\Adaptive.ReactiveTrader.Client.Domain\bin\Debug\System.Reactive.Interfaces.dll</Reference>
  <Reference Relative="Adaptive.ReactiveTrader.Client.Domain\bin\Debug\System.Reactive.Linq.dll">&lt;MyDocuments&gt;\git\ReactiveTrader\App\src\Adaptive.ReactiveTrader.Client.Domain\bin\Debug\System.Reactive.Linq.dll</Reference>
  <Reference Relative="Adaptive.ReactiveTrader.Client.Domain\bin\Debug\System.Reactive.PlatformServices.dll">&lt;MyDocuments&gt;\git\ReactiveTrader\App\src\Adaptive.ReactiveTrader.Client.Domain\bin\Debug\System.Reactive.PlatformServices.dll</Reference>
  <Namespace>Adaptive.ReactiveTrader.Client.Domain</Namespace>
  <Namespace>System</Namespace>
  <Namespace>System.Reactive</Namespace>
  <Namespace>System.Reactive.Linq</Namespace>
</Query>

var api = new ReactiveTrader();
api.Initialize("olivier", new []{"http://localhost:8080"});

api.ConnectionStatus.Dump();


var prices = from currencyPairs in api.ReferenceData.GetCurrencyPairs()
             let eurusd = currencyPairs.First(cp=>cp.Symbol == "EURUSD")
             from price in eurusd.Prices
			 where !price.IsStale
			 select new { Bid = price.Bid.Rate, Ask = price.Ask.Rate}; 
			 
prices.Dump();