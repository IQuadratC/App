using System;

namespace Network
{
    public enum NetworkState
    {
        undefined = 0,
        notConnected = 1,
        connecting = 2,
        idle = 3,
        sending = 4,
        reciving = 5,
        disconnecting = 6,
    }

    public enum MessageId
    {
        debugText = 1,
        notFullyRecived = 2,
        resendLastMessage = 3,

        clientDisconnect = 100,
        serverDisconnect = 101,
    }
}