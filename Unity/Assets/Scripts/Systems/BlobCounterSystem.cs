using System.Collections.Generic;
using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Utils;

//Author : Attika

public class BlobCounterSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        // if there is no blob in the scene, return
        if (BlobUtils.GetCurrentEntityManager().GetAllEntities().Length <= GameManager.GetInstance().entitiesInEnvironment) return;

        var positions = new List<float3>();
        
        // for each on all entities that have translation AND blob unit movement components
        Entities.WithAll<Translation, BlobUnitMovement>().ForEach((Entity entity, ref Translation translation) =>
        {
            positions.Add(translation.Value);
        });
        
        BlobUtils.UpdateBlobPositions(positions);
    }
}