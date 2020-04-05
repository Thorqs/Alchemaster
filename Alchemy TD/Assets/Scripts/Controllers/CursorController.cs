using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour {

    public SpriteRenderer cursor;

    private SnapMode snapMode = SnapMode.None;

    bool isVert;

    // Use this for initialization
	void Start () {
        cursor = GetComponent<SpriteRenderer>();
        isVert = true;
	}


    public void SetCursor(Sprite cursorSprite, float scale, SnapMode snap)
    {
        cursor.sprite = cursorSprite;
        cursor.color = new Color(1, 1, 1, .3f);
        cursor.transform.localScale = Vector3.one * scale;
        snapMode = snap;
        transform.rotation = Quaternion.identity;
        isVert = true;
    }

    // Update is called once per frame
    void Update () {
		Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20));
        float xPos = worldPoint.x;
        float yPos = worldPoint.y;

        if(snapMode == SnapMode.ToGridSpace)
        {
            xPos = Mathf.Round(xPos);
            yPos = Mathf.Round(yPos);
        }

        else if (snapMode == SnapMode.ToGridLines)
        {
            xPos = Mathf.Round(2 * worldPoint.x) / 2;
            yPos = Mathf.Round(worldPoint.y);
            // We want y round to nearest 0.5 if x is whole, nearest whole otherwise
            if (Mathf.Abs(((2 * xPos) % 2)) < Mathf.Epsilon) // check if xPos is a whole number
            {
                yPos += 0.5f;
            }
            if (Mathf.Abs(Mathf.Abs(((2 * xPos) % 2)) - 1f)  < Mathf.Epsilon)
            {
                if (isVert)
                {
                    transform.Rotate(0, 0, 90f);


                    isVert = false;
                }
            }
            else
            {
                if (!isVert)
                {
                    transform.Rotate(0, 0, -90f);
                    isVert = true;
                }
            }
        }

        transform.position = new Vector3(xPos, yPos, transform.position.z);
    }
}


public enum SnapMode { None, ToGridSpace, ToGridLines }