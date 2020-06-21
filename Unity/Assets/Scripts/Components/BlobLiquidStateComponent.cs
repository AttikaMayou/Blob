using System;
using Unity.Entities;

//Author : Attika

namespace Components
{
    [Serializable]
    [GenerateAuthoringComponent]
    public struct BlobLiquidStateComponent : IComponentData
    {
        public float multiplier;
    }
}
