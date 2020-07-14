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
        
        Entities.WithAll<BlobUnitedComponent, BlobInfosComponent>().ForEach((Entity entity, ref BlobUnitedComponent blobUnited, ref BlobInfosComponent blobInfos) =>
        {
            if (blobUnited.united)
            {
                blobInfos.blobUnitState = BlobUtils.GetMajorState();
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