using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Utility
{
    [CreateAssetMenu(fileName = "PublicBool", menuName = "Utility/PublicBool")]
    public class PublicBool : ScriptableObject, ISerializationCallbackReceiver
    {
        [NonSerialized] public bool value;
        [SerializeField] private bool initalValue;

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            /*
           value = new bool[initalValue.Length]
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