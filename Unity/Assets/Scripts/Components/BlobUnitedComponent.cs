using System;
using Unity.Entities;

//Author : Attika

namespace Components
{
    [Serializable]
    [GenerateAuthoringComponent]
    public struct BlobUnitedComponent : IComponentData
    {
        public float radiusValue;
        public bool united;
        public bool needUpdate;
        public float distanceToOthers;
        public float lerpTime;
    }
}
