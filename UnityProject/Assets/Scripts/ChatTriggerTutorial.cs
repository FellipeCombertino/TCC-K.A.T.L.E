using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatTriggerTutorial : MonoBehaviour
{
    public ChatSettings chat;
    public bool canGet;
    private void Update()
    {

        if (canGet)
        {
            Player.Instance.showInteract = true;
            if (Input.GetKeyDown(KeyCode.F))
            {
                ChatSystem.Instance.DisplayMessage(chat);
                FindObjectOfType<KimTutorial>().hasEgg = true;
                Player.Instance.showInteract = false;
                Destroy(gameObject);

            }

        }

    }
    private void OnTriggerEnter(Collider other)
    {


        
        canGet = true;



    }
    private void OnTriggerExit(Collider other)
    {

        Player.Instance.showInteract = false;
        canGet = false;


    }
}
