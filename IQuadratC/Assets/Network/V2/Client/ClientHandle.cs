using Network.V2.Both;
using UnityEngine;
using Utility;

namespace Network.V2.Client
{
    public class ClientHandle
    {
        private Client client;
        public ClientHandle(Client client)
        {
            this.client = client;
        }
        public void ServerConnection(Packet packet)
        {
            byte myId = packet.ReadByte();
            client.clientId.value = myId;
            
            Debug.Log($"CLIENT: ID recived. ID is " + myId);
            
            client.udpClient.Connect();
            client.clientSend.ClientConnectionReceived();
        }
        
        public void DebugMessage(Packet packet)
        {
            string message = packet.ReadString();
            Threader.RunOnMainThread(() =>
            {
                Debug.Log("CLIENT: Debug: "+ message);
            });
        }
    }
}