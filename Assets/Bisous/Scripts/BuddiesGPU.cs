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

	void Start () {
		kernel = computeShader.FindKernel("ComputeInit");
		boidBuffer = GetComputeTexture();
		computeShader.SetTexture(kernel, "_BoidBuffer", boidBuffer);
		computeShader.Dispatch(kernel, dimension/8, dimension/8, 1);

		kernel = computeShader.FindKernel("ComputeBoid");
		computeShader.SetTexture(kernel, "_BoidBuffer", boidBuffer);

		Material material = new Material(spriteShader);
		Geometry.GenerateMeshes(transform, material, dimension, 1f, 1f);
	}
	
	void Update () {
		computeShader.SetFloat("_Dimension", dimension);
		computeShader.SetFloat("_BoidRadius", boidRadius);
		computeShader.SetFloat("_VelocityAvoid", velocityAvoid);
		computeShader.SetFloat("_VelocityFollow", velocityFollow);
		computeShader.SetFloat("_VelocityGravity", velocityGravity);
		computeShader.SetFloat("_VelocityMax", velocityMax);
		computeShader.SetFloat("_VelocitySpeed", velocitySpeed);
		computeShader.SetFloat("_VelocityFriction", velocityFriction);
		computeShader.SetFloat("_VelocityDamping", velocityDamping);
		computeShader.Dispatch(kernel, dimension/8, dimension/8, 1);
		Shader.SetGlobalTexture("_BoidBuffer", boidBuffer);
	}

	RenderTexture GetComputeTexture () {
		RenderTexture texture = new RenderTexture(dimension,dimension,24,RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
		texture.filterMode = FilterMode.Point;
		texture.enableRandomWrite = true;
		texture.Create();
		return texture;
	}
}
