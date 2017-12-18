using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuddiesGPU : MonoBehaviour {

	public int dimension = 32;
	public Shader boidShader;
	public Shader bodyShader;
	public Shader headShader;
	public ComputeShader computeShader;
	public Camera cameraComponent;
	[Header("Boid Radius")]
	public float boidRadius = .5f;
	public float boidFollowRange = 1f;
	[Header("Boid Rules")]
	public float velocityAvoid = 2f;
	public float velocityFollow = 1f;
	public float velocityGravity = 1f;
	[Header("Boid Velocity")]
	public float velocitySpeed = .1f;
	public float velocityFriction = .9f;
	public float velocityDamping = .1f;
	public float velocityMax = 3f;

	private int kernel;
	private RenderTexture boidBuffer;
	private RenderTexture infoBuffer;
	private bool generated;
	private Fetch fetch;
	private RaycastHit raycast;
	private Ray ray;

	void Start () {
		kernel = computeShader.FindKernel("ComputeInit");
		boidBuffer = GetComputeTexture();
		infoBuffer = GetComputeTexture();
		computeShader.SetTexture(kernel, "_BoidBuffer", boidBuffer);
		computeShader.SetTexture(kernel, "_InfoBuffer", infoBuffer);
		computeShader.Dispatch(kernel, dimension/8, dimension/8, 1);
		kernel = computeShader.FindKernel("ComputeBoid");
		computeShader.SetTexture(kernel, "_BoidBuffer", boidBuffer);
		computeShader.SetTexture(kernel, "_InfoBuffer", infoBuffer);
		// generated = false;
		// fetch = GetComponent<Fetch>();
		Material material = new Material(boidShader);
		Geometry.GenerateMeshes(transform, material, dimension, 1f, 1f);
		generated = true;
	}

	void OnDrawGizmos () {
		Gizmos.DrawLine(raycast.point, raycast.point+Vector3.up);
	}
	
	void Update () {

		if (generated) {

			computeShader.SetVector("_HitPoint", Vector3.one*10000f);
			// if (Input.GetMouseButtonDown(0)) {
				ray = cameraComponent.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out raycast, 100f)) {
					computeShader.SetVector("_HitPoint", raycast.point);
				}
			// }

			computeShader.SetFloat("_Dimension", dimension);
			computeShader.SetFloat("_BoidRadius", boidRadius);
			computeShader.SetFloat("_BoidFollowRange", boidFollowRange);
			computeShader.SetFloat("_VelocityAvoid", velocityAvoid);
			computeShader.SetFloat("_VelocityFollow", velocityFollow);
			computeShader.SetFloat("_VelocityGravity", velocityGravity);
			computeShader.SetFloat("_VelocityMax", velocityMax);
			computeShader.SetFloat("_VelocitySpeed", velocitySpeed);
			computeShader.SetFloat("_VelocityFriction", velocityFriction);
			computeShader.SetFloat("_VelocityDamping", velocityDamping);
			computeShader.Dispatch(kernel, dimension/8, dimension/8, 1);

			Shader.SetGlobalTexture("_BoidBuffer", boidBuffer);
			Shader.SetGlobalTexture("_InfoBuffer", infoBuffer);
		} else {
			// if (fetch.loaded) {
				// generated = true;
				// Material material = new Material(bodyShader);
				// material.mainTexture = fetch.bodyAtlas;
				// Mesh[] bodyMeshes = Geometry.GenerateMeshes(transform, material, dimension, 1f, 1f);
				// material = new Material(headShader);
				// material.mainTexture = fetch.headAtlas;
				// Mesh[] headMeshes = Geometry.GenerateMeshes(transform, material, dimension, 1f, 1f);
				// SetupBodyFrame(bodyMeshes);
				// SetupHeadFrame(headMeshes);
			// }
		}
	}

	RenderTexture GetComputeTexture () {
		RenderTexture texture = new RenderTexture(dimension,dimension,24,RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
		texture.filterMode = FilterMode.Point;
		texture.enableRandomWrite = true;
		texture.Create();
		return texture;
	}

	void SetupHeadFrame (Mesh[] meshes) {
		foreach (Mesh mesh in meshes) {
			Vector3[] vertices = mesh.vertices;
			Vector4[] frames = new Vector4[vertices.Length];
			for (int i = 0; i+3 < vertices.Length; i += 4) {
				Vector4 frame = fetch.GetRandomHeadFrame();
				for (int v = 0; v < 4; ++v) {
					frames[i+v] = frame;
				}
			}
			mesh.tangents = frames;
		}
	}

	void SetupBodyFrame (Mesh[] meshes) {
		foreach (Mesh mesh in meshes) {
			Vector3[] vertices = mesh.vertices;
			Vector4[] frames = new Vector4[vertices.Length];
			for (int i = 0; i+3 < vertices.Length; i += 4) {
				Vector4 frame = fetch.GetRandomBodyFrame();
				for (int v = 0; v < 4; ++v) {
					frames[i+v] = frame;
				}
			}
			mesh.tangents = frames;
		}
	}
}
