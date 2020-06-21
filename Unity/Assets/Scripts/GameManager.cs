﻿using UnityEngine;

//Author : Attika

public class GameManager : MonoBehaviour
{
    // game variables
    [Header ("Game Parameters")]
    public int entitiesInEnvironment;
    public int nbEntitiesOnFirstRing;
    public float toleranceDistance;
    public float blobSpeed;
    public float blobRadius;
    
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
}
