using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonControl : MonoBehaviour {

    bool buttonEnabled;

    GameObject indicator;

    // Use this for initialization
    void Start () {
        indicator = transform.GetChild(0).gameObject;
        enableElement();
        indicate(false);
    }


    public void disableElement() {
        GetComponent<Image>().color = Color.black;
        buttonEnabled = false;
        indicate(false);
    }

    public void enableElement() {
        GetComponent<Image>().color = Color.white;
        buttonEnabled = true;
    }

    public bool checkEnabled() {
        return buttonEnabled;
    }

    public void indicate(bool ind) {
        indicator.SetActive(ind);
    }
}
