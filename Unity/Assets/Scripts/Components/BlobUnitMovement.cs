using System;
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
        public float multiplier;
        public float3 position;
        public float3 lastMoveDir;
    }
}
