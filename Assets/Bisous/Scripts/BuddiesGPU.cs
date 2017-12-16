using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuddiesGPU : MonoBehaviour {

	public int dimension = 32;
	public Shader spriteShader;
	public ComputeShader computeShader;
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
	private bool generated;
	private Fetch fetch;

	void Start () {
		kernel = computeShader.FindKernel("ComputeInit");
		boidBuffer = GetComputeTexture();
		computeShader.SetTexture(kernel, "_BoidBuffer", boidBuffer);
		computeShader.Dispatch(kernel, dimension/8, dimension/8, 1);
		kernel = computeShader.FindKernel("ComputeBoid");
		computeShader.SetTexture(kernel, "_BoidBuffer", boidBuffer);
		generated = false;
		fetch = GetComponent<Fetch>();
	}
	
	void Update () {
		if (generated) {
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
		} else {
			if (fetch.loaded) {
				generated = true;
				Material material = new Material(spriteShader);
				Mesh[] meshes = Geometry.GenerateMeshes(transform, material, dimension, 1f, 1f);
				SetupBodyFrame(meshes);
			}
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
