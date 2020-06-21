using System;
using Unity.Entities;

//Author : Attika

namespace Components
{
    [Serializable]
    [GenerateAuthoringComponent]
    public struct BlobIdleStateComponent : IComponentData
    {
        public float multiplier;
    }
}
