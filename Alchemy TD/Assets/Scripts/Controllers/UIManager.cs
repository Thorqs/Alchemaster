using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    PauseMenuManager pauseMenu;

	// Use this for initialization
	void Start ()
    {
        pauseMenu = pauseMenuPanel.GetComponent<PauseMenuManager>();
        pauseMenu.Hide();
	}

	// Update is called once per frame
	void Update ()
	{
        /* if(gameOver)
        {
            pauseMenu.ShowGameover();
        }
        */
        /* else */ if(Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.P))
        {
            pauseMenu.ShowPause();
        }
	}
}
