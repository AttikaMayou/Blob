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
        private struct MoverSystemJob : IJobForEach<Translation, BlobUnitMovement>
        {
            public float deltaTime;
        
            public void Execute(ref Translation translation, ref BlobUnitMovement movement)
            {
                if (!movement.move) return;

                //Peut-être changer cela : 
                //Une boule est maîtresse des autres
                //Et les autres suivent en gardant toujours la même distance
                //entre elles quoi qu'il arrive
                //plutôt que d'envoyer toutes les positions de toutes les boules 
                
                const float reachedPositionDistance = 1.00001f;
                
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