using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

//Author : Attika

public class GameManager : MonoBehaviour
{
    // game variables
    [Header ("Game Parameters")]
    public int entitiesInEnvironment;
    public int nbEntitiesOnFirstRing;
    public float toleranceDistance;
    [FormerlySerializedAs("blobSpeed")] public float blobIdleSpeed;
    [FormerlySerializedAs("blobRadius")] public float blobIdleRadius;

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

    public static void UpdateStateFeedback(string state)
    {
        _instance.stateUpdate.text = StateText + state;
    }
}
