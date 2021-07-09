using UnityEngine;

namespace Network.V1.Server
{
    public static class ServerHandle
    {
        public static void ClientConnectionRecived(int fromClient, Packet packet)
        {
            Debug.Log($"SERVER: {V1.Server.Server.instance.clients[fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now client {fromClient}.");
        }
        public static void DebugMessage(int fromClient, Packet packet)
        {
            string message = packet.ReadString();
            Debug.Log("SERVER: Client "+ fromClient +" [Debug] " + message);
        }
    }
}
