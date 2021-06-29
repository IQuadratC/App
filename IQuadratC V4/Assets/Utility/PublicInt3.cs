using System;
using UnityEngine;
using Unity.Mathematics;
namespace Utility
{
    [CreateAssetMenu(fileName = "PublicInt3", menuName = "Utility/PublicInt3")]
    public class PublicInt3 : ScriptableObject, ISerializationCallbackReceiver
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