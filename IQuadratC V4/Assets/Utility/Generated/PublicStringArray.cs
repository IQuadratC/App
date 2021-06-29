using System;
using UnityEngine;
using Unity.Mathematics;
namespace Utility
{
    [CreateAssetMenu(fileName = "PublicStringArray", menuName = "Utility/PublicStringArray")]
    public class PublicStringArray : ScriptableObject, ISerializationCallbackReceiver
    {
        [NonSerialized] public string[] value;
        [SerializeField] private string[] initalValue;
        
        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            value = initalValue;
        }
    }
}