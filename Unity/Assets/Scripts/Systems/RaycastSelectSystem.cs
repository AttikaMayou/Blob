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
                Debug.Log("1) mouse button 2 clicked !");
                var position = BlobUtils.GetMousePositionInPhysicWorld(out var haveHit);
                Debug.Log("4) position found : " + position);
                
                var forward = Input.mousePosition + new Vector3(0.0f, 0.0f, -100.0f);
                Debug.DrawLine(BlobUtils.GetMousePositionInPhysicWorld(), forward, Color.magenta, 10);
                
                if (haveHit)
                {
                    BlobUtils.MoveEntitiesTo(position);
                }
                Debug.Log("5) end of process");
            }
        }
    }
}