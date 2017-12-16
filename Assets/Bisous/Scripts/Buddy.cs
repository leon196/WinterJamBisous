using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buddy {

	public GameObject gameObject;
	public GameObject body;
	public GameObject head;

	public Material bodyMaterial;
	public Material headMaterial;
	public Material[] materials;

	public Vector2 seed;
	public float size;
	public float avoidScale;
	public float targetScale;
	public float followScale;
	public Vector2 targetDir;
	public Vector2 position;
	public Vector2 velocity;
	public float ground;
	public float jump;
	public float jumpDelta;
	public Transform lookAt;
	public bool kissed;

	public Buddy (Material materialHead, Material materialBody, Transform root = null) {

		gameObject = new GameObject("Buddy");
		BuddyBehaviour bb = gameObject.AddComponent<BuddyBehaviour>();
		bb.buddy = this;
		gameObject.transform.parent = root;
		position = new Vector2(UnityEngine.Random.Range(-2f,2f), UnityEngine.Random.Range(-2f,2f));
		seed = position;
		Vector3 pos = new Vector3(0,0,0);
		pos.x = position.x;
		pos.z = position.y;
		gameObject.transform.localPosition = pos;

		body = Geometry.GetQuad("Body", materialBody, gameObject.transform);
		GameObject.Destroy(body.GetComponent<Collider>());
		head = Geometry.GetQuad("Head", materialHead, gameObject.transform);

		size = UnityEngine.Random.Range(.5f,.9f);
		avoidScale = 3f;//UnityEngine.Random.Range(.6f,.8f);
		targetScale = UnityEngine.Random.Range(.6f,.8f);
		followScale = UnityEngine.Random.Range(.6f,.8f);
		targetDir = Vector2.zero;

		ground = size * 6f;

		bodyMaterial = materialBody;
		headMaterial = materialHead;
		materials = new Material[] { materialBody, materialHead };

		kissed = false;

	}

	public void Update () {
		SetFloat("_Size", size);
		SetVector("_Seed", seed);
		SetFloat("_Kissed", kissed?1f:0f);
		Vector3 pos = gameObject.transform.localPosition;
		pos.x = position.x;
		pos.y = ground + jump;
		pos.z = position.y;
		head.transform.LookAt(lookAt);
		gameObject.transform.localPosition = pos;
		jump = Mathf.Lerp(jump, jumpDelta, .1f);
		jumpDelta *= .9f;
	}

	public void Kiss () {
		jumpDelta += 10f;
		float strengh = 10f;
		Vector2 impulse = new Vector2(UnityEngine.Random.Range(-strengh,strengh), UnityEngine.Random.Range(-strengh,strengh));
		// impulse = impulse.normalized * strengh;
		velocity += impulse;
		kissed = true;
	}

	public void SetFloat (string name, float value) {
		foreach (Material material in materials) {
			material.SetFloat(name, value);
		}
	}

	public void SetVector (string name, Vector2 value) {
		foreach (Material material in materials) {
			material.SetVector(name, value);
		}
	}

	
}
