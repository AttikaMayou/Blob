using TMPro;
using Unity.Mathematics;
using UnityEngine;

//Author : Attika

public class GameManager : MonoBehaviour
{
    [Header("Spawn Parameters")] 
    public GameObject blobUnitPrefab;
    public float3 spawnPosition;
    
    [Header ("Game Parameters")]
    public int entitiesInEnvironment;
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
    private const string StateText = "Current state : ";

    // handle GameManager instance
    private static GameManager _instance;
    public static GameManager GetInstance()
    {
        return _instance;
    }
    private void Awake()
    {
        _instance = this;
    }

    public void UpdateStateFeedback(string state)
    {
        _instance.stateUpdate.text = StateText + state;
    }
}
