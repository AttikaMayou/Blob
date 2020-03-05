using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

//Author : Attika

public class GameManager : MonoBehaviour
{
    //Handle GameManager unique instance
    private static GameManager _instance;
    public static GameManager GetInstance()
    {
        return _instance;
    }
    private void Awake()
    {
        _instance = this;
    }
    
    private void Start()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var ballsArchetype = entityManager.CreateArchetype(
            typeof(Translation),
            typeof(LocalToWorld)
        );
        
        //TODO : find how to get these components from game objects converted into an entity
        //and find a way to add to these fresh converted entities new and custom components (won't be too hard)
    }

}
