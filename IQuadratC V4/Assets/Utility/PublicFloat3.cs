using System;
using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = "Publicfloat3", menuName = "Utility/Publicfloat3")]
    public class PublicInt : ScriptableObject, ISerializationCallbackReceiver
    {
        [NonSerialized] public float3 value;
        [SerializeField] private float3 initalValue;
        
        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            value = initalValue;
        }
    }
}