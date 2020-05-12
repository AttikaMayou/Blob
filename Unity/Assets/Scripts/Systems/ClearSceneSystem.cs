using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using Utils;

namespace Systems
{
    public class ClearSceneSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
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
}