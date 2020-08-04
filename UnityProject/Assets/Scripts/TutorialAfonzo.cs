using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialAfonzo : MonoBehaviour
{
    public GameObject seta;
    public ChatSettings chatWithEgg;
    public ChatSettings chatWithoutEgg;
    bool canTalk;
    public bool withEgg;
    public void Update()
    {

      //  GetComponentInChildren<Animator>().SetBool("talking", chatWithEgg.chatStarted|| chatWithoutEgg.chatStarted);
        if (FindObjectOfType<KimTutorial>().hasEgg)
        {
            if (withEgg)
            {

                seta.SetActive(false);
            }
            else
            {

                seta.SetActive(true);
            }
        }

        if (canTalk)
        {
           
            if (Input.GetKeyDown(KeyCode.F))
            {
                Player.Instance.showInteract = false;
                if (FindObjectOfType<KimTutorial>().hasEgg)
                {
                    
                    FindObjectOfType<ChatSystem>().DisplayMessage(chatWithEgg);
                    withEgg = true;


                }
                else
                {

                    FindObjectOfType<ChatSystem>().DisplayMessage(chatWithoutEgg);

                }
            }
        }







    }
    private void OnTriggerEnter(Collider other)
    {

        canTalk = true;
        Player.Instance.showInteract = true;


    }
    private void OnTriggerExit(Collider other)
    {
        canTalk = false;
        Player.Instance.showInteract = false;

    }

}
