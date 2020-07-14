using Components;
using Unity.Entities;
using Utils;
using BlobState = Components.BlobInfosComponent.BlobState;

//Author : Attika

// This system decide which state is the major one and update UI and major blob according to it

public class BlobStateSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        BlobUtils.UpdateMajorState(ChooseState());
    }

    private BlobState ChooseState()
    {
        GameManager.GetInstance().GetBlobCounts(out var _idleBlobs, out var _liquidBlobs, out var _viscousBlobs);

        if(_idleBlobs >= _liquidBlobs && _idleBlobs >= _viscousBlobs)
            return BlobState.Idle;
        
        if (_liquidBlobs >= _idleBlobs && _liquidBlobs >= _viscousBlobs)
            return BlobState.Liquid;
        
        return BlobState.Viscous;
    }
}