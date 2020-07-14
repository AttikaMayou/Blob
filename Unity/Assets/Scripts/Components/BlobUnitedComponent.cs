using System;
using Unity.Entities;

//Author : Attika

namespace Components
{
    [Serializable]
    [GenerateAuthoringComponent]
    public struct BlobUnitedComponent : IComponentData
    {
        public bool united;
    }
}
