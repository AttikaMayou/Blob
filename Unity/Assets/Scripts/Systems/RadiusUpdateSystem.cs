using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Utils;
using static Unity.Mathematics.math;

//Author : Attika

//This system update radius to fit to target one (= medium radius from all blobs)

public class RadiusUpdateSystem : JobComponentSystem
{
    [BurstCompile]
    struct RadiusUpdateSystemJob : IJobForEach<Translation, BlobUnitedComponent>
    {
        public float DeltaTime;
        public float TargetRadius;
        public float SpeedChange;

        public void Execute(ref Translation translation, ref BlobUnitedComponent blobUnited)
        {
            if (!blobUnited.united) return;
            
            if (!blobUnited.needUpdate) return;
            
            if (blobUnited.lerpTime <= SpeedChange)
            {
                blobUnited.lerpTime += DeltaTime;
                blobUnited.radiusValue = lerp(blobUnited.radiusValue, TargetRadius, blobUnited.lerpTime / SpeedChange);
            }
            else
            {
                blobUnited.radiusValue = TargetRadius;
                blobUnited.lerpTime = 0.0f;
                blobUnited.needUpdate = false;
            }
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new RadiusUpdateSystemJob()
        { 
            DeltaTime = UnityEngine.Time.deltaTime,
            TargetRadius = BlobUtils.GetMediumRadius(),
            SpeedChange = GameManager.GetInstance().changeStateSpeed,
        };
        
        return job.Schedule(this, inputDependencies);
    }
}