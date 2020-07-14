using System;
using Unity.Entities;

//Author : Attika 

namespace Components
{
    [Serializable]
    [GenerateAuthoringComponent]
    public struct BlobInfosComponent : IComponentData
    {
        public enum BlobState
        {
            Idle,
            Liquid,
            Viscous
        }
    
        public BlobState blobUnitState;
    }
}
