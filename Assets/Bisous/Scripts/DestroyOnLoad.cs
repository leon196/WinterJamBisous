using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnLoad : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        //GetComponent<ParticleSystem>().Emit(50);
        GetComponent<ParticleSystem>().Play();
        Destroy(gameObject, 2.0f);
    }

}
