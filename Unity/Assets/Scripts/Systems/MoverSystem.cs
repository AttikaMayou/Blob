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
            // variable of this job
            public float deltaTime; // current time in game
            public float reachedPosition; // tolerance between target position and current blob position
        
            public void Execute(ref Translation translation, ref BlobUnitMovement movement)
            {
                if (!movement.move) return;
                
                // while move is true, if blob didnt reach his target position
                if (math.distance(translation.Value, movement.position) > reachedPosition)
                {
                    // get the normalized direction vector
                    var moveDir = math.normalize(movement.position - translation.Value);
                    movement.lastMoveDir = moveDir;
                    // move blob by updating his position
                    translation.Value += moveDir * movement.moveSpeed * deltaTime;
                }
                // if blob reached his target position
                else
                {
                    // reset move speed
                    movement.moveSpeed = 0.0f;
                    // set move to false
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