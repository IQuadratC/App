﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using Utility;

namespace Network.V1.Server
{
    public class Server : MonoBehaviour
    {
        public static Server instance;
        
        [SerializeField] public int maxClients;
        private const int clientSocketsOverlap = 10;
        
        [SerializeField] private PublicInt port;
        public Dictionary<int, Client> clients;
        public delegate void PacketHandler(int fromClient, Packet packet);
        public Dictionary<int, PacketHandler> packetHandlers;

        private TcpListener tcpListener;
        private UdpClient udpListener;

        public string ip;

        [SerializeField] private PublicEvent startServerEvent;
        [SerializeField] private PublicEvent stopServerEvent;
        [SerializeField] private PublicInt serverState;

        [SerializeField] private PublicEventString debugEvent;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Debug.Log("SERVER: Instance already exists, destroying object!");
                Destroy(this);
            }

            startServerEvent.Register(StartServer);
            stopServerEvent.Register(StopServer);
            serverState.value = (int) NetworkState.notConnected;
            
            debugEvent.Register(ServerSend.DebugMessage);
        }

        private void OnApplicationQuit()
        {
            StopServer();
        }

        /// <summary>Starts the server.</summary>
        private void StartServer()
        {
            if (serverState.value == (int) NetworkState.connected) { return; }
            
            ip = IPManager.GetIPAddress();
            Debug.Log(ip);

            Debug.Log("SERVER: Starting...");
            InitializeServerData();

            tcpListener = new TcpListener(IPAddress.Any, port.value);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(TcpConnectCallback, null);

            udpListener = new UdpClient(port.value);
            udpListener.BeginReceive(UdpReceiveCallback, null);

            serverState.value = (int) NetworkState.connected;
            Debug.Log($"SERVER: started on port {port.value}.");
        }

        /// <summary>Handles new TCP connections.</summary>
        private void TcpConnectCallback(IAsyncResult result)
        {
            TcpClient client = tcpListener.EndAcceptTcpClient(result);
            tcpListener.BeginAcceptTcpClient(TcpConnectCallback, null);
            Debug.Log($"SERVER: Incoming connection from {client.Client.RemoteEndPoint}...");

            for (int i = 1; i <= maxClients + clientSocketsOverlap; i++)
            {
                if (clients[i].tcp.socket != null) continue;
                clients[i].tcp.Connect(client);
                return;
            }

            Debug.Log($"SERVER: {client.Client.RemoteEndPoint} failed to connect: Server full!");
        }

        /// <summary>Receives incoming UDP data.</summary>
        private void UdpReceiveCallback(IAsyncResult result)
        {
            if (serverState.value == (int) NetworkState.notConnected) { return; }
            
            try
            {
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpListener.EndReceive(result, ref clientEndPoint);
                udpListener.BeginReceive(UdpReceiveCallback, null);

                if (data.Length < 2)
                {
                    return;
                }

                using (Packet packet = new Packet(data))
                {
                    byte clientId = packet.ReadByte();

                    if (clientId == 0)
                    {
                        return;
                    }

                    if (clients[clientId].udp.endPoint == null)
                    {
                        // If this is a new connection
                        clients[clientId].udp.Connect(clientEndPoint);
                        return;
                    }

                    if (clients[clientId].udp.endPoint.ToString() == clientEndPoint.ToString())
                    {
                        // Ensures that the client is not being impersonated by another by sending a false clientID
                        clients[clientId].udp.HandleData(packet);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"SERVER: Error receiving UDP data: {ex}");
            }
        }

        /// <summary>Sends a packet to the specified endpoint via UDP.</summary>
        /// <param name="clientEndPoint">The endpoint to send the packet to.</param>
        /// <param name="packet">The packet to send.</param>
        public void SendUdpData(IPEndPoint clientEndPoint, Packet packet)
        {
            if (serverState.value == (int) NetworkState.notConnected) { return; }
            
            try
            {
                if (clientEndPoint != null)
                {
                    udpListener.BeginSend(packet.ToArray(), packet.Length(), clientEndPoint, null, null);
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"SERVER: Error sending data to {clientEndPoint} via UDP: {ex}");
            }
        }

        /// <summary>Initializes all necessary server data.</summary>
        private void InitializeServerData()
        {
            clients = new Dictionary<int, Client>();
            
            for (int i = 1; i <= maxClients + clientSocketsOverlap; i++)
            {
                clients.Add(i, new Client(i));
            }

            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int) Packets.clientConnectionRecived, ServerHandle.ClientConnectionRecived },
                { (int) Packets.debugMessage, ServerHandle.DebugMessage },
            };
        }
        
        public void StopServer()
        {
            if (serverState.value == (int) NetworkState.notConnected) { return; }
            
            foreach (KeyValuePair<int,Client> keyValuePair in clients)
            {
                if (keyValuePair.Value.tcp.socket != null)
                {
                    keyValuePair.Value.Disconnect();
                }
            }
            
            tcpListener.Stop();
            udpListener.Close();

            clients = null;
            
            serverState.value = (int) NetworkState.notConnected;
            Debug.Log("SERVER: Stopped.");
        }
    }
}