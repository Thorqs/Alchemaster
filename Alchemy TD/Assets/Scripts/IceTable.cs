using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceTable : MonoBehaviour {

    public float openTime;
    public GameObject shield;
    public Animator animator;
    
    private float openTimer = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(openTimer >= openTime) {
            animator.SetTrigger("Unfurl");
        } else {
            openTimer += Time.deltaTime;
        }
	}
    
    public void openShield() {
            shield.SetActive(true);
    }
    
    public void shieldWalk() {
            animator.SetTrigger("setUp");
    }
}
