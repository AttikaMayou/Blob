using Unity.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;

public class Testing : MonoBehaviour
{
    public static Testing instance;
    public static Testing Getinstance()
    {
        return instance;
    }

    public Mesh mesh;
    public Material material;
     private void Start()
     {
        instance = this;

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(LevelComponent),
            typeof(Translation),
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(MoveComponent)
        );

        NativeArray<Entity> entityArray = new NativeArray<Entity>(10, Allocator.Temp);
        entityManager.CreateEntity(entityArchetype, entityArray);


        for (int i = 0; i < entityArray.Length; ++i)
        {
            Entity entity = entityArray[i];
            entityManager.SetComponentData(entity, new LevelComponent { level = UnityEngine.Random.Range(10, 20) }); //applique un component à une entité designéeS
            entityManager.SetComponentData(entity, new MoveComponent { moveSpeed = UnityEngine.Random.Range(1f, 2f) });
            entityManager.SetComponentData(entity, new Translation { Value = new float3(UnityEngine.Random.Range(-10f, 10f),
                                                                                        UnityEngine.Random.Range(-5f, 5f),
                                                                                        0) }); 

            entityManager.SetSharedComponentData(entity, new RenderMesh //applique un component à toutes les entities => "shared component"
            {
                mesh = mesh,
                material = material
            });
        }

        entityArray.Dispose();
     }
}
