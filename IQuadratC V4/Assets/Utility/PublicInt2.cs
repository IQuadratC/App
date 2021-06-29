using System;
using UnityEngine;

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
            value = initalValue;
        }
    }
}