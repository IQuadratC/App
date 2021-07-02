using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using Utility;

namespace Network
{
    public class TCPServer : MonoBehaviour
    {
        class Client
        {
            public int id;
            public byte[] buffer;
            public Socket s;
            public StringBuilder sb;
            public NetworkState state;

            public Client(int id, Socket s)
            {
                this.id = id;
                buffer = new byte[bufferSize];
                this.s = s;
                sb = new StringBuilder();
                state = NetworkState.idle;
            }
        }
        
        [SerializeField] private PublicString ip;
        [SerializeField] private PublicInt tcpPort;
        
        [SerializeField] private int maxClients = 100;
        private int currentClients;

        private Client[] clients;
        
        private Socket s;
        private IPEndPoint ipe;
        [SerializeField] private NetworkState state;

        public const int bufferSize = 1024;

        [SerializeField] private PublicEvent  [SerializeField] private PublicEvent startServerEvent;;
        [SerializeField] private PublicEvent stopServerEvent;

        private void Start()
        {
            currentClients = 0;
            state = NetworkState.notConnected;
            clients = new Client[maxClients];

            startServerEvent.Register(() =>
            {
                Threader.RunAsync(StartServer);
            });
            
            startServerEvent.Register(() =>
            {
                Threader.RunAsync(StopServer);
            });

        }

        private void StartServer()
        {
            if (state != NetworkState.notConnected) {return;}
            
            s = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
        
            ipe = new IPEndPoint(IPAddress.Parse(ip.value),tcpPort.value);

            try
            {
                s.Bind(ipe);
                s.Listen(maxClients);
                state = NetworkState.idle;
                
                Threader.RunOnMainThread(() =>
                {
                    Debug.Log("TCP Server: Online");  
                });

                while (true)
                {
                    if (state != NetworkState.connecting)
                    { 
                        s.BeginAccept(AcceptCallback, null);
                        state = NetworkState.idle;
                    }
                    
                    for (int i = 0; i < clients.Length; i++)
                    {
                        Client client = clients[i];
                        if (client != null && !client.s.Connected)
                        {
                            client.s.Close();
                            clients[i] = null;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Threader.RunOnMainThread(() =>
                {
                    Debug.Log(e.ToString());  
                });
            }
        }
        
        public void AcceptCallback(IAsyncResult ar)
        {
            state = NetworkState.connecting;

            Client client = new Client(currentClients, s.EndAccept(ar));
            currentClients++;

            clients[client.id] = client;

            client.s.BeginReceive(client.buffer, 0, bufferSize, 0, ReciveCallback, client);
            client.state = NetworkState.reciving;
            
            Threader.RunOnMainThread(() =>
            {
                Debug.LogFormat("TCP Server: Client {0} from {1} connected.", client.id, client.s.RemoteEndPoint);
            });
        }
        public void ReciveCallback(IAsyncResult ar)
        {
            String content = String.Empty;  
            
            Client client = (Client) ar.AsyncState;
            
            int bytesRead = client.s.EndReceive(ar);  
  
            if (bytesRead > 0) {  
                // There  might be more data, so store the data received so far.  
                client.sb.Append(Encoding.ASCII.GetString(client.buffer, 0, bytesRead));  
  
                // Check for end-of-file tag. If it is not there, read
                // more data.  
                content = client.sb.ToString();  
                
                if (content.IndexOf("<EOF>") > -1) { 
                    
                    // All the data has been read from the
                    // client. Display it on the console.  
                    Debug.LogFormat("Read {0} bytes from socket. \n Data : {1}", content.Length, content );  
                    // Echo the data back to the client.  
                    Send(client, content);

                    client.state = NetworkState.idle;

                } else {  
                    
                    client.s.BeginReceive(client.buffer, 0, bufferSize, 0, ReciveCallback, client);
                    client.state = NetworkState.reciving;
                }  
            }  
        }
        
        private void Send(Client client, String data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            client.s.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, client);
            client.state = NetworkState.sending;
        }
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Client client = (Client) ar.AsyncState;  
                client.state = NetworkState.idle;
                
                int bytesSent = client.s.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);
            }
            catch (Exception e)
            {
                Threader.RunOnMainThread(() =>
                {
                    Debug.Log(e.ToString());  
                });
            }  
        }

        private void StopServer()
        {
            for (int i = 0; i < clients.Length; i++)
            {
                Client client = clients[i];
                if (client != null)
                {
                    
                    client.s.Shutdown(SocketShutdown.Both);  
                    client.s.Close();
                    clients[i] = null;
                }
            }
            state = NetworkState.notConnected;
            
            Threader.RunOnMainThread(() =>
            {
                Debug.Log("TCP Server: Stopped");  
            });
        }
    }
}