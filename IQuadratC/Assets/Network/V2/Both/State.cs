namespace Network.V2.Both
{
    public enum Packets
    {
        serverConnection = 1,
        clientConnectionRecived = 2,
        debugMessage = 3,
        debugImage = 4,

    }

    public enum NetworkState
    {
        notConnected = 1,
        connected = 2,
    }

    public static class State
    {
        public const int BufferSize = 4096;
    }
}