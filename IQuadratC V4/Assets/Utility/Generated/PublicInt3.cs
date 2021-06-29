using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Utility
{
    [CreateAssetMenu(fileName = "PublicInt3", menuName = "Utility/PublicInt3")]
    public class PublicInt3 : ScriptableObject, ISerializationCallbackReceiver
    {
        [NonSerialized] public int3 value;
        [SerializeField] private int3 initalValue;

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            /*
           value = new int3[initalValue.Length]
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