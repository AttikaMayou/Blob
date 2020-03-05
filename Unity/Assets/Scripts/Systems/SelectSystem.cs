using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Utils;

//Author : Attika

namespace Systems
{
    public class SelectSystem : ComponentSystem
    {
        private float3 _mousePos;
        private float _tolerance = 1f;
        
        protected override void OnUpdate()
        {
            if (Input.GetMouseButton(0))
            {
                _mousePos = BlobUtils.GetMouseWorldPosition();
                Debug.Log(_mousePos);
                
                Entities.ForEach((Entity entity, ref Translation translation) =>
                {
                    var entityPosition = translation.Value;
                    if (entityPosition.x >= _mousePos.x + _tolerance &&
                        entityPosition.x <= _mousePos.x - _tolerance &&
                        entityPosition.y >= _mousePos.y + _tolerance &&
                        entityPosition.y <= _mousePos.y - _tolerance &&
                        entityPosition.z >= _mousePos.z + _tolerance &&
                        entityPosition.z <= _mousePos.z - _tolerance)
                    {
                        Debug.Log("Unit selected : " + entity);
                        PostUpdateCommands.AddComponent(entity, new BlobUnitSelected());
                    }
                });
            }

            if (Input.GetMouseButtonUp(0))
            {
                Entities.WithAll<BlobUnitSelected>().ForEach((Entity entity) =>
                    {
                        PostUpdateCommands.RemoveComponent<BlobUnitSelected>(entity);
                    });
            }

            if (Input.GetMouseButtonDown(2))
            {
                Entities.WithAll<BlobUnitSelected>().ForEach((Entity entity, ref BlobUnitMovement movement) =>
                    {
                        movement.position = BlobUtils.GetMouseWorldPosition();
                        movement.move = true;
                    });
            }
        }
    }
}