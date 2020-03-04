using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

//Author : Attika

public class GameManager : MonoBehaviour
{
    //Gestion de l'instance du GameManager
    public static GameManager instance;
    public static GameManager GetInstance()
    {
        return instance;
    }
    private void Awake()
    {
        instance = this;
    }
    
    private void Start()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var ballsArchetype = entityManager.CreateArchetype(
            typeof(Translation),
            typeof(LocalToWorld)
        );
    }

}
