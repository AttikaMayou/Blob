using System;
using Components;
using Unity.Entities;
using Unity.Transforms;
using Utils;
using BlobState = Components.BlobInfosComponent.BlobState;

//Author : Attika

// This system decide which state is the major one and update UI and major blob according to it

public class BlobStateSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        BlobUtils.UpdateMajorState(ChooseState());
        
        Entities.WithAll<BlobUnitedComponent, BlobInfosComponent, BlobUnitMovement>().ForEach((Entity entity, 
            ref BlobUnitedComponent blobUnited, ref BlobInfosComponent blobInfos, ref BlobUnitMovement blobMovement) =>
        {
            if (blobUnited.united)
            {
                var majorState = BlobUtils.GetMajorState();
                blobInfos.blobUnitState = BlobUtils.GetMajorState();

                switch (majorState)
                {
                    case BlobState.Idle:
                        blobMovement.moveSpeed = GameManager.GetInstance().blobIdleSpeed;
                        blobMovement.moveMultiplier = GameManager.GetInstance().idleSpeedMultiplier;
                        break;
                    case BlobState.Liquid:
                        blobMovement.moveSpeed = GameManager.GetInstance().blobLiquidSpeed;
                        blobMovement.moveMultiplier = GameManager.GetInstance().liquidSpeedMultiplier;
                        break;
                    case BlobState.Viscous:
                        blobMovement.moveSpeed = GameManager.GetInstance().blobViscousSpeed;
                        blobMovement.moveMultiplier = GameManager.GetInstance().viscousSpeedMultiplier;
                        break;
                    default:
                        blobMovement.moveSpeed = GameManager.GetInstance().blobIdleSpeed;
                        blobMovement.moveMultiplier = GameManager.GetInstance().idleSpeedMultiplier;
                        break;
                }
            }
        });
    }

    private BlobState ChooseState()
    {
        GameManager.GetInstance().GetBlobCounts(out var idleBlobs, out var liquidBlobs, out var viscousBlobs);

        if(idleBlobs >= liquidBlobs && idleBlobs >= viscousBlobs)
            return BlobState.Idle;
        
        if (liquidBlobs >= idleBlobs && liquidBlobs >= viscousBlobs)
            return BlobState.Liquid;
        
        return BlobState.Viscous;
    }
}