using System;
using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = "PublicIntArray", menuName = "Utility/PublicIntArray")]
    public class PublicIntArray : ScriptableObject, ISerializationCallbackReceiver
    {
        [NonSerialized] public int[] value;
        [SerializeField] private int[] initalValue;
        
        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            value = initalValue;
        }
    }
}