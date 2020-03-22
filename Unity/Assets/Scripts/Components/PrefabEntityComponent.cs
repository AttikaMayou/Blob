using Unity.Entities;
using UnityEngine;

namespace Components
{
    [GenerateAuthoringComponent]
    public struct PrefabEntityComponent : IComponentData
    {
        public Entity prefabEntity;
    }
}
