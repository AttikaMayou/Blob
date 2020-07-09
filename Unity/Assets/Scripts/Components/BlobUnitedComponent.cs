using System;
using Unity.Entities;

//Author : Attika

namespace Components
{
    [Serializable]
    [GenerateAuthoringComponent]
    public class BlobUnitedComponent : IComponentData
    {
        public bool united;
    }
}
