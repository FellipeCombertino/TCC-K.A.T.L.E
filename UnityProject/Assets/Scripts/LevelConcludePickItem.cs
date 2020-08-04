using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelConcludePickItem : MonoBehaviour
{
    public ChatSettings chat;
    int startChildCount;
    public Animator fade;
    public bool startedRoutine;
    
    void Start()
    {
        startChildCount = transform.childCount;
    }

    // Update is called once per frame
    void Update()
    {


        if (startChildCount != transform.childCount)
        {

            if(!chat.chatStarted)
            ChatSystem.Instance.DisplayMessage(chat);


        }
        if (chat.ran)
        {
            if (!startedRoutine)
            {
                StartCoroutine(goMarket());
                startedRoutine = true;
            }

        }


    }
    IEnumerator goMarket()
    {
        yield return new WaitForSeconds(2);
        fade.Play("FadeOut");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Creditos");





    }
}
