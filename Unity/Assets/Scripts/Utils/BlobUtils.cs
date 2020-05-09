using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using Ray = Unity.Physics.Ray;
using RaycastHit = Unity.Physics.RaycastHit;

//Author : Attika

namespace Utils
{
    public class BlobUtils : MonoBehaviour
    {
        private static BlobUtils _instance;

        private void Awake()
        {
            if (!_instance)
                _instance = this;
        }

        // Ground layer
        public LayerMask groundMask;
        
        #region physics methods
        private static EntityManager _manager;
        private static World _world;
        private static BuildPhysicsWorld _buildPhysicsWorld;
        private static PhysicsWorld _physicsWorld;
        private static CollisionWorld _collisionWorld;
        
        public static EntityManager GetCurrentEntityManager()
        {
            return _manager ?? (_manager = World.DefaultGameObjectInjectionWorld.EntityManager);
        }

        private static World GetCurrentWorld()
        {
            return _world ?? (_world = GetCurrentEntityManager().World);
        }

        private static BuildPhysicsWorld BuildPhysicsWorld()
        {
            return _buildPhysicsWorld ?? (_buildPhysicsWorld = GetCurrentWorld().GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>());
        }

        private static PhysicsWorld GetCurrentPhysicsWorld()
        {
            _physicsWorld = BuildPhysicsWorld().PhysicsWorld;
            return _physicsWorld;
        }

        private static CollisionWorld GetCurrentCollisionWorld()
        {
            _collisionWorld = GetCurrentPhysicsWorld().CollisionWorld;
            return _collisionWorld;
        }
        
        public static CollisionFilter LayerMaskToFilter(LayerMask mask)
        {
            var filter = new CollisionFilter()
            {
                BelongsTo = (uint)mask.value,
                CollidesWith = (uint)mask.value
            };
            return filter;
        }
 
        public static CollisionFilter LayerToFilter(int layer)
        {
            if (layer == -1)
            {
                return CollisionFilter.Zero;
            }

            var mask = new BitArray32 {[layer] = true};

            var filter = new CollisionFilter()
            {
                BelongsTo = mask.Bits,
                CollidesWith = mask.Bits
            };
            return filter;
        }
        
        #endregion
        
        #region mouse methods

        private static Vector3 GetMouseWorldPosition(Vector3 screenPosition, Camera worldCamera)
        {
            var pos = worldCamera.ScreenToWorldPoint(screenPosition);
            //pos.z = worldCamera.transform.position.z;
            return pos;
        }
        
        public static Vector3 GetMouseWorldPosition()
        {
            var pos = GetMouseWorldPosition(Input.mousePosition, Camera.main);
            Debug.Log("2) Mouse pos : " + Input.mousePosition);
            Debug.Log("3) Converted in world pos : " + pos);
            return pos;
        }

        public static float3 GetMousePositionInPhysicWorld()
        {
            var hit = GetHitFromMouse(out var isThereEntity);
            return isThereEntity ?  hit.Position : float3.zero;
        }

        public static float3 GetGroundPosition(out bool haveHit)
        {
            haveHit = false;
            if (Camera.main == null) return float3.zero;
            var screenRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                
            var ray = new Ray {Origin = Input.mousePosition};

            var input = new RaycastInput()
            {
                Start = screenRay.origin,
                End = screenRay.GetPoint(100),
                Filter = BlobUtils.LayerMaskToFilter(_instance.groundMask)
            };

            if (GetCurrentCollisionWorld().CastRay(input, out var hit))
            {
                haveHit = true;
                return hit.Position;
            }

            return float3.zero;

        }

        public static float3 GetMousePositionInPhysicWorld(out bool isThereEntity)
        {
            var hit = GetHitFromMouse(out isThereEntity);
            return isThereEntity ?  hit.Position : float3.zero;
        }
        #endregion
        
        #region raycast methods
        public static Entity RayCastFromMouse(out bool isThereEntity)
        {
            var hit = GetHitFromMouse(out isThereEntity);
            return isThereEntity ? GetCurrentPhysicsWorld().Bodies[hit.RigidBodyIndex].Entity : Entity.Null;
        }
        
        public static Entity RayCast(float3 rayFrom, float3 rayTo, out bool haveHit)
        {
            var hit = GetHitFromInWorld(rayTo, rayFrom, out haveHit);
            return haveHit ? GetCurrentPhysicsWorld().Bodies[hit.RigidBodyIndex].Entity : Entity.Null;
        }

        public static RaycastHit GetHitFromMouse(out bool haveHit)
        {
            var input = RayFromMouseInWorld();

            haveHit = _collisionWorld.CastRay(input, out var hit);
            return hit;
        }

        private static RaycastHit GetHitFromInWorld(float3 rayTo, float3 rayFrom, out bool haveHit)
        {
            var input = RayFromInWorld(rayTo, rayFrom);
            
            haveHit = _collisionWorld.CastRay(input, out var hit);
            return hit;
        }

