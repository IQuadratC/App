using System;
using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = "Publicint2", menuName = "Utility/Publicint2")]
    public class PublicInt : ScriptableObject, ISerializationCallbackReceiver
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