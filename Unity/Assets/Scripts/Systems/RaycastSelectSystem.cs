using Components;
using Unity.Entities;
using UnityEngine;
using Utils;

//Author : Attika

namespace Systems
{
    public class RaycastSelectSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            // if user did not clicked, return
            if (!Input.GetMouseButtonDown(2)) return;
            
            // get position on the ground where user clicked
            var position = BlobUtils.GetGroundPosition(out var haveHit);

            // if user did not hit the ground, return
            if (!haveHit) return;

            // if there is no blob in the scene
            if (BlobUtils.GetCurrentEntityManager().GetAllEntities().Length <= GameManager.GetInstance().entitiesInEnvironment) return;

            // get current number of blob entities (all current entities minus environment entities)
            var nbBlobEntities = BlobUtils.GetCurrentEntityManager().GetAllEntities().Length - GameManager.GetInstance().entitiesInEnvironment;
            // get a list of all positions blobs should go to, according to where the player clicked
            var targetPositions = BlobUtils.GetPositionsForBlobEntities(position, nbBlobEntities, 
                GameManager.GetInstance().nbEntitiesOnFirstRing, GameManager.GetInstance().minDistanceBetweenBlobs);
            
            // assign positions and move speed to all blob units
            var positionIndex = 0;
            Entities.WithAll<BlobUnitMovement>().ForEach((Entity entity, ref BlobUnitMovement blobUnitMovement) =>
            {
                blobUnitMovement.position = targetPositions[positionIndex];
                positionIndex = (positionIndex + 1) % targetPositions.Count;
                blobUnitMovement.moveSpeed = GameManager.GetInstance().blobSpeed;
                // trigger movement system by passing this true
                blobUnitMovement.move = true;
            });
        }
    }
}