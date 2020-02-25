using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public struct UnitSelected : IComponentData
{

}

public class BlobControlSystem : ComponentSystem
{
    private float3 mousePosition;
    private float tolerance = 1f;

    protected override void OnUpdate()
    {
        if(Input.GetMouseButton(0))
        {
            mousePosition = BlobUtils.GetMouseWorldPosition();

            Entities.ForEach((Entity entity, ref Translation translation) =>
            {
                float3 entityPosition = translation.Value;
                if (entityPosition.x >= mousePosition.x + tolerance &&
                   entityPosition.x <= mousePosition.x - tolerance &&
                   entityPosition.y >= mousePosition.y + tolerance &&
                   entityPosition.y <= mousePosition.y - tolerance)
                {
                    PostUpdateCommands.AddComponent(entity, new UnitSelected());
                }
            });
        }

        if(Input.GetMouseButtonUp(0))
        {
            Entities.WithAll<UnitSelected>().ForEach((Entity entity) =>
            {
                PostUpdateCommands.RemoveComponent<UnitSelected>(entity);
            });
        }

        if(Input.GetMouseButtonDown(1))
        {
            Entities.WithAll<UnitSelected>().ForEach((Entity entity, ref MoveComponent moveComponent) =>
            {
                moveComponent.position = BlobUtils.GetMouseWorldPosition();
                moveComponent.move = true;
            });
        }

    }
}
