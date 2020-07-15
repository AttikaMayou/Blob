using Unity.Entities;
using UnityEngine;
using Utils;

//Author : Attika

public class ClearSceneSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            CleanWorldFromEntities();
        }
    }

    private static void CleanWorldFromEntities()
    {
        var manager = BlobUtils.GetCurrentEntityManager();
        manager.DestroyEntity(manager.UniversalQuery);
    }
}