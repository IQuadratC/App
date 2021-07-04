using SharedFiles.Utility;
using UnityEngine;

namespace Network.Server
{
    public static class ServerHandle
    {
        public static void ClientConnectionRecived(int fromClient, Packet packet)
        {
            int clientIdCheck = packet.ReadInt();
            
            Debug.Log($"SERVER: {Server.instance.clients[fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now client {fromClient}.");
        }
        
        public static void ClientDisconnect(int fromClient, Packet packet)
        {
            int clientIdCheck = packet.ReadInt();
            
            Debug.Log($"SERVER: {fromClient} disconnected. ");
            
            Server.instance.clients[fromClient].Disconnect();
        }
    }
}
