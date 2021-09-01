using System.Threading;
using Network.Both;
using Unity.Mathematics;
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
            string version = "1.2";
            Debug.Log("CLIENT: sending settings: " +
                      "\nVersion " + version +
                      "\nUDP " + client.clientUdpSupport.value + 
                      "\nCam " + client.camSupport.value + 
                      "\nJoystick " + client.joystickSupport.value +
                      "\nChat " + client.chatSupport.value +
                      "\nLidar " + client.lidarSupport.value + 
                      "\nLidarSim " + client.lidarSimSupport.value
                      );
            
            using (Packet packet = new Packet((byte) Packets.clientSettings))
            {
                packet.Write(version);
                packet.Write(client.clientUdpSupport.value);
                packet.Write(client.camSupport.value);
                packet.Write(client.joystickSupport.value);
                packet.Write(client.chatSupport.value);
                packet.Write(client.lidarSupport.value);
                packet.Write(client.lidarSimSupport.value);
                
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
        
        public void ClientControllMode(int mode)
        {
            using (Packet packet = new Packet((byte) Packets.clientControllMode))
            {
                packet.Write(mode);
                client.SendTCPData(packet);
            }
        }
        
        public void ClientJoystickMove(float3 value)
        {
            using (Packet packet = new Packet((byte) Packets.clientJoystickMove))
            {
                packet.Write(value);
                client.SendUDPData(packet);
            }
        }
        public void ClientJoystickRotate(float value)
        {
            using (Packet packet = new Packet((byte) Packets.clientJoystickRotate))
            {
                packet.Write(value);
                client.SendUDPData(packet);
            }
        }
        
        public void ClientJoystickStop()
        {
            using (Packet packet = new Packet((byte) Packets.clientJoystickStop))
            {
                client.SendTCPData(packet);
            }
        }
        
        public void ClientLidarMode(int mode)
        {
            using (Packet packet = new Packet((byte) Packets.clientLidarMode))
            {
                packet.Write(mode);
                client.SendTCPData(packet);
            }
        }
        
        public void ClientGetSLAMMap()
        {
            using (Packet packet = new Packet((byte) Packets.clientGetSLAMMap))
            {
                client.SendTCPData(packet);
            }
        }
        
        public void ClientGetPosition()
        {
            using (Packet packet = new Packet((byte) Packets.clientGetPosition))
            {
                client.SendTCPData(packet);
            }
        }
        public void ClientSimulatedLidarData()
        {
            using (Packet packet = new Packet((byte) Packets.clientSimulatedLidarData))
            {
                packet.Write(client.lidarDataPolar.value.Length);
                for (int i = 0; i < client.lidarDataPolar.value.Length; i++)
                {
                    packet.Write(client.lidarDataPolar.value[i].y);
                }
                client.SendUDPData(packet);
            }
        }
    }
}