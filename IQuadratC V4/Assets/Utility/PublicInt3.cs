using System;
using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = "Publicint3", menuName = "Utility/Publicint3")]
    public class PublicInt : ScriptableObject, ISerializationCallbackReceiver
    {
        [NonSerialized] public int3 value;
        [SerializeField] private int3 initalValue;
        
        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            value = initalValue;
        }
    }
}