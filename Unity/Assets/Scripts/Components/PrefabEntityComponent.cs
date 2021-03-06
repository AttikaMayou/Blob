﻿using Unity.Entities;

namespace Components
{
    [GenerateAuthoringComponent]
    public struct PrefabEntityComponent : IComponentData
    {
        public Entity BlobEntityPrefab;
        public Entity BlobLiquidPrefab;
        public Entity BlobViscousPrefab;
    }
}
