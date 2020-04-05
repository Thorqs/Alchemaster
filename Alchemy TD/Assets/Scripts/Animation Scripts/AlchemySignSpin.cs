using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlchemySignSpin : MonoBehaviour {

    public float rpm;
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(0, 0, (rpm * 360) * Time.deltaTime);
	}
}
