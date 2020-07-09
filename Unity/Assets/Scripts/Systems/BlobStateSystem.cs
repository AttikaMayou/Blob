using System.Collections.Generic;
using Unity.Entities;
using Utils;
using BlobState = Components.BlobInfosComponent.BlobState;

//Author : Attika

// This system decide which state is the major one and update UI according to it

public class BlobStateSystem : ComponentSystem
{
    private int _idleBlobs;
    private int _liquidBlobs;
    private int _viscousBlobs;
    
    protected override void OnUpdate()
    {
        var states = BlobUtils.GetBlobCurrentStates();

        CountState(states);

        var majorState = ChooseState();
        
        BlobUtils.UpdateMajorState(majorState);
    }

    private void CountState(List<BlobState> states)
    {
        ResetCounts();
        
        foreach (var state in states)
        {
            switch (state)
            {
                case BlobState.Idle:
                    _idleBlobs++;
                    break;
                case BlobState.Liquid:
                    _liquidBlobs++;
                    break;
                case BlobState.Viscous:
                    _viscousBlobs++;
                    break;
                default:
                    _idleBlobs++;
                    break;
            }
        }
    }

    private void ResetCounts()
    {
        _idleBlobs = 0;
        _liquidBlobs = 0;
        _viscousBlobs = 0;
    }

    //TODO : make this dynamic (for more than 3 states)
    private BlobState ChooseState()
    {
        if(_idleBlobs >= _liquidBlobs && _idleBlobs >= _viscousBlobs)
            return BlobState.Idle;
        
        if (_liquidBlobs >= _idleBlobs && _liquidBlobs >= _viscousBlobs)
            return BlobState.Liquid;
        
        return BlobState.Viscous;
    }
}