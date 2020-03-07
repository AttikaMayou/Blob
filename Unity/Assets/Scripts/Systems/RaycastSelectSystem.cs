using Components;
using Unity.Entities;
using Unity.Mathematics;
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
            if (Input.GetMouseButton(0))
            {
                Entities.ForEach((Entity entity, ref BlobUnitSelected selected) =>
                {
                    PostUpdateCommands.RemoveComponent<BlobUnitSelected>(entity);
                });

                var entitySelected = BlobUtils.RayCastFromMouse();
                PostUpdateCommands.AddComponent(entitySelected, new BlobUnitSelected());
            }

            if (Input.GetMouseButtonDown(2))
            {
                // Find a way to point at somewhere
                // and to determine float3 newPosition
            }
        }
    }
}