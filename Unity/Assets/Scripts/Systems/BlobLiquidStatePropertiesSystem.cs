using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

//Author : Attika

public class BlobLiquidStatePropertiesSystem : JobComponentSystem
{
    [BurstCompile]
    struct BlobLiquidStatePropertiesSystemJob : IJobForEach<BlobUnitMovement, BlobLiquidStateComponent>
    {
        public void Execute(ref BlobUnitMovement movement, [ReadOnly] ref BlobLiquidStateComponent liquid)
        {
            movement.moveSpeed *= liquid.multiplier;
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new BlobLiquidStatePropertiesSystemJob();
        
        return job.Schedule(this, inputDependencies);
    }
}