using System.Collections.Generic;
using Components;
using Test.ECS;
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
            // if (Input.GetMouseButtonDown(0))
            // {
            //     Debug.Log("mouse button 0 clicked !");
            //     // Entities.ForEach((Entity entity, ref BlobUnitSelected selected) =>
            //     // {
            //     //     PostUpdateCommands.RemoveComponent<BlobUnitSelected>(entity);
            //     // });
            //
            //     var entitySelected = BlobUtils.RayCastFromMouse(out var isThereEntity);
            //     if(isThereEntity)
            //         PostUpdateCommands.AddComponent(entitySelected, new BlobUnitSelected());
            //     Debug.Log("end of process");
            // }

            if (Input.GetMouseButtonDown(2))
            {
                var position = BlobUtils.GetGroundPosition(out var haveHit);

                if (!haveHit) return;
                
                var targetPositions = BlobUtils.MoveEntitiesTo(position, 20, 5, 10f);
                var positionIndex = 0;
                Entities.WithAll<BlobUnitMovement>().ForEach((Entity entity, ref BlobUnitMovement blobUnitMovement) =>
                {
                    blobUnitMovement.position = targetPositions[positionIndex];
                    Debug.Log(targetPositions[positionIndex] + " at " + positionIndex);
                    positionIndex = (positionIndex + 1) % targetPositions.Count;
                    blobUnitMovement.move = true;
                });
            }
        }
    }
}