using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Utils;
using BlobState = Components.BlobInfosComponent.BlobState;

//Author : Attika

public class BlobSpawnSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Entities.ForEach((ref BlobUnitedComponent unitedComponent) =>
            {
                unitedComponent.needUpdate = true;
            });
            Entities.ForEach((ref PrefabEntityComponent prefabEntityComponent) =>
            {
                SpawnAnEntity(prefabEntityComponent.BlobEntityPrefab, BlobState.Idle, GameManager.GetInstance().blobIdleSpeed, GameManager.GetInstance().idleSpeedMultiplier, GameManager.GetInstance().blobIdleRadius);
            });
        }
        
        if (Input.GetKeyDown(KeyCode.L))
        {
            Entities.ForEach((ref BlobUnitedComponent unitedComponent) =>
            {
                unitedComponent.needUpdate = true;
            });
            Entities.ForEach((ref PrefabEntityComponent prefabEntityComponent) =>
            {
                SpawnAnEntity(prefabEntityComponent.BlobLiquidPrefab, BlobState.Liquid, GameManager.GetInstance().blobLiquidSpeed, GameManager.GetInstance().liquidSpeedMultiplier, GameManager.GetInstance().blobLiquidRadius);
            });
        }
        
        if (Input.GetKeyDown(KeyCode.V))
        {
            Entities.ForEach((ref BlobUnitedComponent unitedComponent) =>
            {
                unitedComponent.needUpdate = true;
            });
            Entities.ForEach((ref PrefabEntityComponent prefabEntityComponent) =>
            {
                SpawnAnEntity(prefabEntityComponent.BlobViscousPrefab, BlobState.Viscous, GameManager.GetInstance().blobViscousSpeed,GameManager.GetInstance().viscousSpeedMultiplier, GameManager.GetInstance().blobViscousRadius);
            });
        }
    }

    private void SpawnAnEntity(Entity prefab, BlobState state, float speed, float multiplier, float radius)
    {
        // spawn an entity
        var spawnedEntity = EntityManager.Instantiate(prefab);
        EntityManager.SetComponentData(spawnedEntity,
            new Translation {Value = GameManager.GetInstance().spawnPosition});
        EntityManager.SetComponentData(spawnedEntity,
            new BlobInfosComponent {blobUnitState = state});
        EntityManager.SetComponentData(spawnedEntity, 
            new BlobUnitMovement{moveSpeed = speed, moveMultiplier = multiplier});
        EntityManager.SetComponentData(spawnedEntity,
            new BlobUnitedComponent{radiusValue = radius, united = false, needUpdate = false, lerpTime = 0.0f});

        // update current number of blobs in scene
        GameManager.GetInstance().UpdateBlobCount(GameManager.GetInstance().GetCurrentBlobCount() + 1, state);
        BlobUtils.CalculateMediumRadius();

    }
}