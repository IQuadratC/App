using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Utility
{
    [CreateAssetMenu(fileName = "PublicInt2", menuName = "Utility/PublicInt2")]
    public class PublicInt2 : ScriptableObject, ISerializationCallbackReceiver
    {
        [NonSerialized] public int2 value;
        [SerializeField] private int2 initalValue;

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            /*
           value = new int2[initalValue.Length]
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