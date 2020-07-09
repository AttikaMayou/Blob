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
        var spawnedEntity = EntityManager.Instantiate(GameManager.GetInstance().blobUnitPrefab);
        EntityManager.SetComponentData(spawnedEntity,
            new Translation {Value = GameManager.GetInstance().spawnPosition});
        EntityManager.SetComponentData(spawnedEntity,
            new BlobInfosComponent {blobUnitState = state});
    }
}