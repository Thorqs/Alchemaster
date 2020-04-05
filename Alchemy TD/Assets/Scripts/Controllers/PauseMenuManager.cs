using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public Text resume = null;
    public Text restart = null;
    public Text quit = null;
    public Text enemiesKilled;

    public LevelMaster levelMaster;

    int optionIndex = 0;
    Text[] optionArray;

    bool pauseGame;

    // Use this for initialization
    void Start ()
    {
        optionArray = new Text[3];
        optionArray[0] = resume;
        optionArray[1] = restart;
        optionArray[2] = quit;
	}

    public void ShowPause()
    {
        pauseGame = true;
        enemiesKilled.text = "Enemies Killed:\n" + levelMaster.totalEnemiesKilled;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        pauseGame = false;
        Time.timeScale = 1f;
    }

    void ExecuteCommand(string command)
    {
        switch(command)
        {
            case "Resume":
                Hide();
                break;

            case "Restart":
                Restart();
                break;

            case "Quit":
                Quit();
                break;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        Text currentSelection = optionArray[optionIndex];

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            optionIndex++;
        }

        else if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            optionIndex--;
        }

        else if(Input.GetKeyDown(KeyCode.Return))
        {
            ExecuteCommand(currentSelection.text);
        }

        if(optionIndex < 0)
        {
            optionIndex = 0;
        }

        else if(optionIndex >= optionArray.Length)
        {
            optionIndex = optionArray.Length - 1;
        }

        // set the font colour of all text boxes to black then current to purple
        for (int i = 0; i < optionArray.Length; i++)
        {
            optionArray[i].color = Color.black;
        }
        currentSelection.color = Color.blue;

        if(pauseGame)
        {
            Time.timeScale = 0;
        }

        if((Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.P)) && pauseGame)
        {
            Hide();
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
