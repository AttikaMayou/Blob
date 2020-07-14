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
    struct RadiusUpdateSystemJob : IJobForEach<Translation, BlobUnitedComponent, BlobUnitMovement>
    {
        public float DeltaTime;
        public float TargetRadius;
        
        public void Execute(ref Translation translation, ref BlobUnitedComponent blobUnited, ref BlobUnitMovement blobMove)
        {
            if (!blobUnited.united || !blobMove.move || !blobUnited.needUpdate) return;
            
            if (blobUnited.lerpTime <= blobUnited.distanceToOthers)
            {
                blobUnited.lerpTime += DeltaTime;
                blobUnited.radiusValue = lerp(blobUnited.radiusValue, TargetRadius,
                    clamp(blobUnited.lerpTime / blobUnited.distanceToOthers, 0.0f, 1.0f));
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
            TargetRadius = BlobUtils.GetMediumRadius()
        };
        
        return job.Schedule(this, inputDependencies);
    }
}