using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Utils;
using UnityEngine.Rendering;
using BlobState = Components.BlobInfosComponent.BlobState;
using UnityEditor;

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
    [Range(2, 100)] public float _Glossiness = 50;
    [Range(0, 2)] public int _reflectionCount = 2;
    [Range(0, 1)] public float _reflectionIntensity;
    [Range(0, 1)] public float _envReflectionIntensity;
    public Cubemap _reflectionCube;

    [Header("Color")]
    public Gradient _gradiantViscious;
    public Gradient _gradiantIdle;
    public Gradient _gradiantLiquid;
    private Color[] _sphereColor;
    [Range(0, 4)] public float _colorIntensity;

    [Header("SDF")]
   // public Slider smoothSlider;
    public float _sphereSmooth = 0.00f;
    public float _sphereSmoothIdle = 0.50f;
    public float _sphereSmoothViscious = 0.20f;
    public float _sphereSmoothLiquid = 0.70f;

    private int nbLiquid = 0;
    private int nbViscous = 0;
    private int nbIdle = 0;

    private Color color;
    private float t = 0;


    [ImageEffectOpaque]
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!_raymarchMaterial)
        {
            Graphics.Blit(source, destination);
            return;
        }

        int nbSphere = BlobUtils.GetBlobsCurrentPositions().Count;
        Texture2D _spheresPos = new Texture2D(nbSphere, 1, TextureFormat.RGBAFloat, false);
        _spheresPos.filterMode = FilterMode.Point;
        _spheresPos.wrapMode = TextureWrapMode.Clamp;

        Texture2D _colors = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        _spheresPos.filterMode = FilterMode.Point;
        _spheresPos.wrapMode = TextureWrapMode.Clamp;

        Color[] pos = new Color[nbSphere];

        //Proportionnal radius
        GameManager gm = GameManager.GetInstance();
        gm.GetBlobCounts(out nbIdle, out nbLiquid, out nbViscous);
        color = new Color();

        for (int i = 0; i < nbSphere; i++)
        {
            //color = Mathf.Lerp(oldRadius[i], newRadius, t);

            _spheresPos.SetPixel(i, 0, new Color(BlobUtils.GetBlobsCurrentPositions()[i].x * 0.001f,
                                                 BlobUtils.GetBlobsCurrentPositions()[i].y * 0.001f,
                                                 BlobUtils.GetBlobsCurrentPositions()[i].z * 0.001f,
                                                 BlobUtils.GetBlobsCurrentPositions()[i].w * 0.001f));
            //Moyenne des couleurs
            switch (BlobUtils.GetBlobCurrentStates()[i])
            {
                case BlobState.Liquid:
                    color += _gradiantLiquid.Evaluate(1f / 8 * (i % 8));
                    break;

                case BlobState.Viscous:
                    color += _gradiantViscious.Evaluate(1f / 8 * (i % 8));
                    break;

                default:
                    color += _gradiantIdle.Evaluate(1f / 8 * (i % 8));
                    break;
            }
        }

        _colors.SetPixel(0, 0, color / nbSphere);

        //Pour le lerp
        if (nbSphere != 0 && t <= 0.999)
            t += 0.2f * Time.deltaTime;
        if (t > 0.999)
            t = 0;

        _spheresPos.Apply();
        _colors.Apply();
        
        //SmoothFunction
        switch (BlobUtils.GetMajorState())
        {
            case BlobState.Liquid:
                _sphereSmooth = _sphereSmooth - 0.01f;
                if (_sphereSmooth < _sphereSmoothLiquid)
                    _sphereSmooth = _sphereSmoothLiquid;
                break;

            case BlobState.Viscous:
                _sphereSmooth = _sphereSmooth + 0.01f;
                if (_sphereSmooth > _sphereSmoothViscious)
                    _sphereSmooth = _sphereSmoothViscious;
                break;

            default:
                _sphereSmooth = _sphereSmooth;
                break;
        }
        
        // Camera
        _raymarchMaterial.SetMatrix("_CamFrustum", GetFrustumCorners(_camera));
        _raymarchMaterial.SetMatrix("_CamToWorld", _camera.cameraToWorldMatrix);
        _raymarchMaterial.SetFloat("_maxDistance", _maxDistance);
        _raymarchMaterial.SetVector("_camPos", this.transform.position);
        _raymarchMaterial.SetInt("_maxIterations", _maxIterations);
        _raymarchMaterial.SetFloat("_accuracy", _accuracy);

        //Light
        _raymarchMaterial.SetVector("_lightDir", _light ? _light.transform.forward : Vector3.down);
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
        _raymarchMaterial.SetFloat("_Glossiness", _Glossiness);
        _raymarchMaterial.SetInt("_reflectionCount", _reflectionCount);
        _raymarchMaterial.SetFloat("_reflectionIntensity", _reflectionIntensity);
        _raymarchMaterial.SetFloat("_envReflectionIntensity", _envReflectionIntensity);
        _raymarchMaterial.SetTexture("_reflectionCube", _reflectionCube);

        //Color
        _raymarchMaterial.SetTexture("_sphereColor", _colors);
        _raymarchMaterial.SetFloat("_colorIntensity", _colorIntensity);

        //SDF
        _raymarchMaterial.SetTexture("_spheres", _spheresPos);
        _raymarchMaterial.SetInt("_nbSphere", nbSphere);
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

    #region UI_FUNCTIONS


    public void UpdateReflectionCount()
    {
        _reflectionCount = (int)reflectionCountSlider.value;
    }

    #endregion
}