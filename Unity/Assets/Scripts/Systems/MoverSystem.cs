using Components;
using Unity.Burst;
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
        private struct MoverSystemJob : IJobForEach<Translation, BlobUnitMovement>
        {
            public float deltaTime;
            public float reachedPosition;
        
            public void Execute(ref Translation translation, ref BlobUnitMovement movement)
            {
                if (!movement.move) return;
                
                if (math.distance(translation.Value, movement.position) > reachedPosition)
                {
                    var moveDir = math.normalize(movement.position - translation.Value);
                    movement.lastMoveDir = moveDir;
                    translation.Value += moveDir * movement.moveSpeed * deltaTime;
                }
                else
                {
                    movement.moveSpeed = 0.0f;
                    movement.move = false;
                }
            }
        }
    
        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var job = new MoverSystemJob()
            {
                deltaTime = UnityEngine.Time.deltaTime,
                reachedPosition = GameManager.GetInstance().toleranceDistance
            };
        
            return job.Schedule(this, inputDependencies);
        }
    }
}