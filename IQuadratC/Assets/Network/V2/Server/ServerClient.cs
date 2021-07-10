using System.Net;
using System.Net.Sockets;
using Network.V2.Both;

namespace Network.V2.Server
{
    public class ServerClient
    {
        public byte id;
        public TcpClient socket;
        public NetworkStream stream;
        public byte[] receiveBuffer;
        public IPEndPoint endPoint;
        public NetworkState state;

        public ServerClient(byte id, TcpClient socket)
        {
            this.id = id;
            this.socket = socket;
            state = NetworkState.notConnected;
        }
    }
}