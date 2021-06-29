using System;
using UnityEngine;
using Unity.Mathematics;
namespace Utility
{
    [CreateAssetMenu(fileName = "PublicFloat3", menuName = "Utility/PublicFloat3")]
    public class PublicFloat3 : ScriptableObject, ISerializationCallbackReceiver
    {
        [NonSerialized] public float3 value;
        [SerializeField] private float3 initalValue;
        
        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            value = initalValue;
        }
    }
}