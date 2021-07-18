using System.Threading;
using Network.Both;
using UnityEngine;
using Utility;

namespace Network.V2.Client
{
    public class ClientSend
    {
        private Network.Client.Client client;
        public ClientSend(Network.Client.Client client)
        {
            this.client = client;
        }
        
        public void DebugMessage(string message)
        {
            using (Packet packet = new Packet((byte) Packets.debugMessage))
            {
                packet.Write(message);
                client.SendTCPData(packet);
            }
        }
        
        public void ClientSettings()
        {
            Debug.Log("CLIENT: sending settings");
            using (Packet packet = new Packet((byte) Packets.clientSettings))
            {
                packet.Write(client.clientUdpSupport);
                
                client.SendTCPData(packet);
            }
        }
        public void ClientUDPConnection()
        {
            Debug.Log("CLIENT: udp test message");
            using (Packet packet = new Packet((byte) Packets.clientUDPConnection))
            {
                client.SendUDPData(packet);
            }
            
            Threader.RunAsync(() =>
            {
                Thread.Sleep(2000);
                if (client.udpConnected) return;
                
                client.serverUdpSupport = false;
                ClientUDPConnectionStatus();
            });
        }
        public void ClientUDPConnectionStatus()
        {
            client.udpConnected = client.clientUdpSupport && client.serverUdpSupport;
            Debug.Log("CLIENT: UDP connection status: "+ client.udpConnected);
            using (Packet packet = new Packet((byte) Packets.clientUDPConnectionStatus))
            {
                packet.Write(client.udpConnected);
                client.SendTCPData(packet);
            }
        }
    }
}