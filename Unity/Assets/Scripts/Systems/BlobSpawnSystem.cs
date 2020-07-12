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
                SpawnAnEntity(prefabEntityComponent.BlobEntityPrefab, BlobState.Idle);
            });
        }
        
        if (Input.GetKeyDown(KeyCode.L))
        {
            Entities.ForEach((ref PrefabEntityComponent prefabEntityComponent) =>
            {
                SpawnAnEntity(prefabEntityComponent.BlobEntityPrefab, BlobState.Liquid);
            });
        }
        
        if (Input.GetKeyDown(KeyCode.V))
        {
            Entities.ForEach((ref PrefabEntityComponent prefabEntityComponent) =>
            {
                SpawnAnEntity(prefabEntityComponent.BlobEntityPrefab, BlobState.Viscous);
            });
        }
    }

    private void SpawnAnEntity(Entity prefab, BlobState state)
    {
        // spawn an entity
        var spawnedEntity = EntityManager.Instantiate(prefab);
        EntityManager.AddComponentData(spawnedEntity,
            new Translation {Value = GameManager.GetInstance().spawnPosition});
        EntityManager.AddComponentData(spawnedEntity,
            new BlobInfosComponent {blobUnitState = state});
        
        // update current number of blobs in scene
        GameManager.GetInstance().UpdateBlobCount(GameManager.GetInstance().GetCurrentBlobCount() + 1);

    }
}