using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLevel1 : MonoBehaviour
{
    Buttom buttonLevel1;
    public int spokenEnemies;
    public ChatSettings chat, inicialChat;
    bool hasPlayed, hasPlayedInicial;
    public ChatTrigger afonso, timo;

    void Start()
    {
        buttonLevel1 = GetComponent<Buttom>();
    }

    void Update()
    {
        if (!hasPlayedInicial)
        {
            hasPlayedInicial = true;
            ChatSystem.Instance.DisplayMessage(inicialChat);
        }

        if (spokenEnemies < 2)
        {
            buttonLevel1.weightToOpen = 14;
            if (buttonLevel1.percentPressed == 0.5f && !hasPlayed)
            {
                ChatSystem.Instance.DisplayMessage(chat);
                hasPlayed = true;
                StartCoroutine(ResetChat());
            }
        }
        else
        {
            buttonLevel1.weightToOpen = 2;
            if (buttonLevel1.percentPressed >= 1)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Level1");
            }
        }

        if (afonso.chat.ran && timo.chat.ran)
        {
            spokenEnemies = 2;
        }
    }

    IEnumerator ResetChat()
    {
        yield return new WaitForSeconds(10);
        hasPlayed = false;
    }
}
