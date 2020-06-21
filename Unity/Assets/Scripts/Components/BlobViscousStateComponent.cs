using System;
using Unity.Entities;

//Author : Attika

namespace Components
{
    [Serializable]
    [GenerateAuthoringComponent]
    public struct BlobViscousStateComponent : IComponentData
    {
        public float multiplier;
    }
}
