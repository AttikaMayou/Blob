using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

//Author : Attika

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
                EntityManager.SetComponentData(spawnedEntity,
                    new BlobIdleStateComponent() {multiplier = 1.0f});
            });
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Entities.ForEach((ref PrefabEntityComponent prefabEntityComponent) =>
            {
                var spawnedEntity = EntityManager.Instantiate(prefabEntityComponent.liquidPrefabEntity);
                EntityManager.SetComponentData(spawnedEntity,
                    new Translation {Value = _spawnPosition});
                EntityManager.SetComponentData(spawnedEntity,
                    new BlobLiquidStateComponent() {multiplier = 2.0f});
            });
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            Entities.ForEach((ref PrefabEntityComponent prefabEntityComponent) =>
            {
                var spawnedEntity = EntityManager.Instantiate(prefabEntityComponent.viscousPrefabEntity);
                EntityManager.SetComponentData(spawnedEntity,
                    new Translation {Value = _spawnPosition});
                EntityManager.SetComponentData(spawnedEntity,
                    new BlobViscousStateComponent() {multiplier = 0.5f});
            });
        }
    }
}