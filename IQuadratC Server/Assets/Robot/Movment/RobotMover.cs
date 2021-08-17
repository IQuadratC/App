using System;
using Network.Both;
using Unity.Mathematics;
using UnityEngine;
using Utility;

public class RobotMover : MonoBehaviour
{
    
    [SerializeField] public PublicInt controllMode;
    [SerializeField] private PublicEventFloat3 joystickMoveEvent;
    [SerializeField] private PublicEventFloat joystickRotateEvent;
    [SerializeField] private PublicEvent joystickStopEvent;

    private void Awake()
    {
        joystickMoveEvent.Register((value) =>
        {
            Debug.Log("Move " + value);
            move = value;
            lastJoystickupdate = Time.time;
        });
        
        joystickRotateEvent.Register((value) =>
        {
            Debug.Log("Rotate " + value);
            rotate = value;
            lastJoystickupdate = Time.time;
        });
        
        joystickStopEvent.Register(() =>
        {
            move = float3.zero;
            rotate = 0;
            lastJoystickupdate = Time.time;
        });
    }

    private float lastJoystickupdate;
    private float3 move;
    private float rotate;
    private void Update()
    {
        if (controllMode.value == (int) ControllMode.joystick)
        {
            float timeNow = Time.time;

            if (lastJoystickupdate + State.MaxJoystickSendIntervall < timeNow)
            {
                joystickStopEvent.Raise();
                return;
            }

            transform.position += (Vector3) new float3(move.xy * move.z * Time.deltaTime, 0).xzy;
            transform.Rotate(Vector3.up, rotate * Mathf.Rad2Deg * Time.deltaTime);
        }
    }
}
