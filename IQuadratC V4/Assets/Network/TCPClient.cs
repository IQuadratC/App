using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Network;
using UnityEngine;
using Utility;

public class TCPClient : MonoBehaviour
{
    [SerializeField] private PublicString ip;
    [SerializeField] private PublicInt tcpPort;

    private Socket s;
    private IPEndPoint ipe;
    [SerializeField] private NetworkState state;
    
    private const int bufferSize = 1024;
    private byte[] buffer;
    private StringBuilder sb;

    private void Start()
    {
        state = NetworkState.notConnected;
        
        buffer = new byte[bufferSize];
        sb = new StringBuilder();
        
        Connect();
    }

    private void OnDisable()
    {
        Disconnect();
    }

    private void Connect()
    {
        s = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);
        
        ipe = new IPEndPoint(IPAddress.Parse(ip.value),tcpPort.value);
        
        s.BeginConnect(ipe, ConnectCallback, s);
        
        state = NetworkState.connecting;
    }
    private void ConnectCallback(IAsyncResult ar)
    {
        try {
            s.EndConnect(ar);
            state = NetworkState.idle;
  
            Debug.LogFormat("TCP Client: Connected to {0}", s.RemoteEndPoint.ToString());

        } catch (Exception e) {  
            Debug.Log(e.ToString());  
        }  
    }
    
    private void Send(byte[] data) {
        
        s.BeginSend(data, 0, data.Length, SocketFlags.None,  
            SendCallback, s);  
        
        state = NetworkState.sending;
    }
    private void SendCallback(IAsyncResult ar) {  
        try {
            
            int bytesSent = s.EndSend(ar);  
            state = NetworkState.idle;
            
            Debug.LogFormat("TCP Client: Sent {0} bytes to server.", bytesSent);
            
        } catch (Exception e) {  
            Debug.Log(e.ToString());  
        }  
    }

    private void Receive() {  
        try {
            s.BeginReceive(buffer, 0, bufferSize, 0, ReceiveCallback, state);
            
            state = NetworkState.reciving;
            
        } catch (Exception e) {  
            Debug.Log(e.ToString());  
        }  
    }
    
    private void ReceiveCallback( IAsyncResult ar ) {  
        try {
            int bytesRead = s.EndReceive(ar);  
            if (bytesRead > 0) {
                
                s.BeginReceive(buffer,0,bufferSize,0, ReceiveCallback, state);  
                state = NetworkState.reciving;
                
            } else {  
                // All the data has arrived; put it in response.  
                if (sb.Length > 1) {
                    state = NetworkState.idle;

                    TCPMessage.ParseMessage(buffer);
                    buffer = new byte[0];
                }
            }  
        } catch (Exception e) {  
            Debug.Log(e.ToString());  
        }  
    }

    private void Disconnect()
    {
        s.Shutdown(SocketShutdown.Both);  
        s.Close();
        s = null;
        state = NetworkState.notConnected;
        
        Threader.RunOnMainThread(() =>
            {
                Debug.Log("TCP Client: Disconnected.");  
            });
    }
}


