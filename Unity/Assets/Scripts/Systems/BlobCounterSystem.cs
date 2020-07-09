using System;
using System.Collections.Generic;
using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Utils;
using BlobState = Components.BlobInfosComponent.BlobState;

//Author : Attika

public class BlobCounterSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        // if there is no blob in the scene, return
        if (BlobUtils.GetCurrentEntityManager().GetAllEntities().Length <= GameManager.GetInstance().entitiesInEnvironment) return;

        var positions = new List<float3>();
        var radius = new List<float>();
        
        // for each on all entities that have translation, scale AND blob infos components
        Entities.WithAll<Translation, BlobInfosComponent>().ForEach((Entity entity, BlobInfosComponent infos, ref Translation translation) =>
        {
            positions.Add(translation.Value);
            switch (infos.blobUnitState)
            {
                case BlobState.Idle:
                    radius.Add(GameManager.GetInstance().blobIdleRadius);
                    break;
                case BlobState.Liquid:
                    radius.Add(GameManager.GetInstance().blobIdleRadius);
                    break;
                case BlobState.Viscous:
                    radius.Add(GameManager.GetInstance().blobIdleRadius);
                    break;
                default:
                    break;
            }
        });
        
        BlobUtils.UpdateBlobPositions(positions, radius);
    }
}