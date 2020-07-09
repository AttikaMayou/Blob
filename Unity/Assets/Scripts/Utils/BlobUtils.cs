using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Physics;
using Unity.Physics.Systems;

//Author : Attika

namespace Utils
{
    public class BlobUtils : MonoBehaviour
    {
        // handle BlobUtils instance
        private static BlobUtils _instance;
        private void Awake()
        {
            if (!_instance)
                _instance = this;
        }

        #region Public Variables
        
        // Ground layer
        public LayerMask groundMask;
        
        //Current positions of all blobs
        public List<Vector4> currentBlobPositions;
        
        #endregion

        #region physics methods
        
        // physic system variables
        private static EntityManager _manager;
        private static World _world;
        private static BuildPhysicsWorld _buildPhysicsWorld;
        private static PhysicsWorld _physicsWorld;
        private static CollisionWorld _collisionWorld;
        
        /// <summary>
        /// Get the current Entity Manager
        /// </summary>
        /// <returns></returns>
        public static EntityManager GetCurrentEntityManager()
        {
            return _manager ?? (_manager = World.DefaultGameObjectInjectionWorld.EntityManager);
        }

        // get current world
        private static World GetCurrentWorld()
        {
            return _world ?? (_world = GetCurrentEntityManager().World);
        }

        // build the physic world
        private static BuildPhysicsWorld BuildPhysicsWorld()
        {
            return _buildPhysicsWorld ?? (_buildPhysicsWorld = GetCurrentWorld().GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>());
        }

        // get current physic world
        private static PhysicsWorld GetCurrentPhysicsWorld()
        {
            _physicsWorld = BuildPhysicsWorld().PhysicsWorld;
            return _physicsWorld;
        }

        // get collision world
        private static CollisionWorld GetCurrentCollisionWorld()
        {
            _collisionWorld = GetCurrentPhysicsWorld().CollisionWorld;
            return _collisionWorld;
        }
        
        /// <summary>
        /// Convert a layer mask in classic unity physic system into a filter for ecs physic system
        /// </summary>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static CollisionFilter LayerMaskToFilter(LayerMask mask)
        {
            var filter = new CollisionFilter()
            {
                BelongsTo = (uint)mask.value,
                CollidesWith = (uint)mask.value
            };
            return filter;
        }
 
        /// <summary>
        /// Convert a layer in classic unity physic system into a filter for ecs physic system
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get the exact position on ground where user point at
        /// </summary>
        /// <param name="haveHit"> return false if ground was not hovered by mouse, otherwise return true </param>
        /// <returns></returns>
        public static float3 GetGroundPosition(out bool haveHit)
        {
            haveHit = false;
            if (Camera.main == null) return float3.zero;
            
            var screenRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            var input = new RaycastInput()
            {
                Start = screenRay.origin,
                End = screenRay.GetPoint(100),
                Filter = LayerMaskToFilter(_instance.groundMask)
            };

            if (GetCurrentCollisionWorld().CastRay(input, out var hit))
            {
                haveHit = true;
                return hit.Position;
            }

            return float3.zero;

        }

        #endregion
        
        #region mathematics methods
        
        private static float3 ApplyRotationToVector(float3 vec, float angle)
        {
            return Quaternion.Euler(0, angle, 0) * vec;
        }
        
        #endregion
       
        #region movement methods

        /// <summary>
        /// Get the positions list to organize blobs
        /// </summary>
        /// <param name="targetPos"> central position </param>
        /// <param name="nbOfEntities"> number of positions to find </param>
        /// <param name="firstRingNbEntities"> number of positions on first ring around central position </param>
        /// <param name="firstRingMinDist"> distance between position </param>
        /// <returns></returns>
        public static List<float3> GetPositionsForBlobEntities(float3 targetPos, int nbOfEntities, int firstRingNbEntities, 
            float firstRingMinDist)
        {
            InitializeRingMovementSystem(nbOfEntities, firstRingNbEntities, firstRingMinDist, 
                out var minDistance, out var entitiesPerRing);

            return GetPositionListAround(targetPos, minDistance, entitiesPerRing);
        }

        // calculate ring parameters according to initial ones
        private static void InitializeRingMovementSystem(int nbEntities, int nbEntitiesPerRing, float dist, 
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

        // get all positions
        private static List<float3> GetPositionListAround(float3 startPosition, IReadOnlyList<float> ringDistance,
            IReadOnlyList<int> ringPositionCount)
        {
            var positionList = new List<float3> {startPosition};

            for (var ring = 0; ring < ringPositionCount.Count; ++ring)
            {
                var ringPositionList =
                    GetPositionListAround(startPosition, ringDistance[ring], ringPositionCount[ring]);
                positionList.AddRange(ringPositionList);
            }
            
            return positionList;
        }

        // get position of a ring
        private static IEnumerable<float3> GetPositionListAround(float3 startPosition, float distance, int positionCount)
        {
            var positionList = new List<float3> {};

            for (var i = 0; i < positionCount; ++i)
            {
                var angle = i * (360 / positionCount);
                var dir = ApplyRotationToVector(new float3(1, 0, 0), angle);
                var position = startPosition + dir * distance;
                positionList.Add(position);
            }
            
            return positionList;
        }

        public static void UpdateBlobPositions(List<float3> positions, List<float> radius)
        {
            var updatedPositions = new List<Vector4>();
            
            for(var i = 0; i < positions.Count; ++i)
            {
                updatedPositions.Add(new Vector4(positions[i].x, 
                    positions[i].y,
                    positions[i].z, 
                    radius[i]));
            }

            _instance.currentBlobPositions = updatedPositions;
        }

        public static List<Vector4> GetBlobsCurrentPositions()
        {
            return _instance != null ? _instance.currentBlobPositions : null;
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
