using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereState : MonoBehaviour
{
    [SerializeField] private Material myMaterial;

    public int moving = 0;
    private float threshold = 0.01f;
    Vector3 lastPos = new Vector3(0, 0, 0);
    Vector3 forwardVector = new Vector3(0, 0, 0);
    public Vector3 trans;

    // Update is called once per frame
    void Update()
    {
        Vector3 curPos = transform.position;
        if (Vector3.Distance(curPos, lastPos) >= threshold)
        {
            moving = 1;
            myMaterial.SetInt("isMoving", moving);
        }
        else
        {
            moving = 0;
            myMaterial.SetInt("isMoving", moving);
        }
        lastPos = curPos;
    }
}
