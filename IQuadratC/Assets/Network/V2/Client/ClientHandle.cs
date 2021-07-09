using Network.V2.Both;

namespace Network.V2.Client
{
    public class ClientHandle
    {
        private Client client;
        public ClientHandle(Client client)
        {
            this.client = client;
        }
        public void ServerConnection(Packet packet)
        {
            
        }
        
        public void DebugMessage(Packet packet)
        {
            
        }
    }
}