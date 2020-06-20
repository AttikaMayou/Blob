using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geometry : MonoBehaviour {

	private int width, height;
	private Material material;

	void Start () {
		width = Screen.width;
		height = Screen.height;
		GeneratePoints();
		material = GetComponent<MeshRenderer>().sharedMaterial;
		Debug.Log(width);
		Debug.Log(height);
	}

	void Update () {
		Camera cam = Camera.main;
		material.SetFloat("_CameraFOV", cam.fieldOfView);
		material.SetVector("_CameraPosition", cam.transform.position);
		material.SetVector("_CameraForward", cam.transform.forward);
		material.SetVector("_CameraUp", cam.transform.up);
		material.SetVector("_CameraRight", cam.transform.right);
		material.SetVector("_CameraPitchYawRoll", cam.transform.rotation.eulerAngles);
		material.SetFloat("_Width", width);
		material.SetFloat("_Height", height);
	}

	public void GeneratePoints () {
		int count = width * height;
		Vector3[] vertices = new Vector3[count];
		int[] indices = new int[count];
		int index = 0;
		for (int w = 0; w < width; ++w) {
			for (int h = 0; h < height; ++h) {
				vertices[index] = new Vector3(
					(w/(float)width)*2f-1f,
					(h/(float)height)*2f-1f,
					0f);
				indices[index] = index;
				index++;
			}
		}
		Debug.Log(vertices.Length);
		Mesh mesh = new Mesh();
		mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
		mesh.vertices = vertices;
		mesh.SetIndices(indices, MeshTopology.Points, 0);
		mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 100f);
		GetComponent<MeshFilter>().sharedMesh = mesh;
	}
}
