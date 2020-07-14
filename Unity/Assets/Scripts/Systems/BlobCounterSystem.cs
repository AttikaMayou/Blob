using System.Collections.Generic;
using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Utils;
using BlobState = Components.BlobInfosComponent.BlobState;

//Author : Attika

// This system update positions, radius and color tracked infos for all spawned blobs. 
// It also lerp blob's radius to target one

public class BlobCounterSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        // if there is no blob in the scene, return
        if (GameManager.GetInstance().GetCurrentBlobCount() <= 0) return;

        var positions = new List<float3>();
        var radius = new List<float>();
        var states = new List<BlobState>();

        var targetRadius = BlobUtils.GetMediumRadius();
        var speedChange = GameManager.GetInstance().changeStateSpeed;
        
        // for each on all entities that have translation, scale AND blob infos components
        Entities.WithAll<Translation, /*Scale,*/ BlobInfosComponent, BlobUnitedComponent>().ForEach((Entity entity,
            ref Translation translation,/* ref Scale scale,*/ ref BlobInfosComponent infos,  ref BlobUnitedComponent blobUnited) =>
        {
            positions.Add(translation.Value);
            states.Add(infos.blobUnitState);
            switch (infos.blobUnitState)
            {
                case BlobState.Idle:
                    if (blobUnited.united)
                    {
                        var radiusLerp = targetRadius;
                        if (blobUnited.lerpTime <= speedChange)
                        {
                            blobUnited.lerpTime += Time.DeltaTime;
                            
                            // lerp to medium radius since blob is "united" to others
                            radiusLerp= math.lerp(GameManager.GetInstance().blobIdleRadius, targetRadius,
                                blobUnited.lerpTime);
                        }
                        else
                        {
                            blobUnited.lerpTime = 0.0f;
                        }
                        radius.Add(radiusLerp);
                        //scale.Value = radiusLerp;
                    }
                    else
                    {
                        radius.Add(GameManager.GetInstance().blobIdleRadius);
                    }
                    break;
                
                case BlobState.Liquid:
                    if (blobUnited.united)
                    {
                        var radiusLerp = targetRadius;
                        if (blobUnited.lerpTime <= speedChange)
                        {
                            blobUnited.lerpTime += Time.DeltaTime;
                            
                            // lerp to medium radius since blob is "united" to others
                            radiusLerp= math.lerp(GameManager.GetInstance().blobLiquidRadius, targetRadius,
                                blobUnited.lerpTime);
                        }
                        else
                        {
                            blobUnited.lerpTime = 0.0f;
                        }
                        radius.Add(radiusLerp);
                        //scale.Value = radiusLerp;
                    }
                    else
                    {
                        radius.Add(GameManager.GetInstance().blobLiquidRadius);
                    }
                    break;
                
                case BlobState.Viscous:
                    if (blobUnited.united)
                    {
                        var radiusLerp = targetRadius;
                        if (blobUnited.lerpTime <= speedChange)
                        {
                            blobUnited.lerpTime += Time.DeltaTime;
                            
                            // lerp to medium radius since blob is "united" to others
                            radiusLerp= math.lerp(GameManager.GetInstance().blobViscousRadius, targetRadius,
                                blobUnited.lerpTime);
                        }
                        else
                        {
                            blobUnited.lerpTime = 0.0f;
                        }
                        radius.Add(radiusLerp);
                        //scale.Value = radiusLerp;
                    }
                    else
                    {
                        radius.Add(GameManager.GetInstance().blobLiquidRadius);
                    }
                    break;
                
                default:
                    if (blobUnited.united)
                    {
                        var radiusLerp = targetRadius;
                        if (blobUnited.lerpTime <= speedChange)
                        {
                            blobUnited.lerpTime += Time.DeltaTime;
                            
                            // lerp to medium radius since blob is "united" to others
                            radiusLerp= math.lerp(GameManager.GetInstance().blobIdleRadius, targetRadius,
                                blobUnited.lerpTime);
                        }
                        else
                        {
                            blobUnited.lerpTime = 0.0f;
                        }
                        radius.Add(radiusLerp);
                        //scale.Value = radiusLerp;
                    }
                    else
                    {
                        radius.Add(GameManager.GetInstance().blobIdleRadius);
                    }
                    break;
            }
           
            
        });
        
        BlobUtils.UpdateBlobPositions(positions, radius, states);
    }
}