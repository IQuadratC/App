using System;
using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = "Publicstring", menuName = "Utility/Publicstring")]
    public class PublicInt : ScriptableObject, ISerializationCallbackReceiver
    {
        [NonSerialized] public string value;
        [SerializeField] private string initalValue;
        
        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            value = initalValue;
        }
    }
}