using System.Collections.Generic;
using Network.V2.Both;
using UnityEngine;
using Utility;

namespace Network.V2.Client
{
    public class Client : MonoBehaviour
    {
        public PublicString ip;
        public PublicInt port;
        public PublicInt clientId;
        public PublicInt clientState;
        
        private delegate void PacketHandler(Packet packet);
        private static Dictionary<byte, PacketHandler> packetHandlers;

        private TCPClient tcpClient;
        private UDPClient udpClient;
        private ClientHandle clientHandle;
        private ClientSend clientSend;
        
        private void Awake()
        {
            tcpClient = new TCPClient(this);
            udpClient = new UDPClient(this);
            clientHandle = new ClientHandle(this);
            clientSend = new ClientSend(this);
            
            packetHandlers = new Dictionary<byte, PacketHandler>()
            {
                { (byte)Packets.serverConnection, clientHandle.ServerConnection },
                { (byte)Packets.debugMessage, clientHandle.DebugMessage },
            };
        }

        public void HandleData(byte[] data)
        {
            using (Packet packet = new Packet(data))
            {
                int packetLength = packet.ReadInt32();
                data = packet.ReadBytes(packetLength);
            }

            Threader.RunOnMainThread(() =>
            {
                using (Packet packet = new Packet(data))
                {
                    byte packetId = packet.ReadByte();
                    packetHandlers[packetId](packet);
                }
            });
        }

        private Packet AddHeaderToPacket(Packet packet)
        {
            packet.Write(0, (byte) clientId.value);
            packet.Write(0, packet.Length());
            return packet;
        }

        public void SendTCPData(Packet packet)
        {
            packet = AddHeaderToPacket(packet);
            tcpClient.SendData(packet.ToArray(), packet.Length());
        }
        
        public void SendUDPData(Packet packet)
        {
            packet = AddHeaderToPacket(packet);
            udpClient.SendData(packet.ToArray(), packet.Length());
        }

        public void Disconnect()
        {
            tcpClient.Disconnect();
            udpClient.Disconnect();

            clientState.value = (int) NetworkState.notConnected;
        }
    }
}