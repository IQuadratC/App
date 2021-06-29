using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Utility
{
    [CreateAssetMenu(fileName = "PublicFloat3", menuName = "Utility/PublicFloat3")]
    public class PublicFloat3 : ScriptableObject, ISerializationCallbackReceiver
    {
        [NonSerialized] public float3 value;
        [SerializeField] private float3 initalValue;

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            /*
           value = new float3[initalValue.Length]
           for (int i = 0; i < initalValue.Length; i++)
           {
               value[i] = initalValue[i];
           }
           */
            
            value = initalValue;
        }
        
        // Debug
        [SerializeField] private string description;
    }
}