using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Interface : MonoBehaviour
{

    public void Start()
    {

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }
    public void Play()
    {

        SceneManager.LoadScene(1);


    } public void Exit()
    {

        Application.Quit();


    }



}
