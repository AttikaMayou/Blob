using System.Collections.Generic;
using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Utils;
using BlobState = Components.BlobInfosComponent.BlobState;

//Author : Attika

// This system update positions, radius and states tracked infos for all spawned blobs.

public class BlobCounterSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        // if there is no blob in the scene, return
        if (GameManager.GetInstance().GetCurrentBlobCount() <= 0) return;

        var positions = new List<float3>();
        var radius = new List<float>();
        var states = new List<BlobState>();
        
        // for each on all entities that have translation, scale AND blob infos components
        Entities.WithAll<Translation, BlobInfosComponent, BlobUnitedComponent>().ForEach((Entity entity,
            ref Translation translation, ref BlobInfosComponent infos,  ref BlobUnitedComponent blobUnited) =>
        {
            positions.Add(translation.Value);
            radius.Add(blobUnited.radiusValue);
            states.Add(infos.blobUnitState);
        });
        
        BlobUtils.UpdateBlobPositions(positions, radius, states);
    }
}