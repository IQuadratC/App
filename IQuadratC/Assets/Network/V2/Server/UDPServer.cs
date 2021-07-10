using System;
using System.Net;
using System.Net.Sockets;
using Network.V2.Both;
using UnityEngine;

namespace Network.V2.Server
{
    public class UDPServer
    {
        private Server server;
        public UDPServer(Server server)
        {
            this.server = server;
        }
        
        private UdpClient udpListener;
        
        public void Start()
        {
            if (server.serverState.value != (int) NetworkState.connecting) { return; }
        
            udpListener = new UdpClient(server.port.value);
            udpListener.BeginReceive(UdpReceiveCallback, null);
        }
        
        private void UdpReceiveCallback(IAsyncResult result)
        {
            if (server.serverState.value != (int) NetworkState.connected) { return; }
            
            try
            {
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpListener.EndReceive(result, ref clientEndPoint);
                udpListener.BeginReceive(UdpReceiveCallback, null);

                if (data.Length < 6)
                {
                    return;
                }

                ServerClient client = server.HandelData(data);
                if (client != null && server.clients[client.id].endPoint == null)
                {
                    server.clients[client.id].endPoint = clientEndPoint;
                }
                
            }
            catch (Exception ex)
            {
                Debug.Log($"SERVER: Error receiving UDP data: {ex}");
            }
        }

        public void SendData(ServerClient client, byte[] data, int length)
        {
            if (server.serverState.value != (int) NetworkState.connected) { return; }
            
            try
            {
                if (client.endPoint != null)
                {
                    udpListener.BeginSend(data, length, client.endPoint, null, null);
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"SERVER: Error sending data to {client.endPoint} via UDP: {ex}");
            }
        }

        public void DisconnectClient(ServerClient client)
        {
            client.endPoint = null;
        }

        public void Stop()
        {
            udpListener.Close();
        }
    }
}