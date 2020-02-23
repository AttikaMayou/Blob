using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class SpriteRenderer : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Translation translation) =>
        {
            Graphics.DrawMesh(Testing.Getinstance().mesh, translation.Value, Quaternion.identity, Testing.Getinstance().material, 0);
        });
    }
}
