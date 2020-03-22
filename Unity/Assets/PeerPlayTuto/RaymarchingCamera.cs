﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class RaymarchingCamera : SceneViewFilter
{

    [SerializeField]
    private Shader _shader;

    public Material _raymarchMaterial
    {
        get
        {
            if (!_raymarchMat && _shader)
            {
                _raymarchMat = new Material(_shader);
                _raymarchMat.hideFlags = HideFlags.HideAndDontSave;
            }

            return _raymarchMat;
        }
    }
    private Material _raymarchMat;

    public Camera _camera
    {
        get
        {
            if (!_cam)
                _cam = GetComponent<Camera>();
            return _cam;
        }
    }
    private Camera _cam;

    [Header("Camera")]
    public float _maxDistance;
    [Range(1, 300)] public int _maxIterations;
    [Range(0.1f, 0.001f)] public float _accuracy;

    [Header("Objects")]
    public List<GameObject> _spheres;
    private Vector4[] _spheresPos = new Vector4[8];

    [Header("Light")]
    public Transform _light;
    public Color _lightCol;
    public float _lightIntensity;

    [Header("Shadow")]
    [Range(1, 4)] public float _shadowIntensity;
    public Vector2 _shadowDistance;
    [Range(1, 128)] public float _shadowPenumbra;

    [Header("Ambiant occlusion")]
    [Range(0.01f, 10.0f)] public float _aoStepSize;
    [Range(1, 5)] public int _aoIterations;
    [Range(0, 1)] public float _aoIntensity;

    [Header("Reflection")]
    [Range(0, 2)] public int _reflectionCount;
    [Range(0, 1)] public float _reflectionIntensity;
    [Range(0, 1)] public float _envReflectionIntensity;
    public Cubemap _reflectionCube;

    [Header("Color")]
    public Gradient _sphereGradiant;
    private Color[] _sphereColor = new Color[8];
    [Range(0, 4)] public float _colorIntensity;

    [Header("SDF")]
    public float _sphereSmooth;

    public GameObject _plane;


    [ImageEffectOpaque]
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!_raymarchMaterial)
        {
            Graphics.Blit(source, destination);
            return;
        }

        //Set spheres property
        for (int i = 0; i < 8; i++)
        {
            Vector3 pos = _spheres[i].transform.position;
            _spheresPos[i] = new Vector4(pos.x, pos.y, pos.z, _spheres[i].transform.localScale.x); 
            _sphereColor[i] = _sphereGradiant.Evaluate(1f / 8 * i);
        }

        Color planeColor = _plane.GetComponent<MeshRenderer>().sharedMaterial.color;

        // Camera
        _raymarchMaterial.SetMatrix("_CamFrustum", GetFrustumCorners(_camera));
        _raymarchMaterial.SetMatrix("_CamToWorld", _camera.cameraToWorldMatrix);
        _raymarchMaterial.SetFloat("_maxDistance", _maxDistance);
        _raymarchMaterial.SetVector("_camPos", this.transform.position);
        _raymarchMaterial.SetInt("_maxIterations", _maxIterations);
        _raymarchMaterial.SetFloat("_accuracy", _accuracy);


        //Light
        _raymarchMaterial.SetVector("_lightDir", _light ? _light.forward : Vector3.down);
        _raymarchMaterial.SetColor("_lightCol", _lightCol);
        _raymarchMaterial.SetFloat("_lightIntensity", _lightIntensity);

        //Shadows
        _raymarchMaterial.SetVector("_shadowDistance", _shadowDistance);
        _raymarchMaterial.SetFloat("_shadowIntensity", _shadowIntensity);
        _raymarchMaterial.SetFloat("_shadowPenumbra", _shadowPenumbra);

        //Ambiant occlusion
        _raymarchMaterial.SetFloat("_aoStepSize", _aoStepSize);
        _raymarchMaterial.SetInt("_aoIterations", _aoIterations);
        _raymarchMaterial.SetFloat("_aoIntensity", _aoIntensity);

        //Reflection
        _raymarchMaterial.SetInt("_reflectionCount", _reflectionCount);
        _raymarchMaterial.SetFloat("_reflectionIntensity", _reflectionIntensity);
        _raymarchMaterial.SetFloat("_envReflectionIntensity", _envReflectionIntensity);
        _raymarchMaterial.SetTexture("_reflectionCube", _reflectionCube);

        //Color
        _raymarchMaterial.SetColorArray("_sphereColor", _sphereColor);
        _raymarchMaterial.SetFloat("_colorIntensity", _colorIntensity);
        _raymarchMaterial.SetColor("_planeColor", planeColor);

        //SDF
        _raymarchMaterial.SetVectorArray("_spheres", _spheresPos);
        _raymarchMaterial.SetFloat("_sphereSmooth", _sphereSmooth);


        CustomGraphicsBlit(source, destination, _raymarchMaterial, 0);
    }

    /// \brief Stores the normalized rays representing the camera frustum in a 4x4 matrix.  Each row is a vector.
    /// 
    /// The following rays are stored in each row (in eyespace, not worldspace):
    /// Top Left corner:     row=0
    /// Top Right corner:    row=1
    /// Bottom Right corner: row=2
    /// Bottom Left corner:  row=3
    private Matrix4x4 GetFrustumCorners(Camera cam)
    {
        float camAspect = cam.aspect;

        Matrix4x4 frustumCorners = Matrix4x4.identity;

        float fov = Mathf.Tan((cam.fieldOfView * 0.5f) * Mathf.Deg2Rad);

        Vector3 toRight = Vector3.right * fov * camAspect;
        Vector3 toTop = Vector3.up * fov;

        Vector3 topLeft = (-Vector3.forward - toRight + toTop);
        Vector3 topRight = (-Vector3.forward + toRight + toTop);
        Vector3 bottomRight = (-Vector3.forward + toRight - toTop);
        Vector3 bottomLeft = (-Vector3.forward - toRight - toTop);

        frustumCorners.SetRow(0, topLeft);
        frustumCorners.SetRow(1, topRight);
        frustumCorners.SetRow(2, bottomRight);
        frustumCorners.SetRow(3, bottomLeft);

        return frustumCorners;
    }

    /// \brief Custom version of Graphics.Blit that encodes frustum corner indices into the input vertices.
    /// 
    /// In a shader you can expect the following frustum cornder index information to get passed to the z coordinate:
    /// Top Left vertex:     z=0, u=0, v=0
    /// Top Right vertex:    z=1, u=1, v=0
    /// Bottom Right vertex: z=2, u=1, v=1
    /// Bottom Left vertex:  z=3, u=1, v=0
    /// 
    /// \warning You may need to account for flipped UVs on DirectX machines due to differing UV semantics
    ///          between OpenGL and DirectX.  Use the shader define UNITY_UV_STARTS_AT_TOP to account for this.
    static void CustomGraphicsBlit(RenderTexture source, RenderTexture dest, Material fxMaterial, int passNr)
    {
        RenderTexture.active = dest;

        fxMaterial.SetTexture("_MainTex", source);

        GL.PushMatrix();
        GL.LoadOrtho(); // Note: z value of vertices don't make a difference because we are using ortho projection

        fxMaterial.SetPass(passNr);

        GL.Begin(GL.QUADS);

        // Here, GL.MultitexCoord2(0, x, y) assigns the value (x, y) to the TEXCOORD0 slot in the shader.
        // GL.Vertex3(x,y,z) queues up a vertex at position (x, y, z) to be drawn.  Note that we are storing
        // our own custom frustum information in the z coordinate.
        GL.MultiTexCoord2(0, 0.0f, 0.0f);
        GL.Vertex3(0.0f, 0.0f, 3.0f); // BL

        GL.MultiTexCoord2(0, 1.0f, 0.0f);
        GL.Vertex3(1.0f, 0.0f, 2.0f); // BR

        GL.MultiTexCoord2(0, 1.0f, 1.0f);
        GL.Vertex3(1.0f, 1.0f, 1.0f); // TR

        GL.MultiTexCoord2(0, 0.0f, 1.0f);
        GL.Vertex3(0.0f, 1.0f, 0.0f); // TL

        GL.End();
        GL.PopMatrix();
    }
}