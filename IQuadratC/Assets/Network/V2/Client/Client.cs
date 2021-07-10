using System.Collections.Generic;
using Network.V2.Both;
using UnityEngine;
using Utility;

namespace Network.V2.Client
{
    public class Client : MonoBehaviour
    {
        public PublicString ip;
        public PublicInt clientPort;
        public PublicInt serverPort;
        public PublicInt clientId;
        public PublicInt clientState;
        
        private delegate void PacketHandler(Packet packet);
        private static Dictionary<byte, PacketHandler> packetHandlers;

        public TCPClient tcpClient;
        public UDPClient udpClient;
        public ClientHandle clientHandle;
        public ClientSend clientSend;

        [SerializeField] private PublicEvent connectEvent;
        [SerializeField] private PublicEvent disconnectEvent;
        [SerializeField] private PublicEventString debugMessageEvent;
        
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

            connectEvent.Register(tcpClient.Connect);
            disconnectEvent.Register(Disconnect);
            debugMessageEvent.Register(clientSend.DebugMessage);

            clientState.value = (int) NetworkState.notConnected;
        }

        public void HandleData(byte[] data)
        {
            Packet packet = new Packet(data);
            packet.PrepareForRead();
            
            int length = packet.ReadInt32();
            if (length + 4 != data.Length)
            {
                Threader.RunOnMainThread(() =>
                {
                    Debug.Log("CLIENT: Packet size not correct.");
                });
                return;
            }
            
            byte serverId = packet.ReadByte();
            if (serverId != 1)
            {
                Threader.RunOnMainThread(() =>
                {
                    Debug.Log("CLIENT: Server ID not correct.");
                });
                return;
            }
            
            byte packetId = packet.ReadByte();

            Threader.RunOnMainThread(() =>
            {
                packetHandlers[packetId](packet);
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
            Debug.Log("CLIENT: Disconnecting...");
            clientState.value = (int) NetworkState.disconnecting;
            
            tcpClient.Disconnect();
            udpClient.Disconnect();

            Debug.Log("CLIENT: Disconnected");
            clientState.value = (int) NetworkState.notConnected;
        }
    }
}