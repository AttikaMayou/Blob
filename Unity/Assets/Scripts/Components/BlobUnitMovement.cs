﻿using System;
using Unity.Entities;
using Unity.Mathematics;

//Author : Attika

namespace Components
{
    [Serializable]
    [GenerateAuthoringComponent]
    public struct BlobUnitMovement : IComponentData
    {
        public bool move;
        public float moveSpeed;
        public float moveMultiplier;
        public float3 position;
    }
}
