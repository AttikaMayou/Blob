using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

//Author : Attika

public class BlobStatePropertiesSystem : JobComponentSystem
{
    [BurstCompile]
    private struct BlobStatePropertiesSystemJob : IJobForEach<BlobIdleStateComponent, BlobUnitMovement>
    {
        public void Execute([ReadOnly] ref BlobIdleStateComponent idle, ref BlobUnitMovement movement)
        {
            movement.multiplier = idle.multiplier;
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new BlobStatePropertiesSystemJob();
         
        return job.Schedule(this, inputDependencies);
    }
}