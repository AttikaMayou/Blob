using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;

public class MoverSystem : JobComponentSystem
{
    //protected override void OnUpdate()
    //{
    //    Entities.ForEach((ref Translation translation, ref MoveComponent moveSpeedComponent) =>
    //    {
    //        translation.Value.y += moveSpeedComponent.moveSpeed * Time.DeltaTime;

    //        if(translation.Value.y > 5f)
    //        {
    //            moveSpeedComponent.moveSpeed = -math.abs(moveSpeedComponent.moveSpeed);
    //        }
    //        if (translation.Value.y < -5f)
    //        {
    //            moveSpeedComponent.moveSpeed = +math.abs(moveSpeedComponent.moveSpeed);
    //        }
    //    });
    //}
    
    [BurstCompile]
    private struct MoverJob : IJobForEachWithEntity<MoveComponent, Translation>
    {
        public float deltaTime;

        public void Execute(Entity entity, int index, ref MoveComponent moveComponent, ref Translation translation)
        {
            if(moveComponent.move)
            {
                float reachedPositionDistance = 1f;
                if(math.distance(translation.Value, moveComponent.position) > reachedPositionDistance)
                {
                    float3 moveDir = math.normalize(moveComponent.position - translation.Value);
                    moveComponent.lastMoveDir = moveDir;
                    translation.Value += moveDir * moveComponent.moveSpeed * deltaTime;
                }
                else
                {
                    moveComponent.move = false;
                }
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new MoverJob()
        {
            deltaTime = Time.DeltaTime
        };
        
        return job.Schedule(this, inputDeps);
    }
}
