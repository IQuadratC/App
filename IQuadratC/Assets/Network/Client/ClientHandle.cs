using System.Net;
using Player.Trooper.LocalTrooper;
using SharedFiles.Utility;
using UI;
using UnityEngine;

namespace Server
{
    public class ClientHandle : MonoBehaviour
    {
        public static void ServerConnection(Packet packet)
        {
            int myId = packet.ReadInt();

            Debug.Log("Established TCP connection");
            Client.instance.myId = myId;
            ClientSend.ServerConnectionReceived();

            // Now that we have the client's id, connect UDP
            Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
        }
        
        public static void GameEnterRejected(Packet packet)
        {
            string message = packet.ReadString();
            Debug.Log(message);
            
            Client.instance.Disconnect();
            UIManager.instance.SwitchPanel(PanelType.startPanel);
        }
        
        public static void GameStateChange(Packet packet)
        {
            GameModeType gameModeType = (GameModeType) packet.ReadInt();
            string lobbyName = packet.ReadString();

            GameManager.instance.gameModeType = gameModeType;
            GameManager.instance.LobbyChange(lobbyName);
        }

        public static void PlayerEnter(Packet packet)
        {
            int id = packet.ReadInt();
            string username = packet.ReadString();

            GameManager.instance.PlayerEnter(id, username);
        }

        public static void PlayerLeave(Packet packet)
        {
            int id = packet.ReadInt();
            
            GameManager.players.Remove(id);
        }
        
        public static void PlayerState(Packet packet)
        {
            int id = packet.ReadInt();
            PlayerState state = (PlayerState) packet.ReadInt();

            GameManager.instance.PlayerStateChanged(id, state);
        }

        public static void TrooperTransformUpdate(Packet packet)
        {
            int id = packet.ReadInt();
            Vector3 position = packet.ReadVector3();
            Quaternion rotation = packet.ReadQuaternion();
            Vector3 velocity = packet.ReadVector3();
            bool grounded = packet.ReadBool();

            GameManager.players[id].trooper.transform.position = position;
            GameManager.players[id].trooper.transform.rotation = rotation;
            GameManager.players[id].trooper.velocity = velocity;
            GameManager.players[id].trooper.grounded = grounded;

            if (id != Client.instance.myId) return;
            TrooperController.instance.rigidbody.velocity = velocity;
            TrooperController.instance.grounded = grounded;
        }
        
        public static void TrooperGrappleUpdate(Packet packet)
        {
            int id = packet.ReadInt();
            bool isGrappling = packet.ReadBool();
            string objectId = packet.ReadString();
            Vector3 position = packet.ReadVector3();
            float maxDistanceFromGrapple = packet.ReadFloat();

            GameManager.players[id].trooper.isGrappling = isGrappling;
            GameManager.players[id].trooper.grappleObjectId = objectId;
            GameManager.players[id].trooper.grapplePoint = position;
            GameManager.players[id].trooper.maxDistanceFromGrapple = maxDistanceFromGrapple;

            if (id != Client.instance.myId) return;
            GrapplingGun.instance.grappling = isGrappling;
            GrapplingGun.instance.grapplePoint = position;
            GrapplingGun.instance.maxDistance = maxDistanceFromGrapple;
        }
    }
}
