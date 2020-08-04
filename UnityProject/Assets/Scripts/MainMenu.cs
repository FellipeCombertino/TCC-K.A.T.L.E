using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject tutorialHud;

    void Start()
    {

        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;


    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GoToScene(string sceneName)
    {


        SceneManager.LoadScene(sceneName);



    }
    public void Sair()
    {


        Application.Quit();



    }
   public void Play()
    {
        if (PlayerPrefs.GetInt("played", 0) == 0)
        {

            GoToScene("Tutorial");



        }
        else
        {

            tutorialHud.SetActive(true);


        }


    }
}
