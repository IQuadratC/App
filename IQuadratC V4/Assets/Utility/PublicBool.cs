using System;
using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = "Publicbool", menuName = "Utility/Publicbool")]
    public class PublicInt : ScriptableObject, ISerializationCallbackReceiver
    {
        [NonSerialized] public bool value;
        [SerializeField] private bool initalValue;
        
        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            value = initalValue;
        }
    }
}