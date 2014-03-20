<Query Kind="Program">
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
  <Namespace>Adaptive.ReactiveTrader.Client.Domain.Models</Namespace>
</Query>

void Main()
{
	var api = new ReactiveTrader();
	api.Initialize("Olivier", new []{"http://localhost:8080"});
	
	api.ConnectionStatusStream.DumpLive();
	
	var eurusd = from currencyPairs in api.ReferenceData.GetCurrencyPairsStream()
	             from currencyPair in currencyPairs
				 where currencyPair.CurrencyPair.Symbol == "EURUSD"
	             from price in currencyPair.CurrencyPair.PriceStream
				 select price;
				 
	eurusd.Select((p,i)=> "price" + i + ":" + p.ToString()).DumpLive();
	
	var execution = from price in eurusd.Skip(10).Take(1)
					from trade in price.Ask.ExecuteRequest(10000, "EUR")
					select trade.ToString();
	
	execution.DumpLive();				 
}