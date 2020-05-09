using System;
using UnityEngine;
using Unity.Physics;
using Utils;
using Plane = UnityEngine.Plane;
using Ray = Unity.Physics.Ray;
using RaycastHit = Unity.Physics.RaycastHit;

//Author : Attika

namespace Test.ECS
{
    public class DebugSelectSytem : MonoBehaviour
    {
        public static DebugSelectSytem instance;
        public LayerMask groundMask;

        private void Awake()
        {
            if (instance == null)
                instance = this;
        }

        public static Vector3 DebugMouseWorldPosition(bool physics = false)
        {
            if (Camera.main != null)
            {
                if (physics)
                {
                    UnityEngine.Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    UnityEngine.RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        return hit.point;
                    }
                }
                else
                {
                    var world = BlobUtils.GetCurrentCollisionWorld();
                    var screenRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                
                    Ray ray = new Ray {Origin = Input.mousePosition};
                    RaycastHit hit;
                    
                    var input = new RaycastInput()
                    {
                        Start = screenRay.origin,
                        End = screenRay.GetPoint(100),
                        Filter = BlobUtils.LayerMaskToFilter(instance.groundMask)
                    };

                    if (world.CastRay(input, out hit))
                    {
                        return hit.Position;
                    }
                }
            }
            
            return -Vector3.one;
        }
    }
}
