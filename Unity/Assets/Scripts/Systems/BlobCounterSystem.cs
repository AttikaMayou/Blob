using System.Collections.Generic;
using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Utils;
using BlobState = Components.BlobInfosComponent.BlobState;

//Author : Attika

// This system update positions, radius and color for all spawned blobs. 
// It also keeps count of how much entity of each state are currently spawned.

public class BlobCounterSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        // if there is no blob in the scene, return
        if (BlobUtils.GetCurrentEntityManager().GetAllEntities().Length <= GameManager.GetInstance().entitiesInEnvironment) return;

        var positions = new List<float3>();
        var radius = new List<float>();
        var states = new List<BlobState>();
        var united = new List<bool>();
        
        // for each on all entities that have translation, scale AND blob infos components
        Entities.WithAll<Translation, BlobInfosComponent>().ForEach((Entity entity, BlobInfosComponent infos, ref Translation translation) =>
        {
            positions.Add(translation.Value);
            states.Add(infos.blobUnitState);
            switch (infos.blobUnitState)
            {
                case BlobState.Idle:
                    radius.Add(GameManager.GetInstance().blobIdleRadius);
                    break;
                case BlobState.Liquid:
                    radius.Add(GameManager.GetInstance().blobLiquidRadius);
                    break;
                case BlobState.Viscous:
                    radius.Add(GameManager.GetInstance().blobViscousRadius);
                    break;
                default:
                    radius.Add(GameManager.GetInstance().blobIdleRadius);
                    break;
            }
        });
        
        BlobUtils.UpdateBlobPositions(positions, radius, states);
    }
}