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

        clientControllMode = 10, // int 1 = no control, 1 = Joystick, 3 = AI
        clientJoystickMove = 21, // dir norm Vec2, speed float
        clientJoystickRotate = 22, // speed float, pos = right, neg = left
        clientJoystickStop = 23, 

        clientMoveAI = 25,
    }

    public enum NetworkState
    {
        notConnected = 1,
        connected = 2,
        connecting = 3,
        disconnecting = 4,
    }
    
    public enum ControllMode
    {
        undefined = 0,
        noControll = 1,
        joystick = 2,
        AIControll = 3,
    }

    public static class State
    {
        public const int BufferSize = 4096;
        public const int MaxClients = 255;
        public const int HeaderSize = 6;
        
        public const float MaxJoystickSendIntervall = 1;
    }
}