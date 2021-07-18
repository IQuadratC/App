﻿using System.Collections.Generic;
using Network.Both;
using UnityEngine;
using Utility;

namespace Network.Server
{
    public class Server : MonoBehaviour
    {
        public PublicInt port;
        public PublicInt serverState;
        
        private delegate void PacketHandler(ServerClient client, Packet packet);
        private static Dictionary<byte, PacketHandler> packetHandlers;

        public TCPServer tcpServer;
        public UDPServer udpServer;
        public ServerHandle serverHandle;
        public ServerSend serverSend;
        
        public ServerClient[] clients;
        
        [SerializeField] private PublicEvent startServerEvent;
        [SerializeField] private PublicEvent stopServerEvent;
        [SerializeField] private PublicEventString debugMessageEvent;

        [SerializeField] public PublicInt controllMode;
        [SerializeField] public PublicEventFloat3 joystickMoveEvent;
        [SerializeField] public PublicEventFloat joystickRotateEvent;
        [SerializeField] public PublicEvent joystickStopEvent;

        public bool serverUdpSupport;

        private void Awake()
        {
            tcpServer = new TCPServer(this);
            udpServer = new UDPServer(this);
            serverHandle = new ServerHandle(this);
            serverSend = new ServerSend(this);
            clients = new ServerClient[State.MaxClients + 1];
            
            packetHandlers = new Dictionary<byte, PacketHandler>()
            {
                { (byte)Packets.debugMessage, serverHandle.DebugMessage },
                
                { (byte)Packets.clientSettings, serverHandle.ClientSettings },
                { (byte)Packets.clientUDPConnection, serverHandle.ClientUDPConnection },
                { (byte)Packets.clientUDPConnectionStatus, serverHandle.ClientUDPConnectionStatus },
                
                { (byte)Packets.clientControllMode, serverHandle.ClientControllMode },
                { (byte)Packets.clientJoystickMove, serverHandle.ClientJoystickMove },
                { (byte)Packets.clientJoystickRotate, serverHandle.ClientJoystickRotate },
                { (byte)Packets.clientJoystickStop, serverHandle.ClientJoystickStop },
            };

            startServerEvent.Register(StartServer);
            stopServerEvent.Register(StopServer);
            debugMessageEvent.Register(serverSend.DebugMessage);

            serverState.value = (int) NetworkState.notConnected;
        }

        private void OnApplicationQuit()
        {
            StopServer();
        }

        public void StartServer()
        {
            if (serverState.value != (int) NetworkState.notConnected) { return; }
            
            Debug.Log("SERVER: Starting...");
            serverState.value = (int) NetworkState.connecting;
            
            tcpServer.Start();
            if (serverUdpSupport)
            {
                udpServer.Start();
            }

            Debug.Log("SERVER: Started");
            serverState.value = (int) NetworkState.connected;
        }

        public void ConnectClient(ServerClient client)
        {
            Debug.Log($"SERVER: Connecting Client " + client.id + "...");
            client.state = NetworkState.connecting;
            tcpServer.ConnectClient(client);
        }

        public void HandelData(byte[] data, ServerClient client = null)
        {
            Packet packet = new Packet(data);
            packet.PrepareForRead();
            
            int length = packet.ReadInt32();
            if (length + 4 != data.Length)
            {
                Threader.RunOnMainThread(() =>
                {
                    Debug.Log("SERVER: Packet size not correct.");
                });
                return;
            }
            
            byte clientId = packet.ReadByte();
            if (clientId == 0 || client != null && client.id != clientId)
            {
                Threader.RunOnMainThread(() =>
                {
                    Debug.Log("CLIENT: Server ID not correct.");
                });
                return;
            } 
            if(clientId != 0 && client == null)
            {
                client = clients[clientId];
            }
            
            byte packetId = packet.ReadByte();
            
            Threader.RunOnMainThread(() =>
            {
                packetHandlers[packetId](client, packet);
            });
        }

        private Packet AddHeaderToPacket(Packet packet)
        {
            packet.Write(0, (byte) 1);
            packet.Write(0, packet.Length());
            return packet;
        }

        public void SendTCPData(ServerClient client, Packet packet)
        {
            packet = AddHeaderToPacket(packet);
            tcpServer.SendData(client, packet.ToArray(), packet.Length());
        }
        public void SendTCPDataToAll(Packet packet)
        {
            packet = AddHeaderToPacket(packet);
            foreach (ServerClient client in clients)
            {
                if (client != null)
                {
                    tcpServer.SendData(client, packet.ToArray(), packet.Length());
                }
            }
        }
        
        public void SendUDPData(ServerClient client, Packet packet)
        {
            if (!serverUdpSupport || !client.clientUdpSupport)
            {
                SendTCPData(client, packet);
                return;
            }
            
            packet = AddHeaderToPacket(packet);
            udpServer.SendData(client, packet.ToArray(), packet.Length());
        }
        public void SendUDPDataToAll(Packet packet)
        {
            packet = AddHeaderToPacket(packet);
            foreach (ServerClient client in clients)
            {
                if (client != null)
                {
                    if (!serverUdpSupport || !client.clientUdpSupport)
                    {
                        SendTCPData(client, packet);
                        continue;
                    }
                    
                    udpServer.SendData(client, packet.ToArray(), packet.Length());
                }
            }
        }
        
        public void DisconnectClient(ServerClient client)
        {
            client.state = NetworkState.disconnecting;
            
            tcpServer.DisconnectClient(client);
            udpServer.DisconnectClient(client);
            clients[client.id] = null;
            
            Debug.Log("SERVER: Client " + client.id + " disconnected.");
        }

        public void StopServer()
        {
            if (serverState.value != (int) NetworkState.connected) { return; }
            
            Debug.Log("SERVER: Stopping...");
            serverState.value = (int) NetworkState.disconnecting;

            foreach (ServerClient client in clients)
            {
                if (client != null)
                {
                    DisconnectClient(client);
                }
            }
            
            tcpServer.Stop();
            udpServer.Stop();

            Debug.Log("SERVER: Stopped");
            serverState.value = (int) NetworkState.notConnected;
        }
    }
}