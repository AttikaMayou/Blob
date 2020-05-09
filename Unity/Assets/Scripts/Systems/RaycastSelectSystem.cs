using System.Collections.Generic;
using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Utils;

//Author : Attika

namespace Systems
{
    public class RaycastSelectSystem : ComponentSystem
    {
        private float3 _mousePos;
        
        protected override void OnUpdate()
        {
            if (!Input.GetMouseButtonDown(2)) return;
            
            var position = BlobUtils.GetGroundPosition(out var haveHit);

            if (!haveHit) return;
                
            var targetPositions = BlobUtils.MoveEntitiesTo(position, 20, 5, 10f);
            var positionIndex = 0;

            if (BlobUtils.GetCurrentEntityManager().GetAllEntities().Length <= 0) return;
            
            Entities.WithAll<BlobUnitMovement>().ForEach((Entity entity, ref BlobUnitMovement blobUnitMovement) =>
            {
                blobUnitMovement.position = targetPositions[positionIndex];
                Debug.Log(targetPositions[positionIndex] + " at " + positionIndex);
                positionIndex = (positionIndex + 1) % targetPositions.Count;
                blobUnitMovement.move = true;
                //UpdateInjectedComponentGroups()
            });
        }
    }
}