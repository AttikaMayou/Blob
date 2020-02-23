using Unity.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class Testing : MonoBehaviour
{
     private void Start()
     {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(LevelComponent),
            typeof(Translation)
        );

        NativeArray<Entity> entityArray = new NativeArray<Entity>(2000, Allocator.Temp);
        entityManager.CreateEntity(entityArchetype, entityArray);


        for (int i = 0; i < entityArray.Length; ++i)
        {
            Entity entity = entityArray[i];
            entityManager.SetComponentData(entity, new LevelComponent { level = Random.Range(10, 20) });
        }

        entityArray.Dispose();
     }
}
