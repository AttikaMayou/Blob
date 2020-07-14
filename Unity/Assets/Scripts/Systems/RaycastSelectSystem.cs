using System;
using Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Utils;
using BlobState = Components.BlobInfosComponent.BlobState;

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
            switch (BlobUtils.GetMajorState())
            {
                case BlobState.Idle:
                    position += new float3(0, GameManager.GetInstance().blobIdleRadius, 0);
                    break;
                case BlobState.Liquid:
                    position += new float3(0, GameManager.GetInstance().blobLiquidRadius, 0);
                    break;
                case BlobState.Viscous:
                    position += new float3(0, GameManager.GetInstance().blobViscousRadius, 0);
                    break;
                default:
                    position += new float3(0, GameManager.GetInstance().blobIdleRadius, 0);
                    break;
            }

            // if user did not hit the ground, return
            if (!haveHit) return;

            // if there is no blob in the scene, return
            if (GameManager.GetInstance().GetCurrentBlobCount() <= 0) return;

            // get current number of blob entities
            var nbBlobEntities = GameManager.GetInstance().GetCurrentBlobCount();
            // get a list of all positions blobs should go to, according to where the player clicked
            var targetPositions = BlobUtils.GetPositionsForBlobEntities(position, nbBlobEntities, 
                GameManager.GetInstance().nbEntitiesOnFirstRing, GameManager.GetInstance().blobIdleRadius);
            
            // assign positions and move speed to all blob units
            var positionIndex = 0;
            Entities.WithAll<BlobUnitMovement, BlobUnitedComponent>().ForEach((Entity entity, ref BlobUnitMovement blobUnitMovement, ref BlobUnitedComponent blobUnited) =>
            {       
                // update united status
                blobUnited.united = true;

                // move blobs
                blobUnitMovement.position = targetPositions[positionIndex];
                positionIndex = (positionIndex + 1) % targetPositions.Count;
                switch (BlobUtils.GetMajorState())
                {
                    case BlobState.Idle:
                        blobUnitMovement.moveSpeed = GameManager.GetInstance().blobIdleSpeed;
                        break;
                    case BlobState.Liquid:
                        blobUnitMovement.moveSpeed = GameManager.GetInstance().blobLiquidSpeed;
                        break;
                    case BlobState.Viscous:
                        blobUnitMovement.moveSpeed = GameManager.GetInstance().blobViscousSpeed;
                        break;
                    default:
                        blobUnitMovement.moveSpeed = GameManager.GetInstance().blobIdleSpeed;
                        break;
                }
                // trigger movement system by passing this true
                blobUnitMovement.move = true;
            });
        }
    }
}