using System;
using Unity.Entities;

//Author : Attika

namespace Components
{
    [Serializable]
    [GenerateAuthoringComponent]
    public struct BlobUnitSelected : IComponentData
    {
        public bool selected;
    }
}
