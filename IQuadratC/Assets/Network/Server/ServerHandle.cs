using SharedFiles.Utility;
using UnityEngine;

namespace Network.Server
{
    public static class ServerHandle
    {
        public static void GameEnterReqest(int fromClient, Packet packet)
        {
            int clientIdCheck = packet.ReadInt();
            
            Debug.Log($"{Server.instance.clients[fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {fromClient}.");
        }
    }
}
