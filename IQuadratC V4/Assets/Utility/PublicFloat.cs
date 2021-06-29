using System;
using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = "Publicfloat", menuName = "Utility/Publicfloat")]
    public class PublicInt : ScriptableObject, ISerializationCallbackReceiver
    {
        [NonSerialized] public float value;
        [SerializeField] private float initalValue;
        
        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            value = initalValue;
        }
    }
}