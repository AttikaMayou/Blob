using Components;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using BlobState = Components.BlobInfosComponent.BlobState;

//Author : Attika

public class BlobSpawnSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Entities.ForEach((ref PrefabEntityComponent prefabEntityComponent) =>
            {
                SpawnAnEntity(prefabEntityComponent.BlobEntityPrefab, BlobState.Idle, GameManager.GetInstance().blobIdleSpeed, GameManager.GetInstance().idleSpeedMultiplier);
            });
        }
        
        if (Input.GetKeyDown(KeyCode.L))
        {
            Entities.ForEach((ref PrefabEntityComponent prefabEntityComponent) =>
            {
                SpawnAnEntity(prefabEntityComponent.BlobEntityPrefab, BlobState.Liquid, GameManager.GetInstance().blobLiquidSpeed, GameManager.GetInstance().liquidSpeedMultiplier);
            });
        }
        
        if (Input.GetKeyDown(KeyCode.V))
        {
            Entities.ForEach((ref PrefabEntityComponent prefabEntityComponent) =>
            {
                SpawnAnEntity(prefabEntityComponent.BlobEntityPrefab, BlobState.Viscous, GameManager.GetInstance().blobViscousSpeed,GameManager.GetInstance().viscousSpeedMultiplier);
            });
        }
    }

    private void SpawnAnEntity(Entity prefab, BlobState state, float speed, float multiplier)
    {
        // spawn an entity
        var spawnedEntity = EntityManager.Instantiate(prefab);
        EntityManager.SetComponentData(spawnedEntity,
            new Translation {Value = GameManager.GetInstance().spawnPosition});
        EntityManager.SetComponentData(spawnedEntity,
            new BlobInfosComponent {blobUnitState = state});
        EntityManager.SetComponentData(spawnedEntity, 
            new BlobUnitMovement{moveSpeed = speed, moveMultiplier = multiplier});
        
        // update current number of blobs in scene
        GameManager.GetInstance().UpdateBlobCount(GameManager.GetInstance().GetCurrentBlobCount() + 1);

    }
}