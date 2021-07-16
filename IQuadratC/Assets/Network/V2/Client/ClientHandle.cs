using System;
using System.IO;
using Network.V2.Both;
using Pathfinding;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
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