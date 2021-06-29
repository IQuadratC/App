using System;
using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = "Publicint4", menuName = "Utility/Publicint4")]
    public class PublicInt : ScriptableObject, ISerializationCallbackReceiver
    {
        [NonSerialized] public int4 value;
        [SerializeField] private int4 initalValue;
        
        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            value = initalValue;
        }
    }
}