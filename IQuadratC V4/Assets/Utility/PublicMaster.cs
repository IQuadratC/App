using System;
using UnityEngine;
using Unity.Mathematics;
namespace Utility
{
    [CreateAssetMenu(fileName = "PublicMaster", menuName = "Utility/PublicMaster")]
    public class PublicMaster : ScriptableObject, ISerializationCallbackReceiver
    {
        [NonSerialized] public object value;
        [SerializeField] private object initalValue;
        
        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            value = initalValue;
        }
    }
}