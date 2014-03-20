## For the impatient

Requirements:
 - Windows 
 - [Microsoft .NET Framework 4.5](http://www.microsoft.com/en-gb/download/details.aspx?id=30653)

1. Download the [package](https://github.com/AdaptiveConsulting/ReactiveTrader/raw/master/src/ReactiveTrader.zip)
2. If you are on Windows 7+ unblock the zip (right click, properties, unblock)
3. Extract the zip
4. Launch the file __LaunchReactiveTrader__.bat to start both the client UI and the server admin UI, which allow you to control the server
5. Have fun

![image](https://f.cloud.github.com/assets/1256913/2311956/3fd6d2ca-a2f6-11e3-8224-d83a4e107b5a.png)

... talking via Web Sockets ...

![image](https://f.cloud.github.com/assets/1256913/2311970/5c2b2f0c-a2f6-11e3-92ba-380d2f383351.png)

## Overview

Reactive Trader is a client server application demonstrating some of the problems you have to deal with when building reactive UIs. It was initially build as a demo app for a presentation we gave at ReactConf 2014 and we decided to open source it (video available soon).

We have selected the requirements carefully for this app to highlight the 4 pillars of reactive applications as defined in the reactive manifesto: resilient, event-driven, scalable and responsive.

We are a London based software consultancy called Adaptive and, even if the company is relatively new, our consultants have been working on reactive backend systems and building reactive UIs in the finance industry for pretty much the last decade. We thought it would be great to share some of our ideas and design and hopefully open the discussion. If you have questions or ideas, please give us a shout!

## No experience with trading systems? Read this

If you know what FX, SPOT and SDP means, you can skip this section.

This application is a (very) simplified and cut-down version of what you would find in the the finance industry and more specifically in FX (Foreign Exchange). Banks, for instance, offer this type of applications to their clients so they can trade electronically with them. You might have heard of SDP - it stands for Single Dealer Platforms, this is how Banks eCommerce platforms are often called.

In the real-world clients would launch those trading UI and they would connect via internet or some private lines to the bank backend system.

Some financial products' price moves very quickly and this is very much the case in FX, especially for SPOT. What is FX SPOT? When some big company want to change millions of US Dollars (USD) to Euro (EUR) for instance, and need to do this transaction "now" this is called a SPOT trade. FX eCommerce platform "stream" SPOT prices to their clients via APIs or providing UIs and this price has to sides: a buy side and a sell side. The different between those prices is called the spread. It is not rare that a price will be moving several times per second on the main currency pairs.

## Intro to Reactive Trader

Reactive Trader is a sample trading application composed a UI, written in WPF and a server, also written in .NET. To illustrate reactive UI problems we decided to implement trading functionalities around FX (Foreign Exchange): this is an area where price are moving quickly and you need an event driven architecture to cope with that.

Client-server communication needs to be duplex (ie. both the server and the client need to be able to send a message to the other party at any point in time). Lots of solutions exist for that, from simple web socket based libraries to high performance gateways that we commonly use when building such system in Finance.

We wanted something very simple to setup so somebody can clone from GitHub and hit run, without much more configuration and also wanted the transport to be "web friendly" so you can use the same concepts in HTML5/Javascript or any other UI technology. In the .NET world SignalR was a good candidate (it's an abstraction on top of web socket with fallback to Server Send Event, long polling, etc) - we did some investigations and decided that it was perfectly good enough for what we want to demonstrate here.


## Features

In this section you will find a description of the different features of Reactive Trader, how you can reproduce them in the application and also some pointers to the relevant source code.

### Streaming

The application streams spot prices for different currency pairs: the UI connects to the server and subscribes to price streams for EUR/USD and other currency pairs. The server generate pseudo-random prices and push them to the client. 

![image](https://f.cloud.github.com/assets/1256913/2321909/a8a6fcb2-a3aa-11e3-9cc2-036c77b6c6e7.png)

**Resilience**:
 - if connectivity with the server is lost, the UI client detects the error and invalidate all the prices (pricing tiles become blank). Pricing tiles resubscribe automatically when the connection is re-established.
 - if a stream becomes stale (stops to update for some time), the client will assume that there is a problem with the stream and invalid the price, until it receives a new price.

**Responsive**: the UI client applies a conflation algorithm to protect the UI against burst of prices.
### Trade execution

The user can client the buy or sell price to execute a trade. The server will process the request and accept or reject the trade. 

**Resilience**:
 - It is possible that the server does not respond in a timely maner and the client will timeout and display an error message.

### Blotter

All trade executed, wether accepted or rejected are inserted in the blotter. In a real trading system the user would see only his own trade (or trades for his desk), in reative trade all clients see all the trades.

**Resilience**: 
 - If the server fails or connection is lost, the blotter will still display the current trades, so that the user can continue to work, and attempt to reconnect. Once the connectivity is retrieved, the blotter will re-synchronize with the server and display the up to date list of trades.

### Reference Data

Reactive Trader UI request the list of currency pairs from the server and automatically creates a pricing tile for each currency pair. 

The server can also notify the client when a currency pair is added or removed (you can think of this as adding a user permission for this currency pair or removing one). We decided to implement that feature to show that a push based design makes sense not only for streaming prices, but also to notify the client in real time when permissions or some other slow moving reference data changes, without requiring a restart of the client application.

**Resilience**: 
 - If the server fails or connection is lost, the client will keep the current list of currency pairs it has and wait for the connection to come back. Once the connection comes back it will re-subscribe and synchronize the currency pairs.

## Architecture

The following architecture diagram provides a view of the main components and layering within the client and server. Note that the focus of reactive trader is reactive UI design, so we kept the server as dumb and simple as possible here.

![image](https://f.cloud.github.com/assets/1256913/2321883/421d7f48-a3aa-11e3-8a4c-22bf0858a085.png)

## Reactive domain

## Technologies used


