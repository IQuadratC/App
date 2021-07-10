## TCP/UDP Package:
int -> data Length  
byte -> client/server ID  
byte -> Package Id  
bytes data

min length 6 byte  
server Id = 1;

## ids:
serverConnection = 1,  
clientConnectionRecived = 2,  
debugMessage = 3,  
debugImage = 4,


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
   
4. Server -> Client ServerConnection (TCP)  
    clientId send from server to client  
    client UDP Connection opened  
   
5. Server <- Client ClientConnectionReceived (UDP)  
    server client endpoint mapped  
   
--- Connected ---
