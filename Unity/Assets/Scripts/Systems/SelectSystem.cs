using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Utils;

namespace Systems
{
    public class SelectSystem : ComponentSystem
    {
        private float3 _mousePos;
        private readonly float _tolerance = 1f;
        
        protected override void OnUpdate()
        {
              if (Input.GetMouseButton(0))
            {
                Entities.ForEach((Entity entity, ref BlobUnitSelected selected) =>
                {
                    PostUpdateCommands.RemoveComponent<BlobUnitSelected>(entity);
                });

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
                var minDistance = 1f;
                var minDistanceTab = new float[] { 0f };
                var entitiesCountPerRing = new int[] { 0 };
                var initialize =
                    BlobUtils.InitializeRingMovementSystem(minDistance, out minDistanceTab, out entitiesCountPerRing);
                if (initialize == -1) return;
                float3 targetPosition = BlobUtils.GetMouseWorldPosition();
                var movePositionList =
                    BlobUtils.GetPositionListAround(targetPosition, minDistanceTab, entitiesCountPerRing);
                var positionIndex = 0;

                Entities.WithAll<BlobUnitSelected>().ForEach((Entity entity, ref BlobUnitMovement movement) =>
                {
                    movement.position = movePositionList[positionIndex];
                    positionIndex = (positionIndex + 1) % movePositionList.Count;
                    movement.move = true;
                });

            }

        }
    }
}