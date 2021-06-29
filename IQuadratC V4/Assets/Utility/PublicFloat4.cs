using System;
using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = "Publicfloat4", menuName = "Utility/Publicfloat4")]
    public class PublicInt : ScriptableObject, ISerializationCallbackReceiver
    {
        [NonSerialized] public float4 value;
        [SerializeField] private float4 initalValue;
        
        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            value = initalValue;
        }
    }
}