using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashcanScript : MonoBehaviour {

    public float cleanupPeriod;
    
    private Queue<GameObject> trash = new Queue<GameObject>();
    
    private List<GameObject> particleBin = new List<GameObject>();    
    float trashTimer = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		trashTimer += Time.deltaTime;
        if(trashTimer >= cleanupPeriod) {
            for(int i = 0; i < particleBin.Count; i++) {
                
                //null check must come first in-case object is already destroyed.
                if(particleBin[i] != null && !particleBin[i].GetComponent<ParticleSystem>().IsAlive()) {
                    if(particleBin[i] != null) {
                        Destroy(particleBin[i]);
                    }
                }
                
                if(particleBin[i] == null) {
                    particleBin.RemoveAt(i);
                    i--;
                }
            }
            
            while(trash.Count > 0) {
                GameObject item = trash.Dequeue();
                if(item != null) {
                    Destroy(item);
                }
            }
        }
	}
    
    public void trashObject(GameObject item) {
        trash.Enqueue(item);
    }
    
    public void trashParticle(GameObject item) {
        particleBin.Add(item);
    }
}
