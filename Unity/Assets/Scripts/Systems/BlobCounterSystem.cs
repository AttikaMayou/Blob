using System.Collections.Generic;
using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Authoring;
using Unity.Transforms;
using UnityEngine;
using Utils;
using Collider = Unity.Physics.Collider;
using SphereCollider = Unity.Physics.SphereCollider;

//Author : Attika

public class BlobCounterSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        // if there is no blob in the scene, return
        if (BlobUtils.GetCurrentEntityManager().GetAllEntities().Length <= GameManager.GetInstance().entitiesInEnvironment) return;

        var positions = new List<float3>();
        //var radius = new List<float>();
        
        // for each on all entities that have translation AND blob unit movement components
        Entities.WithAll<Translation, BlobUnitMovement>().ForEach((Entity entity, ref Translation translation) =>
        {
            positions.Add(translation.Value);
            //radius.Add(GameManager.GetInstance().blobRadius);
        });
        
        BlobUtils.UpdateBlobPositions(positions, GameManager.GetInstance().blobRadius);
    }
}