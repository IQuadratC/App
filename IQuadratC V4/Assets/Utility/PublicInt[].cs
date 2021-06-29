using System;
using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = "Publicint[]", menuName = "Utility/Publicint[]")]
    public class PublicInt : ScriptableObject, ISerializationCallbackReceiver
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