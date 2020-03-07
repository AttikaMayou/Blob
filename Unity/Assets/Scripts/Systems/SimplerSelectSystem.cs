using Components;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Utils;

namespace Systems
{
    public class SimplerSelectSystem : ComponentSystem
    {
        private float3 _mousePos;
        private readonly float _tolerance = 1f;

        protected override void OnUpdate()
        {
            if (Input.GetMouseButtonDown(2))
            {
                float3 targetPos = BlobUtils.GetMouseWorldPosition();
                
                Entities.WithAll<BlobUnitSelected>().ForEach((Entity entity, ref BlobUnitMovement movement) =>
                {
                    movement.position = movement.position + targetPos * 100000;
                    movement.move = true;
                });

            }
            
        }
    }
}