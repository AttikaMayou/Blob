using Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Utils;

//Author : Attika

public class RaycastSelectSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        // Move blobs
        if (Input.GetMouseButtonDown(2))
        {
            // get position on the ground where user clicked
            var position = BlobUtils.GetGroundPosition(out var haveHit);
            position += new float3(0, GameManager.GetInstance().blobIdleRadius, 0);

            // if user did not hit the ground, return
            if (!haveHit) return;

            // if there is no blob in the scene, return
            if (BlobUtils.GetCurrentEntityManager().GetAllEntities().Length <= GameManager.GetInstance().entitiesInEnvironment) return;

            // get current number of blob entities (all current entities minus environment entities)
            var nbBlobEntities = BlobUtils.GetCurrentEntityManager().GetAllEntities().Length - GameManager.GetInstance().entitiesInEnvironment;
            // get a list of all positions blobs should go to, according to where the player clicked
            var targetPositions = BlobUtils.GetPositionsForBlobEntities(position, nbBlobEntities, 
                GameManager.GetInstance().nbEntitiesOnFirstRing, GameManager.GetInstance().blobIdleRadius);
            
            // assign positions and move speed to all blob units
            var positionIndex = 0;
            Entities.WithAll<BlobUnitMovement>().ForEach((Entity entity, ref BlobUnitMovement blobUnitMovement) =>
            {
                blobUnitMovement.position = targetPositions[positionIndex];
                positionIndex = (positionIndex + 1) % targetPositions.Count;
                blobUnitMovement.moveSpeed = GameManager.GetInstance().blobIdleSpeed;
                // trigger movement system by passing this true
                blobUnitMovement.move = true;
            });
        }
            
        // Attract blobs
        if (Input.GetKeyDown(KeyCode.A))
        {
            var position = BlobUtils.GetGroundPosition(out var haveHit);

            if (!haveHit) return;
                
                
        }
            
        // Give an impulse away to blobs
        if (Input.GetKeyDown(KeyCode.E))
        {
            var position = BlobUtils.GetGroundPosition(out var haveHit);

            if (!haveHit) return;
        }
            
    }
}