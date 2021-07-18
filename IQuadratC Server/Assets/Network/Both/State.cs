namespace Network.Both
{
    public enum Packets
    {
        debugMessage = 1,

        serverSettings = 2,  
        clientSettings = 3,
        serverStartUDP = 4,
        clientUDPConnection = 5,
        serverUDPConnection = 6,
        clientUDPConnectionStatus = 7,
        
        serverCamImage = 8,
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