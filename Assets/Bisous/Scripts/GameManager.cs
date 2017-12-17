using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    [NonSerialized]public bool playing = false;
    public GameObject startButton;
    public GameObject startImage;
    public GameObject restartButton;
    public Text messageFin;
    public Text countText;
    public GameObject particule;
    public AudioSource bruitBisous;

    private int countKiss = 0;

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        if (playing)
            countText.text = countKiss + "/" + GetComponent<Buddies>().buddyList.Count;

        if (countKiss == GetComponent<Buddies>().buddyList.Count && countKiss != 0)
        {
            EndGame();
        }

        if (Input.GetMouseButtonDown(0) && playing)
        {
            Instantiate(particule);

            int soundIndex = UnityEngine.Random.Range(0, GetComponent<FetchSound>().bruitages.Count);
            bruitBisous.clip = GetComponent<FetchSound>().bruitages[soundIndex];
            bruitBisous.Play();
        }
    }

    public void Playing()
    {
        playing = true;
        startButton.SetActive(false);
        startImage.SetActive(false);
        restartButton.SetActive(false);
        messageFin.text = "";

        foreach (Buddy buddy in GetComponent<Buddies>().buddyList)
        {
            buddy.kissed = false;
        }
    }

    public void EndGame()
    {
        playing = false;
        restartButton.SetActive(true);
        countKiss = 0;

        messageFin.text = "TU ES GÉNIAL-E, TU AS GAGNÉ !";

    }

    public void UpdateCount()
    {
        ++countKiss;
    }
}
