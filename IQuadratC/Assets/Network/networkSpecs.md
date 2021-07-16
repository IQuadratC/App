## TCP/UDP Package:
int -> data Length  
byte -> client/server ID  
byte -> Package Id  
bytes data

min length 6 byte  
server Id = 1;

## ids:
debugMessage = 1,

serverSettings = 2,  
clientSettings = 3,
serverStartUDP = 4,
clientUDPConnection = 5,
serverUDPConnection = 6,
clientUDPConnectionStatus = 7,

serverCamImage = 8,

## Procedure
0. Awake  
    clientState = notConnected  
    serverState = notConnected  

1. Server Start  
   serverState = Connected  
   server TCP Connection opened  
   server UDP Connection opened  
   
--- Server Online ---

2. ClientConnect (TCP)  
   clientState = Connecting  
   
3. TCP Connected  
   clientState = Connected  
   
4. Server -> Client ServerSettings (TCP)  
    clientId send from server to client
    settings:
        udp support bool
   
5.  Server <- Client ClientSettings (TCP)
    settings:
        udp support bool
    
6. Server -> Client ServerStartUDP (TCP) 
    
7. Server <- Client ClientUDPConnection (UDP) 
    server client endpoint mapped 
   
8. Server -> Client ServerUDPConnection (UDP) / (TCP) 
    udp from Client recived bool 

9. Server <- Client ClientUDPConnectionStatus (TCP) 
   udp from Server recived bool
   
--- Connected ---
