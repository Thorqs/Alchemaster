using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

    [HideInInspector]
    public GameObject pathingGrid;

    private GameObject alchemyManager;

    private AudioSource audioSource;
    public AudioClip[] sounds;


    // Use this for initialization
    void Start () {
        pathingGrid = GameObject.FindGameObjectWithTag("PathingGrid");
        pathingGrid.GetComponent<DijkstraMap>().CalcMap(gameObject);
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0.05f;
        if (sounds.Length > 0)
        {
            int i = Random.Range(0, sounds.Length);
            audioSource.clip = sounds[i];
            audioSource.Play();
        }
    }
}
