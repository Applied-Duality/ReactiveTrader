class ConnectionInfo {
    connectionStatus: ConnectionStatus;
    server: string;

    constructor(connectionStatus: ConnectionStatus, server: string) {
        this.connectionStatus = connectionStatus;
        this.server = server;
    }

    toString() {
        return "ConnectionStatus: " + this.connectionStatus + ", Server: " + this.server;
    }
}