using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderChatPlayer : MonoBehaviour
{

    public ChatSettings chat;
    //public float cooldown;


    private void OnTriggerEnter(Collider other)
    {

        print("colidiu");

        if (other.transform.CompareTag("Player"))
        {
            print("colidiu player");
            ChatSystem.Instance.DisplayMessage(chat);

        }


    }

   
}
