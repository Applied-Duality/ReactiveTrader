1. Overview
2. Play with the App
3. Compiling from source
4. No experience with trading systems? Read this
5. Introduction to Reactive Trader
6. Features
7. Architecture

## 1. Overview

Reactive Trader is a client-server application demonstrating some of the problems needing to be dealt with when building reactive user interfaces. It was initially built as a demonstration app for a presentation we gave at [ReactConf 2014](http://reactconf.com/) and we have decided to open source it so the community can hopefully benefit.

We have selected the requirements carefully for this app to highlight the 4 pillars of reactive applications as defined in the [reactive manifesto](http://www.reactivemanifesto.org/): resilient, event-driven, scalable and responsive.

[Adaptive](http://weareadaptive.com) is a London based software consultancy. Even if we are relatively new, our consultants have been working on reactive backend systems and building reactive user interfaces in the finance industry for the last decade. We thought it would be great to share some of our ideas and designs and hopefully encourage discussion. 

If you have questions or ideas, please get in touch with us:
 - by email: info at weareadaptive dot com
 - on twitter [@AdaptiveLimited](https://twitter.com/adaptivelimited)
 - or by creating an entry on the [issue tracker](https://github.com/AdaptiveConsulting/ReactiveTrader/issues)

We will be blogging about specific aspects of Reactive Trader on [Adaptive's blog](http://weareadaptive.com/blog/) and new entries of the blog will be anounced on Twitter [@AdaptiveLimited](https://twitter.com/adaptivelimited) - Please subscribe!

There are 2 falvors of ReactiveTrader:
 - a .NET client using WPF, RxNET and SignalR
 - a Web client using TypeScript, RxJS and SignalR (work in progress)

## 2. Play with the app

Requirements:

 - Windows 
 - [Microsoft .NET Framework 4.5](http://www.microsoft.com/en-gb/download/details.aspx?id=30653)

Steps:

1. Download the [package](https://github.com/AdaptiveConsulting/ReactiveTrader/raw/master/src/ReactiveTrader.zip)
2. If you are on Windows 7+ unblock the zip (right click, properties, unblock)
3. Extract the zip
4. Launch the file __LaunchReactiveTrader__.bat to start both the client UI and the server admin UI, which allow you to control the server
5. Have fun

![image](https://f.cloud.github.com/assets/1256913/2470980/8e95e5c6-b01c-11e3-9311-cc17a7c1b191.png)

... talking via Web Socket with ...

![image](https://f.cloud.github.com/assets/1256913/2470993/d7f153ea-b01c-11e3-9c0c-ac8c8261299a.png)

## 3. Compiling from source

Requirements:
 - Visual Studio 2012/13
 - the [TypeScript Visual Studio plugin](http://www.microsoft.com/en-us/download/details.aspx?id=34790) - if you want to play with the JavaScript version of ReactiveTrader.

Steps:

1. Clone the project
2. Open the solution
3. Compile the solution
4. In solution properties choose both *.GUI projects as startup projects
5. hit F5

## 4. No experience with trading systems? Read this

If you know what FX, SPOT and SDP mean, you can skip this section.

This application is a (very) simplified version of what you would find in the the finance industry and more specifically in FX (Foreign Exchange). Investment banks, for instance, offer this type of applications to their clients so they can trade electronically with them. You might have heard of the acryonym SDP - it stands for Single Dealer Platforms. This term describes what a bank's e-commerce (electronic commerce) platforms is often called.

In the real world clients would launch these trading UIs and connect via the internet or via leased or otherwise  private lines to the bank's backend system.

Some financial products' price moves very quickly and this is very much the case in FX, especially for SPOT. What is SPOT? When some large company wants to change millions of US Dollars (USD) to Euro (EUR) for instance, and need to do this transaction "now" this is called a SPOT trade. FX e-commerce platform "stream" SPOT prices to their clients via APIs or providing a user interface. This price has two sides: a buy side and a sell side. The different between those prices is called the spread. It is not uncommon for a price - both buy and sell sides - to move several times a second on highly traded (liquid) currency pairs, such as EURUSD.

## 5. Introduction to Reactive Trader

Reactive Trader is a sample trading application composed as UI, written in WPF and a server, also written in .NET. To illustrate reactive UI problems we decided to implement trading functionalities around FX (Foreign Exchange): this is an area where price move quickly and you need an event driven architecture to cope with that.

Client-server communication needs to be duplex (ie. both the server and the client need to be able to send a message to the other party at any point in time). Lots of solutions exist for this, from simple web socket based libraries to high performance gateways that we commonly use when building such a system for a client.

We wanted something simple to setup so someone could clone this repo from GitHub and hit run, without much more configuration. We also wanted the transport to be "web friendly" so you could use the same concepts in HTML5/Javascript or any other UI technology. In the .NET world SignalR was a good candidate (it's an abstraction on top of web sockets with fallback to Server Sent Events, then to long polling, and so one). We did some investigations and decided that it was perfectly good enough for what we wanted to demonstrate here.

## 6. Features

In this section you will find a description of the different features of Reactive Trader, how you can reproduce them in the application and also some pointers to the relevant source code.

### 6.1. Streaming

The application streams spot prices for different currency pairs: the UI connects to the server and subscribes to price streams for EUR/USD and other currency pairs. The server generate pseudo-random prices and push them to the client. 

![image](https://f.cloud.github.com/assets/1256913/2321909/a8a6fcb2-a3aa-11e3-9cc2-036c77b6c6e7.png)

**Resilience**:
 - if connectivity with the server is lost, the UI detects the error and invalidates all the prices (pricing tiles become blank). Pricing tiles resubscribe automatically when the connection is re-established.
 - if a stream becomes stale (no update is seen for some time), the client will assume that there is a problem with the stream and invalidate the price, until it receives a new price.

**Responsive**: the UI client applies a conflation algorithm to protect the UI against burst of prices.

### 6.2. Trade execution

The user can specify an amount and whether they wish to buy or sell, then attempt to execute a trade. The server will process the request and accept or reject the trade. 

**Resilience**:
 - It is possible that the server does not respond in a timely maner and the client will timeout and display an error message.

### 6.3. Blotter

All trades executed, whether accepted or rejected are inserted in the blotter. In a real trading system the user would see only his own trades (or trades for his desk). In Reactive Trader all clients can see all the trades.

**Resilience**: 
 - If the server fails or the connection is lost, the blotter will still display the current trades, so that the user can continue to work, and attempt to reconnect. Once the connectivity is restored, the blotter will re-synchronize with the server and display the up to date list of trades.

### 6.4. Reference Data

Reactive Trader UI request the list of currency pairs from the server and automatically creates a pricing tile for each currency pair. 

The server can also notify the client when a currency pair is added or removed (you can think of this as adding a user permission for this currency pair or removing one). We decided to implement that feature to show that a push based design makes sense not only for streaming prices, but also to notify the client in real time when permissions or some other slow moving reference data changes, without requiring a restart of the client application.

**Resilience**: 
 - If the server fails or connection is lost, the client will keep the current list of currency pairs it has and wait for the connection to come back. Once the connection comes back it will re-subscribe and synchronize the currency pairs.

## 7. Architecture

The following architecture diagram provides a view of the main components and layering within the client and server. Note that the focus of reactive trader is reactive UI design, so we kept the server as dumb and simple as possible here.

![image](https://f.cloud.github.com/assets/1256913/2321883/421d7f48-a3aa-11e3-8a4c-22bf0858a085.png)
