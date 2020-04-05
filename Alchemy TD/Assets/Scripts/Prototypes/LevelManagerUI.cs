using UnityEngine;
using UnityEngine.UI;

public class LevelManagerUI : MonoBehaviour {

	// public variables for inspector
	public Slider healthBar;

	// Use this for initialization
	void Start ()
	{

	}

	// Update is called once per frame
	void Update ()
	{

	}

	void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.tag == "Monster")
		{
			Destroy(collision.transform.parent.gameObject);
			healthBar.value -= 1;
		}
	}
}
