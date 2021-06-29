using System;
using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = "PublicInt", menuName = "Utility/PublicInt")]
    public class PublicInt : ScriptableObject, ISerializationCallbackReceiver
    {
        [NonSerialized] public int value;
        [SerializeField] private int initalValue;
        
        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            value = initalValue;
        }
    }
}