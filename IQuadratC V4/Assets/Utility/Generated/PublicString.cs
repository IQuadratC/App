using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Utility
{
    [CreateAssetMenu(fileName = "PublicString", menuName = "Utility/PublicString")]
    public class PublicString : ScriptableObject, ISerializationCallbackReceiver
    {
        [NonSerialized] public string value;
        [SerializeField] private string initalValue;

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            /*
           value = new string[initalValue.Length]
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