        private static RaycastInput RayFromMouseInWorld()
        {
            GetCurrentCollisionWorld(); // initialization of current physics world 
            
            var input = new RaycastInput
            {
                End = Input.mousePosition + new Vector3(0.0f, 0.0f, 100.0f),
                Filter = new CollisionFilter
                {
                    BelongsTo = ~0u, // bit mask (which layers this collider belongs to)
                    CollidesWith = ~0u, // bit mask (which layers this collider can collide with)
                    GroupIndex = 0
                },
                Start = GetMouseWorldPosition()
            };
            
            return input;
        }

        private static RaycastInput RayFromInWorld(float3 rayTo, float3 rayFrom)
        {
            GetCurrentCollisionWorld(); // initialization of current physics world 
            
            var input = new RaycastInput
            {
                End = rayTo,
                Filter = new CollisionFilter
                {
                    BelongsTo = ~0u, // bit mask (which layers this collider belongs to)
                    CollidesWith = ~0u, // bit mask (which layers this collider can collide with)
                    GroupIndex = 0
                },
                Start = rayFrom
            };
            
            return input;
        }

        public static RaycastInput RayFromInWorld(float3 rayTo, float3 rayFrom, CollisionFilter filter)
        {
            GetCurrentCollisionWorld(); // initialization of current physics world 
            
            var input = new RaycastInput
            {
                End = rayTo,
                Filter = filter,
                Start = rayFrom
            };
            
            return input;
        }
        #endregion
        
        #region mathematics methods
        private static float3 ApplyRotationToVector(float3 vec, float angle)
        {
            return Quaternion.Euler(0, 0, angle) * vec;
        }
        #endregion
       
        #region movement methods

        public static List<float3> GetPositionsForBlobEntities(float3 targetPos, int nbOfEntities, int firstRingNbEntities, 
            float firstRingMinDist)
        {
            Debug.Log("Nombre entités premier anneau : " + firstRingNbEntities);
            Debug.Log("Nombre d'entités en tout : " + nbOfEntities);
            
            InitializeRingMovementSystem(nbOfEntities, firstRingNbEntities, firstRingMinDist, 
                out var minDistance, out var entitiesPerRing);

            return GetPositionListAround(targetPos, minDistance, entitiesPerRing);
            // calculate new position for each entity with 'MovementComponent' 
            // add the new position to each entity's current position 
            // multiply it with Time.deltaTime to do it smoothly
            // maybe move this method onto 'RayCastSelectSystem' to burst compile it
        }

        public static void InitializeRingMovementSystem(int nbEntities, int nbEntitiesPerRing, float dist, 
            out float[] minDistance, out int[] entitiesPerRing)
        {
            var i = 0;

            nbEntities -= 1; // don't count the first one which is central

            var iterations = (nbEntities - 1) / nbEntitiesPerRing;
            iterations++;
            
            var distanceResult = new float[iterations];
            var countResult = new int[iterations];

            var ringIndex = 0; // index of the current ring
            
            for (i = 0; i < nbEntities; i+=nbEntitiesPerRing)
            {
                if (i >= nbEntitiesPerRing)
                {
                    nbEntitiesPerRing *= 2;
                    ++ringIndex;
                }
                distanceResult[ringIndex] = dist + dist * ringIndex;
                countResult[ringIndex] = nbEntitiesPerRing;
            }

            minDistance = distanceResult;
            entitiesPerRing = countResult;
        }

        private static List<float3> GetPositionListAround(float3 startPosition, float[] ringDistance,
            int[] ringPositionCount)
        {
            var positionList = new List<float3> {startPosition};

            for (var ring = 0; ring < ringPositionCount.Length; ++ring)
            {
                Debug.Log("ring no : " + ring);
                var ringPositionList =
                    GetPositionListAround(startPosition, ringDistance[ring], ringPositionCount[ring]);
                positionList.AddRange(ringPositionList);
            }
            
            return positionList;
        }

        private static IEnumerable<float3> GetPositionListAround(float3 startPosition, float distance, int positionCount)
        {
            var positionList = new List<float3> {startPosition};

            for (var i = 0; i < positionCount; ++i)
            {
                var angle = i * (360 / positionCount);
                var dir = BlobUtils.ApplyRotationToVector(new float3(0, 1, 0), angle);
                var position = startPosition + dir * distance;
                Debug.Log(position + " index : " + i);
                positionList.Add(position);
            }
            
            return positionList;
        }
        #endregion
        
    }
    
    public struct BitArray32
    {
        public uint Bits;
 
        public bool this[int index]
        {
            get
            {
                uint mask = 1u << index;
                return (Bits & mask) == mask;
            }
            set
            {
                uint mask = 1u << index;
                if (value)
                {
                    Bits |= mask;
                }
                else
                {
                    Bits &= ~mask;
                }
            }
        }
    }
}
