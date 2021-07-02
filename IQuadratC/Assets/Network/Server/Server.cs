using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using SharedFiles.Utility;
using UnityEngine;
using Utility;

namespace Network.Server
{
    public class Server : MonoBehaviour
    {
        public static Server instance;
        
        [SerializeField] public int maxClients;
        private const int clientSocketsOverlap = 10;
        
        [SerializeField] private PublicInt port;
        public Dictionary<int, Client> clients = new Dictionary<int, Client>();
        public delegate void PacketHandler(int fromClient, Packet packet);
        public Dictionary<int, PacketHandler> packetHandlers;

        private TcpListener tcpListener;
        private UdpClient udpListener;

        public string ip;

        [SerializeField] private PublicEvent startServerEvent;
        [SerializeField] private PublicEvent stopServerEvent;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Debug.Log("Instance already exists, destroying object!");
                Destroy(this);
            }

            startServerEvent.Register(StartServer);
            stopServerEvent.Register(StopServer);
        }

        /// <summary>Starts the server.</summary>
        private void StartServer()
        {
            ip = IPManager.GetIPAddress();
            Debug.Log(ip);

            Debug.Log("Starting server...");
            InitializeServerData();

            tcpListener = new TcpListener(IPAddress.Any, port.value);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(TcpConnectCallback, null);

            udpListener = new UdpClient(port.value);
            udpListener.BeginReceive(UdpReceiveCallback, null);

            Debug.Log($"Server started on port {port.value}.");
        }

        /// <summary>Handles new TCP connections.</summary>
        private void TcpConnectCallback(IAsyncResult result)
        {
            TcpClient client = tcpListener.EndAcceptTcpClient(result);
            tcpListener.BeginAcceptTcpClient(TcpConnectCallback, null);
            Debug.Log($"Incoming connection from {client.Client.RemoteEndPoint}...");

            for (int i = 1; i <= maxClients + clientSocketsOverlap; i++)
            {
                if (clients[i].tcp.socket != null) continue;
                clients[i].tcp.Connect(client);
                return;
            }

            Debug.Log($"{client.Client.RemoteEndPoint} failed to connect: Server full!");
        }

        /// <summary>Receives incoming UDP data.</summary>
        private void UdpReceiveCallback(IAsyncResult result)
        {
            try
            {
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpListener.EndReceive(result, ref clientEndPoint);
                udpListener.BeginReceive(UdpReceiveCallback, null);

                if (data.Length < 4)
                {
                    return;
                }

                using (Packet packet = new Packet(data))
                {
                    int clientId = packet.ReadInt();

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
                Debug.Log($"Error receiving UDP data: {ex}");
            }
        }

        /// <summary>Sends a packet to the specified endpoint via UDP.</summary>
        /// <param name="clientEndPoint">The endpoint to send the packet to.</param>
        /// <param name="packet">The packet to send.</param>
        public void SendUdpData(IPEndPoint clientEndPoint, Packet packet)
        {
            try
            {
                if (clientEndPoint != null)
                {
                    udpListener.BeginSend(packet.ToArray(), packet.Length(), clientEndPoint, null, null);
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Error sending data to {clientEndPoint} via UDP: {ex}");
            }
        }

        /// <summary>Initializes all necessary server data.</summary>
        private void InitializeServerData()
        {
            for (int i = 1; i <= maxClients + clientSocketsOverlap; i++)
            {
                clients.Add(i, new global::Network.Server.Client(i));
            }

            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ClientPackets.gameEnterRequest, ServerHandle.GameEnterReqest },
            };
            Debug.Log("Initialized packets.");
        }
        
        public void StopServer()
        {
            tcpListener.Stop();
            udpListener.Close();
        }
    }
}
