using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geometry {


	static public GameObject GetQuad (string name, Material material, Transform root = null) {
		GameObject meshGameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
		meshGameObject.name = name;
		meshGameObject.GetComponent<Renderer>().material = material;
		meshGameObject.transform.parent = root;
		meshGameObject.transform.localPosition = Vector3.zero;
		meshGameObject.transform.localScale = Vector3.one;
		meshGameObject.transform.localRotation = Quaternion.identity;	
		meshGameObject.GetComponent<MeshCollider>().convex = true;
		return meshGameObject;
	}

	static public Mesh[] GenerateMeshes (Transform root, Material material, int dimension, float sliceX = 1f, float sliceY = 1f)
	{
		Vector2 slices = new Vector2(sliceX, sliceY);
		Vector2 faces = new Vector2(slices.x+1f, slices.y+1f);
		int vertexCount = (int)(faces.x * faces.y);
		int total = dimension * dimension;
		int totalVertices = total * vertexCount;
		int verticesMax = 65000;
		int meshCount = 1 + (int)Mathf.Floor(totalVertices / verticesMax);
		Mesh[] meshes = new Mesh[meshCount];
		int mapIndex = 0;
		for (int m = 0; m < meshCount; ++m)
		{
			GameObject meshGameObject = new GameObject(root.gameObject.name + "_mesh" + m);
			MeshRenderer render = meshGameObject.AddComponent<MeshRenderer>();
			MeshFilter filter = meshGameObject.AddComponent<MeshFilter>();

			meshGameObject.transform.parent = root.transform;
			meshGameObject.transform.localPosition = Vector3.zero;
			meshGameObject.transform.localRotation = Quaternion.identity;
			meshGameObject.transform.localScale = Vector3.one;
			meshGameObject.layer = root.gameObject.layer;
			render.material = material;

			int count = totalVertices;
			if (meshCount > 1) {
				if (m == meshCount - 1) count = count % verticesMax;
				else count = verticesMax;
			}

			List<Vector3> vertices = new List<Vector3>();
			List<Vector2> anchors = new List<Vector2>();
			List<Vector2> indexMap = new List<Vector2>();
			List<int> indices = new List<int>();
			int vIndex = 0;
			float min = -1f;
			float max = 1f;
			for (int index = 0; index < count/(faces.x*faces.y); ++index) {
				float u = (float)(mapIndex % dimension)/(float)dimension;
				float v = (float)(mapIndex / dimension)/(float)dimension;
				Vector3 position = new Vector3(UnityEngine.Random.Range(min, max), UnityEngine.Random.Range(min, max), UnityEngine.Random.Range(min, max));
				for (int y = 0; y < faces.y; ++y) {
					for (int x = 0; x < faces.x; ++x) {
						
						indexMap.Add(new Vector2(u,v));
						vertices.Add(position);
						anchors.Add(new Vector2((x/slices.x)*2f-1f, (y/slices.y)*2f-1f));
					}
				}
				for (int y = 0; y < slices.y; ++y) {
					for (int x = 0; x < slices.x; ++x) {
						indices.Add(vIndex);
						indices.Add(vIndex+1);
						indices.Add(vIndex+1+(int)slices.x);
						indices.Add(vIndex+1+(int)slices.x);
						indices.Add(vIndex+1);
						indices.Add(vIndex+2+(int)slices.x);
						vIndex += 1;
					}
					vIndex += 1;
				}
				vIndex += (int)faces.x;
				++mapIndex;
			}

			Mesh mesh = new Mesh();
			mesh.vertices = vertices.ToArray();
			mesh.uv = anchors.ToArray();
			mesh.uv2 = indexMap.ToArray();
			mesh.SetTriangles(indices.ToArray(), 0);
			mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000f);

			filter.mesh = mesh;
			meshes[m] = mesh;
		}
		return meshes;
	}
}
