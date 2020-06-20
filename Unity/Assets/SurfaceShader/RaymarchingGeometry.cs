using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class RaymarchingGeometry : MonoBehaviour
{
	private int width, height;
	private Material material;

	[Range(1, 2000)]
	[SerializeField] private int rayMarchStep = 1000;
	[Range(0, 10)] [SerializeField] private float smoothIntensity;

	void Start()
	{
		width = Screen.width;
		height = Screen.height;
		GeneratePoints();
		material = GetComponent<MeshRenderer>().sharedMaterial;
		Debug.Log(width);
		Debug.Log(height);
	}

	// Update is called once per frame
	void Update()
    {
		Camera cam = Camera.main;
		material.SetVector("_CamForward", transform.forward);
		material.SetVector("_CamRight", transform.right);
		material.SetVector("_CamUp", -transform.up);
		material.SetVector("_CamPosition", transform.position);
		material.SetFloat("_Fov", (cam.fieldOfView * Mathf.Deg2Rad) / 2.0f);
		material.SetFloat("_Width", width);
		material.SetFloat("_Height", height);

		material.SetFloat("MAX_MARCHING_STEPS", rayMarchStep);
		material.SetFloat("k", smoothIntensity);

		//Vector4[] sphereLocation = new Vector4[BlobUtils.GetBlobsCurrentPositions().Count];
		//if (BlobUtils.GetBlobsCurrentPositions().Count > 0)
		//{
		//	for (int i = 0; i < BlobUtils.GetBlobsCurrentPositions().Count; i++)
		//		sphereLocation[i] = BlobUtils.GetBlobsCurrentPositions()[i];
		//}
		//else
		//	sphereLocation = new Vector4[1];

		Vector4[] sphereLocation = new Vector4[3] {new Vector4(0, 0, 0, 2), new Vector4(2, 0, 0, 2), new Vector4(0, 2, 0, 2) };
		material.SetInt("numberOfSpheres", sphereLocation.Length);
		material.SetVectorArray("sphereLocation", sphereLocation);
	}

	public void GeneratePoints()
	{
		int count = width * height;
		Vector3[] vertices = new Vector3[count];
		int[] indices = new int[count];
		int index = 0;
		for (int w = 0; w < width; ++w)
		{
			for (int h = 0; h < height; ++h)
			{
				vertices[index] = new Vector3(
					(w / (float)width) * 2f - 1f,
					(h / (float)height) * 2f - 1f,
					0f);
				indices[index] = index;
				index++;
			}
		}
		Mesh mesh = new Mesh();
		mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
		mesh.vertices = vertices;
		mesh.SetIndices(indices, MeshTopology.Points, 0);
		mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 100f);
		GetComponent<MeshFilter>().sharedMesh = mesh;
	}
}
