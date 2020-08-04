using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatTrigger : MonoBehaviour
{
    public bool needsToInteract;
    public ChatSettings chat;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            if (other.transform.GetComponent<PlayerBagCollision>().enabled)
            {
                if (!needsToInteract)
                {

                    ChatSystem.Instance.DisplayMessage(chat);

                }
                else
                {
                    if (chat.ran)
                    {

                        if (chat.runOnce)
                        {


                        }
                        else
                        {
                            Player.Instance.showInteract = true;

                        }


                    }
                    else
                    {
                        Player.Instance.showInteract = true;

                    }
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        
                    }

                }
            }
           

        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {

            if (!needsToInteract)
            {
                //ChatSystem.Instance.DisplayMessage(chat);
            }

            else
            {
                if (chat.ran)
                {

                    if (chat.runOnce)
                    {


                    }
                    else
                    {
                        Player.Instance.showInteract = true;

                    }


                }
                else
                {
                    Player.Instance.showInteract = true;

                }
                if (Input.GetKeyDown(KeyCode.F))
                {
                    bool everyFalse = true;
                    for (int i = 0; i < ChatSystem.Instance.speakerImages.Length; i++)
                    {
                        if (ChatSystem.Instance.speakerImages[i].enabled)
                        {
                            everyFalse = false;
                        }
                    }
                    if (everyFalse)
                    {
                        ChatSystem.Instance.DisplayMessage(chat);
                    }
                }

            }


        }

    }
    private void OnTriggerExit(Collider other)
    {

        if (other.transform.CompareTag("Player"))
        {

            if (!needsToInteract)
            {
               
            }
            else
            {
                Player.Instance.showInteract = false;

            }


        }




    }

}
