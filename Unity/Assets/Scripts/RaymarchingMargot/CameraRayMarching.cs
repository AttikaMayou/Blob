﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class CameraRayMarching : MonoBehaviour
{

    [Header("Scene infos")]
    [SerializeField] private Camera camera;
    [SerializeField] private Light light;

    [SerializeField] private List<Transform> spherePositions;// = new List<Transform>();
    private ToShaderStruct[] toShaderCustomStruct;
    private ComputeBuffer buffer;
    private ComputeBuffer customStructBuffer;


    [Header("Raymarch parameters")]
    [SerializeField] private Shader myShader;
    [SerializeField] private Material myMaterial;
    [Range(1, 1000)]
    [SerializeField] private int rayMarchStep;
    [Range(0, 3)]
    [SerializeField] private int ChooseSmoothFunction;
    [SerializeField] private float smoothIntensity;
    [SerializeField] private float radius = 3;

    //---------------------------POUR DEMO---------------------------------------
   // [SerializeField] private Slider smooth;



    [ImageEffectOpaque]
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        myMaterial.SetVector("_CamForward", transform.forward);
        myMaterial.SetVector("_CamRight", transform.right);
        myMaterial.SetVector("_CamUp", -transform.up);
        myMaterial.SetFloat("_Fov", (camera.fieldOfView * Mathf.Deg2Rad) / 2.0f);
        myMaterial.SetFloat("_Aspect", camera.aspect);

        myMaterial.SetFloat("MAX_MARCHING_STEPS", rayMarchStep);
        myMaterial.SetInt("smoothFunctionChoosed", ChooseSmoothFunction);
        myMaterial.SetFloat("k", smoothIntensity);

        myMaterial.SetVector("lightColor", light.color);
        myMaterial.SetFloat("lightIntensity", light.intensity);
        myMaterial.SetVector("lightPosition", light.transform.position);

        Vector4[] sphereLocation = new Vector4[BlobUtils.GetBlobsCurrentPositions().Count];
        for (int i = 0; i < 1; i++) //sphereLocation.Length
        {
            sphereLocation[i] = new Vector4(spherePositions[i].position.x, spherePositions[i].position.y, spherePositions[i].position.z, radius);//new Vector4(BlobUtils.GetBlobsCurrentPositions()[i].x, BlobUtils.GetBlobsCurrentPositions()[i].y, BlobUtils.GetBlobsCurrentPositions()[i].z, radius);
            //Debug.Log(BlobUtils.GetBlobsCurrentPositions()[i]);
        }
        
        myMaterial.SetInt("numberOfSpheres", sphereLocation.Length);
        Debug.Log(sphereLocation.Length);
        myMaterial.SetVectorArray("sphereLocation", sphereLocation);

        Graphics.Blit(source, destination, myMaterial, 0);
    }

    /*
    public void changeSmooth()
    {
        smoothIntensity = smooth.value;
    }
    */
}

public struct ToShaderStruct
{
    //public Vector3 color; // Size => 12
    public Vector4 position; // Size => 16
    // Total Size => 16
}
