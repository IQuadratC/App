namespace Network.V2.Both
{
    public enum Packets
    {
        serverConnection = 1,
        clientConnectionRecived = 2,
        debugMessage = 3,
    }

    public enum NetworkState
    {
        notConnected = 1,
        connected = 2,
        connecting = 3,
        disconnecting = 4,
    }

    public static class State
    {
        public const int BufferSize = 4096;
        public const int MaxClients = 255;
        public const int HeaderSize = 6;
    }
}