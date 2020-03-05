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

    //TODO : handle these a better way
    [SerializeField] private List<GameObject> balls;
    [SerializeField]
    private GameObject environment;
    
    private void Start()
    {
        Initialize();
        // var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        //
        // var ballsArchetype = entityManager.CreateArchetype(
        //     typeof(Translation),
        //     typeof(LocalToWorld)
        // );
        
        //TODO : find how to get these components from game objects converted into an entity
        //and find a way to add to these fresh converted entities new and custom components (won't be too hard)
    }

    private void Initialize()
    {
        var initializer = EntitiesInitializer.GetInstance();
        initializer.FillGameObjectsToConvert(balls);
        initializer.ConvertGameObjects();
        initializer.FillGameObjectsToConvert(environment);
        initializer.ConvertGameObjects();
    }
}
