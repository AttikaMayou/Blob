using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public struct MoveComponent : IComponentData
{
    public float moveSpeed;
    public bool move;
    public float3 position;
    public float3 lastMoveDir;
}
