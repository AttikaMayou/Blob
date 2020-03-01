using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class RayMarchCam : MonoBehaviour
{
    [SerializeField] private Shader myShader;
    [SerializeField] private Material myMaterial;
    [SerializeField] private Camera myCamera;
    //TODO : transformer en liste
    //[SerializeField] private List<Transform> transformSpheres = new List<Transform>();

    [SerializeField] private Transform sphere1;
    [SerializeField] private Transform sphere2;

    private Vector4[] testPositionSphere = new Vector4[2];
    /*
    Light[] lights;
    //Texture2D lightInfo;
    ComputeBuffer lightInfo;
    public Light lightScene;
    float[] lightPositions;

    public Transform p1;
    public Transform p2;
    */
    //[ImageEffectOpaque]
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {

        if(!myMaterial)
        {
            //Copie source texture in destination texture
            Graphics.Blit(source, destination);
            return;
        }

        float fov = Mathf.Tan((myCamera.fieldOfView * 0.5f) * Mathf.Deg2Rad);

        //Set uniform camera
        myMaterial.SetMatrix("CameraToWorldMatrix", myCamera.cameraToWorldMatrix);
        myMaterial.SetMatrix("CameraFrustrum", CamFrustrum(myCamera, fov));
        myMaterial.SetVector("CameraWorldSPace", myCamera.transform.position);

        myMaterial.SetVector("_CamForward", transform.forward);
        myMaterial.SetVector("_CamRight", transform.right);
        myMaterial.SetVector("_CamUp", -transform.up);

        myMaterial.SetFloat("fov", fov);
        myMaterial.SetFloat("Ratio", myCamera.aspect * fov);

        testPositionSphere[0] = sphere1.position;
        testPositionSphere[1] = sphere2.position;

        myMaterial.SetVector("positionsSphere1", sphere1.position);
        myMaterial.SetVector("positionsSphere2", sphere2.position);

        //Destination render texture
        CustomGraphicsBlit(source, destination, myMaterial, 0);

        /*
                myMaterial.SetVector("_CamUp", -transform.up);
                myMaterial.SetFloat("_Fov", (Camera.main.fieldOfView * Mathf.Deg2Rad) / 2.0f);

                lights = FindObjectsOfType(typeof(Light)) as Light[];
                //lightInfo = new Texture2D(1, lights.Length);
                lightInfo = new ComputeBuffer(4 * lights.Length, sizeof(float));
                lightPositions = new float[4 * lights.Length];
                myMaterial.SetBuffer("_LightInfo", lightInfo);

                for (int i = 0; i < lights.Length; i++)
                {
                    Vector3 lp = lightScene.transform.position;

                    lightPositions[(i * 4) + 0] = lp.x;
                    lightPositions[(i * 4) + 1] = lp.y;
                    lightPositions[(i * 4) + 2] = lp.z;
                    lightPositions[(i * 4) + 3] = 1.0f;
                }
                lightInfo.SetData(lightPositions);

                myMaterial.SetVector("_p1", p1.position);
                myMaterial.SetVector("_p2", p2.position);

                Graphics.Blit(source, destination, myMaterial, 0);
                */
    }

    //Camera.CalculateFrustumCorners
    private Matrix4x4 CamFrustrum(Camera myCamera, float fov)
    {
        Matrix4x4 frustrum = Matrix4x4.identity;
        //permet de calculer les 4 corners du frustrum
        Vector3 goUp = Vector3.up * fov;
        Vector3 goRight = Vector3.right * fov * myCamera.aspect;

        //calcul des 4 corners
        Vector3 topLeftCorner = (-Vector3.forward - goRight + goUp);
        Vector3 topRightCorner = (-Vector3.forward + goRight + goUp);
        Vector3 DownLeftCorner = (-Vector3.forward + goRight - goUp);
        Vector3 DownRightCorner = (-Vector3.forward - goRight - goUp);

        frustrum.SetRow(0, topLeftCorner);
        frustrum.SetRow(1, topRightCorner);
        frustrum.SetRow(2, DownLeftCorner);
        frustrum.SetRow(3, DownRightCorner);

        return frustrum;
    }

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