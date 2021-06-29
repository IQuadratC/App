using System;
using UnityEngine;
using Unity.Mathematics;
namespace Utility
{
    [CreateAssetMenu(fileName = "PublicFloat", menuName = "Utility/PublicFloat")]
    public class PublicFloat : ScriptableObject, ISerializationCallbackReceiver
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