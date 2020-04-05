using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardFollow : MonoBehaviour {

    Vector3 mousePos;
    Vector3 objectPos;
    float angle;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update ()
    {
        mousePos = Input.mousePosition;
        objectPos = Camera.main.WorldToScreenPoint(transform.position);
        angle = Mathf.Atan2((objectPos.x - mousePos.x), (objectPos.y - mousePos.y)) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180-angle));
    }
}
