﻿using System;
using UnityEngine;
using Unity.Mathematics;
namespace Utility
{
    [CreateAssetMenu(fileName = "PublicFloat4", menuName = "Utility/PublicFloat4")]
    public class PublicFloat4 : ScriptableObject, ISerializationCallbackReceiver
    {
        [NonSerialized] public float4 value;
        [SerializeField] private float4 initalValue;
        
        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            value = initalValue;
        }
    }
}