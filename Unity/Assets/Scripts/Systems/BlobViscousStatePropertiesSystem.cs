using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

//Author : Attika

public class BlobViscousStatePropertiesSystem : JobComponentSystem
{
    [BurstCompile]
    struct BlobViscousStatePropertiesSystemJob : IJobForEach<BlobUnitMovement, BlobViscousStateComponent>
    {
        public void Execute(ref BlobUnitMovement movement, [ReadOnly] ref BlobViscousStateComponent viscous)
        {
           movement.multiplier = viscous.multiplier;
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new BlobViscousStatePropertiesSystemJob();
        
        return job.Schedule(this, inputDependencies);
    }
}