using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

//Author : Attika

namespace Systems
{
    public class BlobSphereSpawnSystem : ComponentSystem
    {
        private readonly float3 _spawnPosition = new float3(0f, 6.0f, 0f);

        protected override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Entities.ForEach((ref PrefabEntityComponent prefabEntityComponent) =>
                {
                    var spawnedEntity = EntityManager.Instantiate(prefabEntityComponent.prefabEntity);
                    EntityManager.SetComponentData(spawnedEntity,
                        new Translation {Value = _spawnPosition});
                });

            }
        }
    }
}