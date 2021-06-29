using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Utility
{
    [CreateAssetMenu(fileName = "PublicFloat2", menuName = "Utility/PublicFloat2")]
    public class PublicFloat2 : ScriptableObject, ISerializationCallbackReceiver
    {
        [NonSerialized] public float2 value;
        [SerializeField] private float2 initalValue;

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            /*
           value = new float2[initalValue.Length]
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