﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Utils;

[RequireComponent(typeof(Camera))]
public class RaymarchingCameraShadow : SceneViewFilter
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
    private Vector4[] _spheresPos;

    [Header("Light")]
    public Light _light;

    [Header("Shadow")]
    [Range(1, 4)] public float _shadowIntensity;
    public Vector2 _shadowDistance;
    [Range(1, 128)] public float _shadowPenumbra;

    [Header("Ambiant occlusion")]
    [Range(0.01f, 10.0f)] public float _aoStepSize;
    [Range(1, 5)] public int _aoIterations;
    [Range(0, 1)] public float _aoIntensity;

    [Header("Reflection")]
    public Slider reflectionCountSlider;
    [Range(0, 2)] public int _reflectionCount = 2;
    [Range(0, 1)] public float _reflectionIntensity;
    [Range(0, 1)] public float _envReflectionIntensity;
    public Cubemap _reflectionCube;

    [Header("Color")]
    public Gradient _sphereGradiant;
    private Color[] _sphereColor;
    [Range(0, 4)] public float _colorIntensity;

    [Header("SDF")]
    public Slider smoothSlider;
    [Range(0, 10)] public float _sphereSmooth = 0.7f;

    [ImageEffectOpaque]
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!_raymarchMaterial)
        {
            Graphics.Blit(source, destination);
            return;
        }

        _spheresPos = new Vector4[BlobUtils.GetBlobsCurrentPositions().Count];
        int nbSphere = BlobUtils.GetBlobsCurrentPositions().Count;

        if (nbSphere > 0)
        {
            _sphereColor = new Color[nbSphere];

            for (int i = 0; i < BlobUtils.GetBlobsCurrentPositions().Count; i++)
            {
                _spheresPos[i] = BlobUtils.GetBlobsCurrentPositions()[i];
                _sphereColor[i] = _sphereGradiant.Evaluate(1f / 8 * (i % 8));
            }
        }
        else
        {
            _spheresPos = new Vector4[1];
            _sphereColor = new Color[1];
        }
        //_spheresPos = new Vector4[1];
        //_spheresPos[0] = new Vector4(0, 0, 0, 1);

        //Set spheres property
        //if (_spheres.Count == 0)
        //{
        //    _spheresPos = new Vector4[8];
        //    _sphereColor = new Color[8];

        //    for (int i = 0; i < 8; i++)
        //    {
        //        Vector3 pos = Vector3.zero;
        //        _spheresPos[i] = new Vector4(pos.x, pos.y, pos.z, 4.0f);
        //        _sphereColor[i] = _sphereGradiant.Evaluate(1f / 8 * i);
        //    }
        //}
        //else
        //{

        //    _spheresPos = new Vector4[_spheres.Count];
        //    _sphereColor = new Color[_spheres.Count];

        //    for (int i = 0; i < _spheres.Count; i++)
        //    {
        //        Vector3 pos = _spheres[i].transform.position;
        //        _spheresPos[i] = new Vector4(pos.x, pos.y, pos.z, _spheres[i].transform.localScale.x); 
        //        _sphereColor[i] = _sphereGradiant.Evaluate(1f / 8 * i % 8);
        //    }
        //}

        // Camera
        _raymarchMaterial.SetMatrix("_CamFrustum", GetFrustumCorners(_camera));
        _raymarchMaterial.SetMatrix("_CamToWorld", _camera.cameraToWorldMatrix);
        _raymarchMaterial.SetFloat("_maxDistance", _maxDistance);
        _raymarchMaterial.SetVector("_camPos", this.transform.position);
        _raymarchMaterial.SetVector("_camPosWorld", transform.TransformPoint(this.transform.position));
        _raymarchMaterial.SetInt("_maxIterations", _maxIterations);
        _raymarchMaterial.SetFloat("_accuracy", _accuracy);


        //Light
        _raymarchMaterial.SetVector("_lightDir", _light ? _light.transform.forward : Vector3.down);
        _raymarchMaterial.SetVector("_LightDirectionRight", _light.transform.right.normalized);
        _raymarchMaterial.SetVector("_LightDirectionUp", _light.transform.up.normalized);
        _raymarchMaterial.SetColor("_lightCol", _light.color);
        _raymarchMaterial.SetFloat("_lightIntensity", _light.intensity);

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

        //SDF
        _raymarchMaterial.SetVectorArray("_spheres", _spheresPos);
        _raymarchMaterial.SetInt("_nbSphere", nbSphere);
        _raymarchMaterial.SetFloat("_sphereSmooth", _sphereSmooth);

        CustomGraphicsBlit(source, destination, _raymarchMaterial, 0);
    }

    void Start()
    {
        
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

    #region UI_FUNCTIONS

    public void UpdateSphereSmooth()
    {
        _sphereSmooth = smoothSlider.value;
    }

    public void UpdateReflectionCount()
    {
        _reflectionCount = (int)reflectionCountSlider.value;
    }

    #endregion
}