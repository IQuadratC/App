using System.Collections.Generic;
using Network.Both;
using Network.V2.Client;
using UnityEngine;
using Utility;

namespace Network.Client
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
        
        public PublicBool clientUdpSupport;
        [HideInInspector] public bool serverUdpSupport;
        [HideInInspector] public bool udpConnected;

        public PublicBool camSupport;
        public PublicBool serverCamSupport;
        
        public PublicBool joystickSupport;
        public PublicBool serverJoystickSupport;
        
        [SerializeField] private PublicEventInt controllModeEvent;
        [SerializeField] private PublicEventFloat3 joystickMoveEvent;
        [SerializeField] private PublicEventFloat joystickRotateEvent;
        [SerializeField] private PublicEvent joystickStopEvent;
        
        public PublicBool chatSupport;
        public PublicBool serverChatSupport;
        
        public PublicBool lidarSupport;
        public PublicBool serverLidarSupport;

        [SerializeField] private PublicEventInt lidarModeEvent;
        public PublicInt lidarMode;

        [SerializeField] private PublicEvent getSLAMMapEvent;
        public PublicByteArray SLAMMap;
        
        [SerializeField] private PublicEvent getPositionEvent;
        public PublicFloat3 position;

        public PublicBool lidarSimSupport;
        public PublicBool serverLidarSimSupport;
        public PublicFloat2Array lidarDataPolar;

        private void Awake()
        {
            tcpClient = new TCPClient(this);
            udpClient = new UDPClient(this);
            clientHandle = new ClientHandle(this);
            clientSend = new ClientSend(this);
            
            packetHandlers = new Dictionary<byte, PacketHandler>()
            {
                { (byte)Packets.debugMessage, clientHandle.DebugMessage },
                
                { (byte)Packets.serverSettings, clientHandle.ServerSettings },
                { (byte)Packets.serverStartUDP, clientHandle.ServerStartUDP },
                { (byte)Packets.serverUDPConnection, clientHandle.ServerUDPConnection },
                
                { (byte)Packets.serverLidarStatus, clientHandle.ServerLidarStatus },
                { (byte)Packets.servertSLAMMap, clientHandle.ServerSLAMMap },
                { (byte)Packets.serverPosition, clientHandle.ServerPosition },
                
                { (byte)Packets.serverGetSimulatedLidarData, clientHandle.ServerGetSimulatedLidarData },
            };

            connectEvent.Register(tcpClient.Connect);
            disconnectEvent.Register(Disconnect);
            debugMessageEvent.Register(clientSend.DebugMessage);
            
            controllModeEvent.Register(clientSend.ClientControllMode);
            joystickMoveEvent.Register(clientSend.ClientJoystickMove);
            joystickRotateEvent.Register(clientSend.ClientJoystickRotate);
            joystickStopEvent.Register(clientSend.ClientJoystickStop);

            lidarModeEvent.Register(clientSend.ClientLidarMode);
            getSLAMMapEvent.Register(clientSend.ClientGetSLAMMap);
            getPositionEvent.Register(clientSend.ClientGetPosition);
            
            clientState.value = (int) NetworkState.notConnected;
        }

        private void OnApplicationQuit()
        {
            Disconnect();
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
            if (!serverUdpSupport || !clientUdpSupport)
            {
                SendTCPData(packet);
                return;
            }
            
            packet = AddHeaderToPacket(packet);
            udpClient.SendData(packet.ToArray(), packet.Length());
        }

        public void Disconnect()
        {
            if (clientState.value != (int) NetworkState.connected) {return;}
            
            Debug.Log("CLIENT: Disconnecting...");
            clientState.value = (int) NetworkState.disconnecting;
            
            tcpClient.Disconnect();
            udpClient.Disconnect();

            Debug.Log("CLIENT: Disconnected");
            clientState.value = (int) NetworkState.notConnected;
        }
    }
}