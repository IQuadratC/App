using Network.V2.Both;

namespace Network.V2.Client
{
    public class ClientSend
    {
        private Client client;
        public ClientSend(Client client)
        {
            this.client = client;
        }
        
        public void ClientConnectionReceived()
        {
            
            
            using (Packet packet = new Packet((byte) Packets.clientConnectionRecived))
            {
                client.SendUDPData(packet);
            }
        }
        
        public void DebugMessage(string message)
        {
            using (Packet packet = new Packet((byte) Packets.debugMessage))
            {
                packet.Write(message);
                client.SendTCPData(packet);
            }
        }
    }
}