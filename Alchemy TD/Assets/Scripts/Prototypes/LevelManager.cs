using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {

	// public variables for inspector

    //float startTime;

    public GameObject wall;

    public CursorController cursorController;

	// Use this for initialization
	void Start ()
	{
        //startTime = Time.time;
	}

    public void PlaceWall()
    {
        StartCoroutine("GetCursorPosition");
    }

    IEnumerator GetCursorPosition()
    {
        //cursorController.cursor.sprite = cursorController.wallImage;

        while (!Input.GetMouseButtonDown(0))
        {
            yield return null;
        }
        Vector3 location = Input.mousePosition;
        location = Camera.main.ScreenToWorldPoint(location);
        Instantiate(wall, new Vector3(Mathf.Round(2*location.x)/2, Mathf.Round(2*location.y)/2, 0), cursorController.cursor.transform.rotation);
        cursorController.cursor.sprite = null;
        yield return null;
    }

	void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.tag == "Monster")
		{
			Destroy(collision.transform.parent.gameObject);
		}
	}
}
