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
        var radius = new List<float>();
        
        // for each on all entities that have translation, scale AND blob unit movement components
        Entities.WithAll<Translation, Scale, BlobUnitMovement>().ForEach((Entity entity, ref Translation translation, ref Scale scale) =>
        {
            positions.Add(translation.Value);
            radius.Add(scale.Value);
        });
        
        BlobUtils.UpdateBlobPositions(positions, radius);
    }
}