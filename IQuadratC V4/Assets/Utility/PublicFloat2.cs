using System;
using UnityEngine;
using Unity.Mathematics;
namespace Utility
{
    [CreateAssetMenu(fileName = "PublicFloat2", menuName = "Utility/PublicFloat2")]
    public class PublicFloat2 : ScriptableObject, ISerializationCallbackReceiver
    {
        [NonSerialized] public float2 value;
        [SerializeField] private float2 initalValue;
        
        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            value = initalValue;
        }
    }
}