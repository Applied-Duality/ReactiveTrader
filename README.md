Reactive Trader
===============

Reactive Trader is a reference implementation for a reactive UI application, as defined in the [reactive manifesto](http://www.reactivemanifesto.org/). 

Reactive Trader is developed and maintained by [Adaptive](http://weareadaptive.com/): we are a London based consultancy specialised in building real-time trading systems.

The application was presented at [ReactConf 2014](http://reactconf.com/) by Lee Campbell and Matt Barrett as part of their talk on reactive UIs. (TODO link video here)

We build reactive trader to demonstrate what we think a reactive UI application is and specifically chose the requirements for this application to highlight the 4 pillars of reactive applications:
 - event-driven
 - scalable
 - resilient
 - responsive

Requirements
============

1. Display realtime streaming prices
------------------------------------

The application will present the user with a set of FX SPOT tiles, which are classically used to display FX prices in trading applications. 

The user should be able to select a currency pair within each tile. Once the currency pair is selected the application will subscribe to a server component which will provide a realtime price feed. The feed can be simulated (pseudo random numbers), a replay of some historical price tick data or a real feed.

The subscription to a price feed will contain:
 - the currency pair
 - the notional (optionally - this can be build if we have time to show how to modify or switch to another stream)

The quotes will contain
 - an identifier for the quote: a long starting a 0 and incremented by 1 for each currenncy pair stream 
 - a bid price
 - a ask price
 - a mid price
 - the value date (spot in this case)

This requirement will help to highligh the [event-driven](http://www.reactivemanifesto.org/#event-driven) nature of reactive applications.

2. Unsubscribe from a stream
----------------------------

The user should be able to close a tile. Once a tile is closed, reactive trader should notify the streaming service that this stream is no longer required and unsubscribe.

Closing the application should have the same effect and unsubscribe from all streams.

This requirement helps to show that when an application consumes a realtime feed of data, this is a steful operation for the server - and this state needs to be disposed at some point to not exhaust server resources or use resources which are no longer required.

3. Server to dispose all subscriptions on client disconnected
-------------------------------------------------------------

The server should be fault tolerant to a mis-behaving client. If the client crashes or the client / server connectivity is lost, the server should detect the disconnection and dispose all the active subscriptions for that user session with 10 seconds (to be able to demonstrate this during a demo).

4. Client to detect server disconnection
----------------------------------------

The client should be fault tolerant to a crash from the server or a loss of network connectivity. 
The client should detect within 5 seconds or less that the connection with the server has been lost, invalidate all prices and notify the user of the issue. The application will automatically enter in a reconnection loop and attempt to reconnect to the server and re-subscribe to all currency pairs. 

If it fails to do so within 30 seconds it will attempt to connect to a secondary pricing server instance and re-subscribe all price streams.

This requirement highlights some resilience aspects of the application and how fault detection and fail over can be implemented.

5. Client to detect a stale stream
----------------------------------

The pricing server could stay up and running but have some internal fault or one of its downstream systems could have a fault which could stale (or freeze) one or all the price streams. The client should detect when a price stream becomes stale, notify the client (invalidate the tile). Once the stream becomes live again the application should display prices again. The contract here is that the server should be able to heal itself from stale stream: the client is not going to resubscribe or failover when a single stream becomes stale.

This requirement will help to show techniques like heartbeating (resilience).

6. Client to handle burst of events without adding excessive layency
--------------------------------------------------------------------

When a client is subscribed to a set of streams it may happen that the server sends an important number of updates per seconds (tens to hundreads of updates per second per stream). The UI should implement some algorithm to prevent price tick processing time to exceed 200ms (time displayed in UI - time received from socket < 200ms)

The application will for instance apply a conflation algorithm: if several prices of a same stream (ie. same currency pair) are received within a fraction of a second the application can drop (ie. not render) some prices, as long as it meets the previous SLA.

The application should also display a visual indicator of the internal UI latency for price distribution so we can see the effects of bursts. A chart would be ideal.

7. client to be able to subscribe quickly to tens to hundreads of price streams
-------------------------------------------------------------------------------

When the UI starts up or when the user switch from one tab of tiles to another, the client should be able to subscribe and receive prices in a timely maner, even when the client has poor connectivity with the server (high latency link client/server).

This requirement will help showing the importance of batching, and the impacts of head of line blocking. A general rule of thumb should be that one user action should never trigger O(N) messages but a constant O(1) number of messages (ideally 1).  

8. Client to view its blotter
-----------------------------

The client should be able to see all trades he made during the day. The blotter will also display trades performed by other users (normally it would be limited to a desk but we won't model that here).

Once opened the blotter should receive all trades performed during the day (aka. state of the world).
When a trade is performed (done or rejected), a new line should appear in the blotter with the trade details.

Columns:
 - tradeId
 - trader name or sessionid of user who did the trade
 - currency pair
 - notional
 - direction (buy/sell)
 - spot price
 - trade date
 - value date
 - status (done/rejected)

9. Blotter to detect server failure
-----------------------------------

In the event of server failure (crash) or loss of connectivity between the client and its server, the blotter should detect the issue and notify the user. The blotter should then attempt to reconnect to the same node for 30sec, after which it will failover to another server.

Upon reconnection, the blotter should be able to sync it state with the server and display the correct list of trades (ie. some new trades might have been executed during the disconnection).

10. Blotter service to dispose client subscription on disconnection
-------------------------------------------------------------------

The blotter service should be able to detect a client disconnection (client close the app or network failure or client crash) and release all resources used for this user (subscription, cached trades, etc).

11. Client to execute a trade
-----------------------------

Double clicking on a side of a SPOT tile should send a trade request to the server. Upon reception of the request the server will accept or reject the trade (since we are simulating the server we can randomly reject 20% of the trades).

Once the server has decided if the trade was done or rejected it will send an affirmation back to the client who requested the trade.

The trade execution service will notify the blotter service with this new trade (done or rejected) and the blotter service will in turn notify all subscribed clients (broadcast).

Trade execution request:
 - currency pair
 - notional
 - quote id
 - streamID (if we decide to identiy streams per an identifier, otherwise currency pair should be fine)
 - direction (buy/sell)
 - dealt currency (for simplicity dealt currency will always be base currency, ie. for EURUSD the dealt currency is EUR)
 - user name / session ID (this should ideally be known of the server and not send by the client: we should not trust the client)

Affirmation:
 - tradeId
 - trader name or sessionid of user who did the trade
 - currency pair
 - notional
 - direction (buy/sell)
 - spot price
 - trade date
 - value date
 - status (done/rejected)




TODO (other requirements to document):
 - client to execute a trade (trade execution command + active query on trades - CQRS style / state machines)
 - client to subscribe to its blotter (state of the world, updates, no polling => reduce server load)
 - client to retrieve reference data (currency pairs) from server and allow adding new currency pairs at runtime without restarting the client
 - client to share streams for a currency pair (if same currency pair displayed in multiple tiles)
 - client to delay unsubscription to price stream to allow the client to reconnect quickly to a stream (switching between tabs, etc)
