using System;
using UnityEngine;
using Unity.Mathematics;
namespace Utility
{
    [CreateAssetMenu(fileName = "PublicInt4", menuName = "Utility/PublicInt4")]
    public class PublicInt4 : ScriptableObject, ISerializationCallbackReceiver
    {
        [NonSerialized] public int4 value;
        [SerializeField] private int4 initalValue;
        
        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            value = initalValue;
        }
    }
}