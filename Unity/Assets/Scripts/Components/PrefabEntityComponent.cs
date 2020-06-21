using Unity.Entities;
using UnityEngine;

namespace Components
{
    [GenerateAuthoringComponent]
    public struct PrefabEntityComponent : IComponentData
    {
        public Entity prefabEntity;
        public Entity viscousPrefabEntity;
        public Entity liquidPrefabEntity;
    }
}
