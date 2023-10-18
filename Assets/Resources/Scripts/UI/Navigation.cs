using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Navigation : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject singlePlayer;
    public GameObject multiplayer;
    public GameObject GameOver;
    public GameObject playUI;


    void Update()
    {
        previousScene();
    }
    public void closeApp()
    {
        Application.Quit();
    }
    public void previousScene()
    {
     
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (singlePlayer.activeSelf)
            {
                singlePlayer.SetActive(false);
                mainMenu.SetActive(true);
            }
            else
            if(multiplayer.activeSelf)
            {
                multiplayer.SetActive(false);
                mainMenu.SetActive(true);
            }
            else if (mainMenu.activeSelf)
            {
                Application.Quit();
            }
            else if(GameOver.activeSelf)
            {
                singlePlayer.SetActive(true);
                GameOver.SetActive(false);
                playUI.SetActive(false);
            }
        }
    }

}