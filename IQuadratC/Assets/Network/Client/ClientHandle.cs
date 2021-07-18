using Network.Both;
using UnityEngine;
using Utility;

namespace Network.Client
{
    public class ClientHandle
    {
        private Network.Client.Client client;
        public ClientHandle(Network.Client.Client client)
        {
            this.client = client;
        }
        
        public void DebugMessage(Packet packet)
        {
            string message = packet.ReadString();
            Threader.RunOnMainThread(() =>
            {
                Debug.Log("CLIENT: Debug: "+ message);
            });
        }
        
        public void ServerSettings(Packet packet)
        {
            Debug.LogFormat("CLIENT: recived server settings");
            client.clientId.value = packet.ReadByte();
            client.serverUdpSupport = packet.ReadBool();
            
            client.clientSend.ClientSettings();
        }
        
        public void ServerStartUDP(Packet packet)
        {
            client.udpClient.Connect();
            client.clientSend.ClientUDPConnection();
        }
        
        public void ServerUDPConnection(Packet packet)
        {
            client.clientSend.ClientUDPConnectionStatus();
        }
        
        public void ServerCamImage(Packet packet)
        {
            byte[] data = packet.ReadBytes(packet.Length() - 14);
            
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(data);
            client.imageRenderer.material.mainTexture = tex;
        }
    }
}