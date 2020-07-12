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
    public float blobIdleRadius;
    public float blobLiquidRadius;
    public float blobViscousRadius;
    public TextMeshProUGUI stateUpdate;
    #endregion
    private int _entitiesInEnvironment;
    private int _blobCount;
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
        
        _instance._entitiesInEnvironment = BlobUtils.InitializeEntitiesInEnvironment();
    }

    #region Public Methods
    public void UpdateStateFeedback(string state)
    {
        _instance.stateUpdate.text = StateText + state;
    }

    public void UpdateBlobCount(int nb)
    {
        _blobCount = nb;
    }
    
    public int GetCurrentBlobCount()
    {
        return _blobCount;
    }
    #endregion
}
