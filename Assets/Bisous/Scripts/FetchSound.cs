using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FetchSound : MonoBehaviour {

    [NonSerialized] public List<AudioClip> bruitages;
    private string[] filePaths;
    private string dataPath;
    private int total;

    void Start () {
        dataPath = Application.dataPath;
        List<string> paths = new List<string>(dataPath.Split('/'));
        paths.RemoveAt(paths.Count - 1);
        dataPath = String.Join("/", paths.ToArray());
        dataPath = System.IO.Path.Combine(dataPath, "Sound/");

        var info = new DirectoryInfo(dataPath);
        FileInfo[] fileInfos = info.GetFiles();
        filePaths = new string[fileInfos.Length];
        total = fileInfos.Length;

        bruitages = new List<AudioClip>();

        for (int i = 0; i < fileInfos.Length; ++i)
        {
            filePaths[i] = "file://" + dataPath + fileInfos[i].Name;
            StartCoroutine(Load(filePaths[i], bruitages));
        }
    }


    IEnumerator Load(string url, List<AudioClip> list)
    {
        WWW www = new WWW(url);
        yield return www;
        list.Add(WWWAudioExtensions.GetAudioClip(www));
    }
}
