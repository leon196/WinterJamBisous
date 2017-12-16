using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public bool playing = false;
    public GameObject startButton;
    public GameObject startImage;
    public GameObject restartButton;
    public Text messageFin;
    public Text countText;

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

        messageFin.text = "Vous êtes génial, vous avez gagné !";

    }

    public void UpdateCount()
    {
        ++countKiss;
    }
}
