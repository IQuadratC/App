using System;
using UnityEngine;
using Unity.Mathematics;
namespace Utility
{
    [CreateAssetMenu(fileName = "PublicString", menuName = "Utility/PublicString")]
    public class PublicString : ScriptableObject, ISerializationCallbackReceiver
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