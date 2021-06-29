using System;
using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = "Publicfloat2", menuName = "Utility/Publicfloat2")]
    public class PublicInt : ScriptableObject, ISerializationCallbackReceiver
    {
        [NonSerialized] public float2 value;
        [SerializeField] private float2 initalValue;
        
        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            value = initalValue;
        }
    }
}