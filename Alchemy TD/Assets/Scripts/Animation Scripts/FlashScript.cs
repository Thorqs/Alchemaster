using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashScript : MonoBehaviour {

    public float flashFreq;
    
    //value between 0 and 1 indicating how much of a flash the sprite will be invisible. 0 = visible, 1 = invisible.
    public float flashLength;
    
    float invisTimer = 0;
    float flashTimer = 0;
    Vector3 normalScale;

	// Use this for initialization
	void Start () {
        normalScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		if(flashTimer >= (1/flashFreq) *(1-flashLength)) {
            if(invisTimer == 0) {
                hideSprite();
            }
            invisTimer += Time.deltaTime;
            
            if(invisTimer >= (1/flashFreq) *flashLength) {
                displaySprite();
                invisTimer = 0;
                flashTimer = 0;
            }
        } else {
            flashTimer += Time.deltaTime;
        }
	}
    
    void OnEnable() {
        invisTimer = 0;
        flashTimer = 0;
    }
    
    public void hideSprite() {
        transform.localScale = new Vector3(0, 0, 0);
    }
    
    public void displaySprite() {
        transform.localScale = normalScale;
    }
}
