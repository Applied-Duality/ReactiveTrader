 class ServiceClientBase {
     private _connectionProvider: IConnectionProvider;

     constructor(connectionProvider: IConnectionProvider) {
         this._connectionProvider = connectionProvider;
     }

     getResilientStream<T>(streamFactory: (connection: IConnection) => Rx.Observable<T>, connectionTimeoutMs: number): Rx.Observable<T> {
         var activeConnections =
             this._connectionProvider.getActiveConnection()
                 .selectMany(connection => connection.status,
                 (connection: IConnection, status: ConnectionInfo) => {
                     return { c: connection, s: status };
                 })
                 .where(t => t.s.connectionStatus == ConnectionStatus.Connected
                     || t.s.connectionStatus == ConnectionStatus.Reconnected)
                 .select(t => t.c)
                 .publish()
                 .refCount();

         // get the first connection
         var firstConnection = activeConnections.take(1).timeout(connectionTimeoutMs);

         // 1 - notifies when the first connection gets disconnected
         var firstDisconnection =
             firstConnection.selectMany(connection => connection.status,
                 (connection: IConnection, status: ConnectionInfo) => {
                     return { c: connection, s: status };
                 })
                 .where(t => t.s.connectionStatus == ConnectionStatus.Reconnecting ||
                            t.s.connectionStatus == ConnectionStatus.Closed)
                 .select(t => { });

         // 2- connection provider created a new connection it means the active one has droped
         var subsequentConnection = activeConnections.skip(1).select(_ => { }).take(1);

         // OnError when we get 1 or 2
         var disconnected = <Rx.Observable<T>> firstDisconnection.merge(subsequentConnection)
             .select(_ => Rx.Notification.createOnError<T>(new TransportDisconnectedException("Connection was closed.")))
             .dematerialize();

         // create a stream which will OnError as soon as the connection drops
         return firstConnection.selectMany(connection => streamFactory(connection))
             .merge(disconnected)
             .publish()
             .refCount();
     }

     requestUponConnection<T>(factory: (connection: IConnection) => Rx.Observable<T>, timeoutMs: number): Rx.Observable<T> {
         return this._connectionProvider.getActiveConnection().take(1).timeout(timeoutMs)
             .selectMany(c => factory(c))
             .take(1)
             .publishLast()
             .refCount();
     }
 }