using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

//Author : Attika

namespace Systems
{
    public class MoverSystem : JobComponentSystem
    {
        [BurstCompile]
        private struct MoverSystemJob : IJobForEach<Translation, Rotation, BlobUnitMovement>
        {
             public float deltaTime;
        
            public void Execute(ref Translation translation, [ReadOnly] ref Rotation rotation, ref BlobUnitMovement movement)
            {
                if (!movement.move) return;
                
                const float reachedPositionDistance = 1f;
                
                if (math.distance(translation.Value, movement.position) > reachedPositionDistance)
                {
                    var moveDir = math.normalize(movement.position - translation.Value);
                    movement.lastMoveDir = moveDir;
                    translation.Value += moveDir * movement.moveSpeed * deltaTime;
                }
                else
                {
                    movement.move = false;
                }
            }
        }
    
        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var job = new MoverSystemJob()
            {
                deltaTime = UnityEngine.Time.deltaTime
            };
        
            return job.Schedule(this, inputDependencies);
        }
    }
}