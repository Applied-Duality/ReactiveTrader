class TransportDisconnectedException implements Error {
    name: string;
    message: string;

    constructor(message: string) {
        this.message = message;
        this.name = "TransportDisconnectedException";
    }
} 