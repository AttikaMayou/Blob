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
    public TextMeshProUGUI entitiesCount;
    public TextMeshProUGUI fpsUpdate;

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
    private const string EntitiesText = "Entites count : ";
    private const string FpsText = "FPS : ";
    private int _frameCount = 0;
    private float _dt = 0.0f;
    private float _updateRate = 4.0f;
    private float _fps = 0.0f;

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

    private void Update()
    {
        _frameCount++;
        _dt += Time.deltaTime;
        if (_dt > 1.0 / _updateRate)
        {
            _fps = _frameCount / _dt;
            _frameCount = 0;
            _dt -= 1 / _updateRate;
        }

        fpsUpdate.text = FpsText + _fps;
    }

    #region Public Methods
    public void UpdateStateFeedback(string state)
    {
        _instance.stateUpdate.text = StateText + state;
    }

    public void UpdateBlobCount(int nb, BlobInfosComponent.BlobState state)
    {
        _instance._blobCount = nb;
        _instance.entitiesCount.text = EntitiesText + _instance._blobCount;
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
