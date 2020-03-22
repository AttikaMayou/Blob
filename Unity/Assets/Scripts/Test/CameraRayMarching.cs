using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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

    //---------------------------POUR DEMO---------------------------------------
    [SerializeField] private Slider smooth;



    /*
    void Start()
    {
        for(int i = 0; i < spherePositions.Count; i++)
        {
            positionScale[i] = new Vector4(spherePositions[i].position.x, spherePositions[i].position.y, spherePositions[i].position.z, spherePositions[i].localScale.x);
        }

        toShaderCustomStruct = new ToShaderStruct[spherePositions.Count];
        for (int i = 0; i < positionScale.Length; i++)
        {
            toShaderCustomStruct[i] = new ToShaderStruct
            {
                position = positionScale[i]
            };
        }
    }
    */

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

        Vector4[] sphereLocation = new Vector4[spherePositions.Count];
        for (int i = 0; i < sphereLocation.Length; i++)
            sphereLocation[i] = new Vector4(spherePositions[i].position.x, spherePositions[i].position.y, spherePositions[i].position.z, spherePositions[i].localScale.x);
        myMaterial.SetInt("numberOfSpheres", sphereLocation.Length);
        myMaterial.SetVectorArray("sphereLocation", sphereLocation);

        Graphics.Blit(source, destination, myMaterial, 0);
    }
    /*
    public void FillBuffers()
    {
        // Deuxième paramètre = taille d'un élément dans le buffer. Ici taille d'un Vector3
        // 12 = sizeof(Vector3) = 3 * sizeof(float) = 12
        buffer = new ComputeBuffer(positionScale.Length, 16);
        // On remplit le buffer avec les données contenues dans le tableau
        buffer.SetData(positionScale);

        // On initialise le Buffer.
        // La taille d'un élément est précisé dans la définition de la struct ci-dessus
        customStructBuffer = new ComputeBuffer(toShaderCustomStruct.Length, 16);
        // On passe le tableau au GPU
        customStructBuffer.SetData(toShaderCustomStruct);

    }
    */
    /*
    public void OnPostRender()
    {
        // A la fin du rendu, on relache la mémoire 
        this.buffer.Dispose();
        this.customStructBuffer.Dispose();
    }*/

    public void changeSmooth()
    {
        smoothIntensity = smooth.value;
    }

}

public struct ToShaderStruct
{
    //public Vector3 color; // Size => 12
    public Vector4 position; // Size => 16
    // Total Size => 16
}
