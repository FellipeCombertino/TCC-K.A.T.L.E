using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Creditos : MonoBehaviour
{

    VideoPlayer playerVideo;

    void Start()
    {
        playerVideo = GetComponent<VideoPlayer>();
    }

    void Update()
    {
        if (playerVideo.isPaused)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MenuNew");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
