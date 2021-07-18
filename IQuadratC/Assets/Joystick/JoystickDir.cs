using UnityEngine;
using Unity.Mathematics;
using UnityEngine.EventSystems;
using Utility;

public class JoystickDir : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    
    [SerializeField] private Transform stick;
    [SerializeField] private float maxDistance;
    
    [SerializeField] private PublicFloat2 joystickDir;
    
    private bool pressed;
    private float3 lastPos;
    private float2 fingerPos;
    
    public void OnPointerDown(PointerEventData eventData){
        lastPos = Input.mousePosition;
        pressed = true;
    }
     
    public void OnPointerUp(PointerEventData eventData){
        pressed = false;
    }

    public void Update()
    {
        if (pressed)
        {
            // calculate finger position
            if (Input.touchCount > 0)
            {
                fingerPos += (float2)Input.touches[0].deltaPosition;
            }
            else
            {
                float3 pos = Input.mousePosition;
                fingerPos += (pos.xy - lastPos.xy);;
                lastPos = pos;
            }


            // format point to maxDistance 
            if (math.length(fingerPos.xy) > maxDistance)
            {
                joystickDir.value = math.normalize(fingerPos);
                stick.localPosition = new float3(math.normalize(fingerPos), 0) * maxDistance;
            }
            else
            {
                joystickDir.value = (fingerPos) / maxDistance;
                stick.localPosition = new float3(fingerPos, 0);
            }
        }
        else
        {
            // resets the stick position
            joystickDir.value = float2.zero;
            stick.localPosition = Vector3.zero;
            fingerPos = float2.zero;
        }
    }
}
