using Unity.Mathematics;
using UnityEngine;
using Utility;

public class JoystickSender : MonoBehaviour
{
    [SerializeField] private PublicFloat2 joystickDir;
    [SerializeField] private PublicFloat joystickSpeed;
    [SerializeField] private PublicFloat joystickRotate;
    
    [SerializeField] private PublicEventFloat3 joystickMoveEvent;
    [SerializeField] private PublicEventFloat joystickRotateEvent;
    [SerializeField] private PublicEvent joystickStopEvent;

    private float lastSendTime;
    [SerializeField] private float sendIntervall = 0.5f;

    private void Update()
    {
        float time = Time.time;
        if (lastSendTime + sendIntervall >= time)
        {
            return;
        }
        lastSendTime = time;
        
        if (joystickDir.value.x != 0 || joystickDir.value.y != 0)
        {
            joystickMoveEvent.Raise(new float3(joystickDir.value, joystickSpeed.value));
        }else if (joystickRotate.value != 0)
        {
            joystickRotateEvent.Raise(joystickRotate.value);
        }
        else
        {
            joystickStopEvent.Raise();
        }
    }
}
