using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Utility
{
    [CreateAssetMenu(fileName = "PublicFloat", menuName = "Utility/PublicFloat")]
    public class PublicFloat : ScriptableObject, ISerializationCallbackReceiver
    {
        [NonSerialized] public float value;
        [SerializeField] private float initalValue;

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            /*
           value = new float[initalValue.Length]
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