using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buddies : MonoBehaviour
{

    public Camera cameraComponent;

    public Shader headShader;
    public Shader bodyShader;

    public Texture kissTexture;

    [Header("Boid Area")]
    public float followRange = 3f;

    [Header("Boid Velocity")]
    public float velocitySpeed = .1f;
    public float velocityFriction = .9f;
    public float velocityDamping = .1f;
    public float velocityMax = 1f;


    [NonSerialized]public List<Buddy> buddyList;
    private List<Texture2D> headTextures;
    private List<Texture2D> bodyTextures;
    private string[] fileHeadPaths;
    private string[] fileBodyPaths;
    private string dataHeadPath;
    private string dataBodyPath;
    private int total;
    private RaycastHit raycast;
    private Ray ray;
    private float mouseClick;

    void Start()
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


        buddyList = new List<Buddy>();
        mouseClick = 0f;
        Fetch();

        InvokeRepeating("Direction", 0, 3);
        InvokeRepeating("RetourZero", 0, 21);
    }

    void Update()
    {

        float mouseDelta = 0f;
        if (Input.GetMouseButton(0) && GetComponent<GameManager>().playing) mouseDelta = Mathf.Clamp01(mouseDelta + 5f);

        if (Input.GetMouseButtonDown(0) && GetComponent<GameManager>().playing)
        {
            ray = cameraComponent.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycast, 100f))
            {
                GameObject other = raycast.collider.gameObject;
                if (other.transform.parent.parent == transform)
                {
                    if (other.transform.parent.GetComponent<BuddyBehaviour>().buddy.kissed == false)
                    {
                        GetComponent<GameManager>().UpdateCount();
                    }
                    other.transform.parent.GetComponent<BuddyBehaviour>().buddy.Kiss();

                }
            }
        }

        mouseClick = Mathf.Lerp(mouseClick, mouseDelta, .1f);
        Shader.SetGlobalVector("_MousePosition", Input.mousePosition);
        Shader.SetGlobalFloat("_MouseClick", mouseClick);
        Shader.SetGlobalTexture("_KissTexture", kissTexture);

        foreach (Buddy buddy in buddyList)
        {

            Vector2 velocity = new Vector2(0, 0);

            Vector2 target = (buddy.targetDir - buddy.position).normalized;

            Vector2 avoid = new Vector2(0, 0);
            Vector2 follow = new Vector2(0, 0);
            foreach (Buddy other in buddyList)
            {
                float dist = Vector2.Distance(buddy.position, other.position);
                float radius = buddy.size + other.size + (buddy.jumpDelta + other.jumpDelta) * 2f;
                Vector2 dir = buddy.position - other.position;
                if (dist < radius && dist > 0.0001f)
                {
                    avoid += dir.normalized * Mathf.Max(buddy.size, dir.magnitude);
                }
                if (dist < followRange && dist > 0.0001f)
                {
                    follow += other.velocity;
                }
            }

            velocity += avoid * buddy.avoidScale;
            velocity += follow * buddy.followScale;
            velocity += target * buddy.targetScale;
            velocity = velocity.normalized * Mathf.Min(velocityMax, velocity.magnitude);

            buddy.velocity = Vector2.Lerp(buddy.velocity * velocityFriction, velocity, velocityDamping);
            buddy.position += buddy.velocity * velocitySpeed;


            buddy.Update();

        }
    }

    public void CreateBuddy(Texture2D texture)
    {
        Material materialHead = new Material(headShader);
        Material materialBody = new Material(bodyShader);
        materialHead.mainTexture = texture;
        materialBody.mainTexture = bodyTextures[(int)UnityEngine.Random.Range(0, bodyTextures.Count)];
        Buddy buddy = new Buddy(materialHead, materialBody, transform);
        buddy.lookAt = cameraComponent.transform;
        buddyList.Add(buddy);
    }

    public void CreateBuddies()
    {
        foreach (Texture2D texture in headTextures)
        {
            CreateBuddy(texture);
        }
    }

    public void Fetch()
    {
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

        for (int i = 0; i < fileInfos.Length; ++i)
        {
            fileHeadPaths[i] = "file://" + dataHeadPath + fileInfos[i].Name;
            StartCoroutine(Load(fileHeadPaths[i], headTextures));
        }
        for (int i = 0; i < fileBodyInfos.Length; ++i)
        {
            fileBodyPaths[i] = "file://" + dataBodyPath + fileBodyInfos[i].Name;
            StartCoroutine(Load(fileBodyPaths[i], bodyTextures));
        }
    }

    IEnumerator Load(string url, List<Texture2D> list)
    {
        WWW www = new WWW(url);
        yield return www;
        list.Add(www.texture);
        if (headTextures.Count + bodyTextures.Count == total)
        {
            CreateBuddies();
        }
    }

    public void Direction()
    {
        //Debug.Log("Bonjour direction");

        foreach (Buddy buddy in buddyList)
        {
            buddy.targetDir = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-2f, 1f));
            buddy.followScale = UnityEngine.Random.Range(.6f, .8f) * (buddy.targetDir.x * buddy.targetDir.x + buddy.targetDir.y * buddy.targetDir.y);
        }

    }

    public void RetourZero()
    {
        Debug.Log("Bonjour direction");

        foreach (Buddy buddy in buddyList)
        {
            buddy.targetDir = Vector2.zero;
            buddy.followScale = UnityEngine.Random.Range(.6f, .8f);
        }

    }
}