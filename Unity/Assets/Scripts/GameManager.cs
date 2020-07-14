using System;
using System.Runtime.InteropServices;
using Components;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using Utils;

//Author : Attika

public class GameManager : MonoBehaviour
{
    
    #region Public Variables
    [Header("Spawn Parameters")] 
    public float3 spawnPosition;
    
    [Header ("Game Parameters")]
    public int nbEntitiesOnFirstRing;
    public float toleranceDistance;

    [Header("State Parameters")]
    public float blobIdleSpeed;
    public float blobLiquidSpeed;
    public float blobViscousSpeed;
    public float idleSpeedMultiplier;
    public float liquidSpeedMultiplier;
    public float viscousSpeedMultiplier;
    public float blobIdleRadius;
    public float blobLiquidRadius;
    public float blobViscousRadius;
    public TextMeshProUGUI stateUpdate;
    #endregion
    
    private int _blobCount = 0;
    private int _idleCount = 0;
    private int _liquidCount = 0;
    private int _viscousCount = 0;
    private const string StateText = "Current state : ";

    // handle GameManager instance
    private static GameManager _instance;
    public static GameManager GetInstance()
    {
        return _instance;
    }
    private void Awake()
    {
        if (!_instance)
            _instance = this;
    }

    #region Public Methods
    public void UpdateStateFeedback(string state)
    {
        _instance.stateUpdate.text = StateText + state;
    }

    public void UpdateBlobCount(int nb, BlobInfosComponent.BlobState state)
    {
        _instance._blobCount = nb;
        switch (state)
        {
            case BlobInfosComponent.BlobState.Idle:
                _instance._idleCount += 1;
                break;
            case BlobInfosComponent.BlobState.Liquid:
                _instance._liquidCount += 1;
                break;
            case BlobInfosComponent.BlobState.Viscous:
                _instance._viscousCount += 1;
                break;
            default:
                _instance._idleCount += 1;
                break;
        }
    }
    
    public int GetCurrentBlobCount()
    {
        return _instance._blobCount;
    }

    public void GetBlobCounts(out int idle, out int liquid, out int viscous)
    {
        idle = _instance._idleCount;
        liquid = _instance._liquidCount;
        viscous = _instance._viscousCount;
    }
    #endregion
}
