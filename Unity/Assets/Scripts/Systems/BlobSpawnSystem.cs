using Components;
using Unity.Entities;
using Unity.Physics;
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
            SpawnAnEntity(BlobState.Idle);
        }
        
        if (Input.GetKeyDown(KeyCode.L))
        {
            SpawnAnEntity(BlobState.Liquid);
        }
        
        if (Input.GetKeyDown(KeyCode.V))
        {
            SpawnAnEntity(BlobState.Viscous);
        }
    }

    private void SpawnAnEntity(BlobState state)
    {
        // spawn an entity
        var archetypeEntity = EntityManager.CreateArchetype(typeof(RigidBody));
        var spawnedEntity = EntityManager.Instantiate(GameManager.GetInstance().blobUnitPrefab);
        EntityManager.AddComponentData(spawnedEntity,
            new Translation {Value = GameManager.GetInstance().spawnPosition});
        EntityManager.AddComponentData(spawnedEntity,
            new BlobInfosComponent {blobUnitState = state});
        
        // update current number of blobs in scene
        GameManager.GetInstance().UpdateBlobCount(GameManager.GetInstance().GetCurrentBlobCount() + 1);

    }
}