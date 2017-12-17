using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fetch : MonoBehaviour
{
	[HideInInspector] public bool loaded;

	public Texture2D bodyAtlas;
	public Texture2D headAtlas;
	private List<Texture2D> headTextures;
	private List<Texture2D> bodyTextures;
	private string[] fileHeadPaths;
	private string[] fileBodyPaths;
	private string dataHeadPath;
	private string dataBodyPath;
	private int total;
	private Rect[] bodyRects;
	private Rect[] headRects;

	void Awake()
	{
		dataHeadPath = Application.dataPath;
		List<string> paths = new List<string>(dataHeadPath.Split('/'));
		paths.RemoveAt(paths.Count - 1);
		dataHeadPath = String.Join("/", paths.ToArray());
		dataHeadPath = System.IO.Path.Combine(dataHeadPath, "Heads/");

		dataBodyPath = Application.dataPath;
		paths = new List<string>(dataBodyPath.Split('/'));
		paths.RemoveAt(paths.Count - 1);
		dataBodyPath = String.Join("/", paths.ToArray());
		dataBodyPath = System.IO.Path.Combine(dataBodyPath, "Bodies/");

		loaded = false;
		total = 0;
		headTextures = new List<Texture2D>();
		bodyTextures = new List<Texture2D>();

		var info = new DirectoryInfo(dataHeadPath);
		FileInfo[] fileInfos = info.GetFiles();
		fileHeadPaths = new string[fileInfos.Length];
		total += fileInfos.Length;

		info = new DirectoryInfo(dataBodyPath);
		FileInfo[] fileBodyInfos = info.GetFiles();
		fileBodyPaths = new string[fileBodyInfos.Length];
		total += fileBodyInfos.Length;

		for (int i = 0; i < fileInfos.Length; ++i) {
			fileHeadPaths[i] = "file://" + dataHeadPath + fileInfos[i].Name;
			StartCoroutine(Load(fileHeadPaths[i], headTextures));
		}
		for (int i = 0; i < fileBodyInfos.Length; ++i) {
			fileBodyPaths[i] = "file://" + dataBodyPath + fileBodyInfos[i].Name;
			StartCoroutine(Load(fileBodyPaths[i], bodyTextures));
		}
	}

	IEnumerator Load(string url, List<Texture2D> list)
	{
		WWW www = new WWW(url);
		yield return www;
		list.Add(www.texture);
		if (headTextures.Count + bodyTextures.Count == total) {
			int dimension = 2048;
			bodyAtlas = new Texture2D(dimension, dimension);
			headAtlas = new Texture2D(dimension, dimension);
			bodyRects = bodyAtlas.PackTextures(bodyTextures.ToArray(), 2, dimension);
			headRects = headAtlas.PackTextures(headTextures.ToArray(), 2, dimension);
			loaded = true;
		}
	}

	public Vector4 GetRandomBodyFrame () {
		Rect rect = bodyRects[(int)UnityEngine.Random.Range(0, bodyRects.Length)];
		return new Vector4(rect.x,rect.y,rect.width,rect.height);
	}

	public Vector4 GetRandomHeadFrame () {
		Rect rect = headRects[(int)UnityEngine.Random.Range(0, headRects.Length)];
		return new Vector4(rect.x,rect.y,rect.width,rect.height);
	}

}