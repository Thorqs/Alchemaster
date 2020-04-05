using UnityEngine;
using UnityEngine.UI;

public class hp : MonoBehaviour
{
    public Text status;
    public Slider health;

	// Use this for initialization
	void Start ()
    {
        status.text = Mathf.Floor(health.value) + "/" + health.maxValue;
	}
	
	// Update is called once per frame
	void Update ()
    {
        status.text = Mathf.Floor(health.value) + "/" + health.maxValue;
    }
}
