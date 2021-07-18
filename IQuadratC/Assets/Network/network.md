## TCP/UDP Package:
int -> data Length  
byte -> client/server ID  
byte -> Package Id  
bytes data

min length 6 byte  s
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

clientControllMode = 10, // 1 = no control, 1 = Joystick, 3 = AI
clientJoystickMove = 21, // dir norm Vec2, speed float
clientJoystickRotate = 22, // speed float, pos = right, neg = left
clientJoystickStop = 23, 

clientMoveAI = 25,

## StartUp Procedure
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

6. Server -> Client ServerStart