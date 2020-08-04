using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class Options
{
    public string optionName;
    public string message;
    public AudioClip voice;



}
[System.Serializable]
public class Message
{
    public enum Speaker { System, Barto, Kim,Afonzo,Afonso,Timo,Koda, Pierre,Loki, Klotild, Charles }
    public Speaker speaker;
    public Animator speakerAnim;
    public AudioClip speakSound;
    [TextArea(3, 5)]
    public string message;


}
[System.Serializable]
public class ChatSettings
{
    [Header("Interactive Chat")]
 
    public Message.Speaker speaker;
    public Animator animSpeaker;
    public AudioClip mainQuestionVoice;
    public bool Interactive;
    public Options[] options;
    public string firstQuestion;
    [Header("Trader Options")]
    public Message[] messageifTraded;
    public bool TradeItems;
    public Items.Item ItemPlayerTem;
    public Items.Item ItemNpcTem;
    public bool traded;
    
    [Space(20)]
    public bool runOnce;
    
    public bool chatStarted;
    public bool forceStop;
    public bool StopWhenGetOut;
    public bool canOverlap;
    public Message[] message;
    public bool ran;
    
}

public class ChatSystem : MonoBehaviour
{
    public Transform OptionsTransform;
    public static ChatSystem Instance;
    public bool optionClicked;
    public int optionIntClicked;
    public bool started;
    public bool ChatFinished;
    public Animator chatAnimator;
    public AudioSource source;
    public Text textBox;
    [Range (0.01f, 1)]
    public float textSpeed;
    public float delayBetweenTexts;
    public bool chatIsRunning;

    public Image[] speakerImages;

    public ChatSettings chatRunning;
    public Animator animNpc;
    void Start()
    {
        source = GetComponent<AudioSource>();
        Instance = this;
        chatAnimator = GetComponent<Animator>();

    }
    private void Update()
    {

     
                     

    }

    public void DisplayMessage(ChatSettings chatSettings)
    {
       
        if (chatSettings.canOverlap)
        {

            ClearChat();

        }
        chatIsRunning = true;
        chatRunning = chatSettings;
        chatSettings.chatStarted = true;
       
        Message[] message = new Message[chatSettings.message.Length];
        //string[] nextMessage = new string[chatSettings.message.Length];
        //.whosSpeaking = new ChatSettings.message[chatSettings.messages.Length];
        if (chatSettings.runOnce)
        {
            if (!chatSettings.ran)
            {
                for (int i = 0; i < chatSettings.message.Length; i++)
                {
                    message = chatSettings.message;

                }

                if (textBox.text == "")
                {
                    if (!chatSettings.runOnce)
                    {
                        StartCoroutine(SlowlyMessageDisplay(message));
                    }
                    else
                    {
                        if (!chatSettings.ran)
                        {
                            
                            StartCoroutine(SlowlyMessageDisplay(message));
                        }

                    }
                }
            }
            else
            {

                if (chatRunning.Interactive)
                {
                    StartCoroutine(SlowlyMessageDisplayInteractive());
                }



            }
        }
        else
        {
            if (chatRunning.TradeItems)
            {
                if (!chatRunning.traded)
                {
                    if (Inventory.Instance.CheckItem(chatRunning.ItemPlayerTem) && BagController.Instance.asBag)
                    {

                        StartCoroutine(SlowlyMessageDisplayInteractive());


                    }
                    else
                    {

                        StartCoroutine(SlowlyMessageDisplay(chatRunning.message));


                    }
                }
                else
                {
                    if (chatRunning.messageifTraded.Length > 0)
                    {
                        StartCoroutine(SlowlyMessageDisplay(chatRunning.messageifTraded));

                    }
                    else
                    {
                        if (chatRunning.forceStop)
                        {
                            Player.Instance.forceKimStop = false;
                            Player.Instance.ForceAnimation(false);
                        }

                       
                    }

                }






            }
            else
            {
                for (int i = 0; i < chatSettings.message.Length; i++)
                {
                    message = chatSettings.message;

                }

                if (textBox.text == "")
                {
                    if (!chatSettings.runOnce)
                    {
                        StartCoroutine(SlowlyMessageDisplay(message));
                    }
                    else
                    {
                        if (!chatSettings.ran)
                        {

                            StartCoroutine(SlowlyMessageDisplay(message));
                        }

                    }
                }
            }

        }
        
    }
    public void ClearChat()
    {

       
        textBox.text = ""; // a mensagem é zerada
        chatAnimator.SetBool("chatOn", false); // some a animação
        for (int i = 0; i < speakerImages.Length; i++) // some a fotinho de quem tá falando
        {
            speakerImages[i].enabled = false;
            if (chatRunning.forceStop) 
            {

                Player.Instance.ForceAnimation(false);
                Player.Instance.forceKimStop = false;
               
            }


            chatRunning.chatStarted = false;
        }
        StopAllCoroutines();
        chatIsRunning = false;

    }
    public void showPic(Message.Speaker speacker)
    {
        switch (speacker)
        {
            case Message.Speaker.System:

                break;
            case Message.Speaker.Barto:
                speakerImages[0].enabled = true;

                break;

            case Message.Speaker.Kim:
                speakerImages[1].enabled = true;
                break;
            case Message.Speaker.Afonzo:
                speakerImages[2].enabled = true;

                break;
            case Message.Speaker.Afonso:
                speakerImages[3].enabled = true;
                break;
            case Message.Speaker.Timo:
                speakerImages[4].enabled = true;

                break;
            case Message.Speaker.Koda:
                speakerImages[5].enabled = true;

                break;
            case Message.Speaker.Pierre:
                speakerImages[6].enabled = true;

                break;
            case Message.Speaker.Loki:
                speakerImages[7].enabled = true;

                break;
            case Message.Speaker.Klotild:
                speakerImages[8].enabled = true;

                break;
            case Message.Speaker.Charles:
                speakerImages[9].enabled = true;

                break;
            default:
                break;
        }

    }
    IEnumerator SlowlyMessageDisplayInteractive()
    {
        OptionsTransform.gameObject.SetActive(false);
        optionClicked = false;
        chatAnimator.SetBool("chatOn", true);
        if (chatRunning.forceStop)
        {
            if (!BagController.Instance.forceOpenInvent)
            {
                Player.Instance.forceKimStop = true;
                Player.Instance.ForceAnimation(true, Player.PlayerStates.Idle);
            }
        }

        showPic(chatRunning.speaker);

        if (chatRunning.mainQuestionVoice)
        {
            source.PlayOneShot(chatRunning.mainQuestionVoice);
        }

        for (int i = 0; i < chatRunning.firstQuestion.Length + 1; i++)
        {
            textBox.text = chatRunning.firstQuestion.Substring(0, i);
            yield return new WaitForSeconds(textSpeed);
            if (Input.GetKeyDown(KeyCode.F))
            {

                textBox.text = chatRunning.firstQuestion;
                i = chatRunning.firstQuestion.Length;

            }
        }

        
        yield return new WaitForSeconds(0.5f);
        for (float timer = delayBetweenTexts; timer >= 0; timer -= Time.deltaTime)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                yield return null;
                timer = 0;
            }
            yield return null;
        }


        if (chatRunning.animSpeaker)
        {
            if (chatRunning.speaker != Message.Speaker.Kim)
            {
                chatRunning.animSpeaker.SetBool("talking", false);
            }
            else
            {

                    if (chatRunning.forceStop)
                    {
                        Player.Instance.ForceAnimation(false);
                    }
       

            }

        }
      
        for (int i = 0; i < chatRunning.options.Length; i++)
        {

            OptionsTransform.GetChild(i).GetComponentInChildren<Text>().text = chatRunning.options[i].optionName;
            OptionsTransform.GetChild(i).gameObject.SetActive(true);

        }
        if (chatRunning.TradeItems)
        {
            OptionsTransform.GetChild(2).gameObject.SetActive(false);
        }
        OptionsTransform.gameObject.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        CameraController.Instance.lockCamera = true;

        source.PlayOneShot(chatRunning.options[optionIntClicked].voice);

        while (optionClicked == false)
        {

            yield return null;

        }
        OptionsTransform.gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        CameraController.Instance.lockCamera = false;
        for (int i = 0; i < chatRunning.options[optionIntClicked].message.Length + 1; i++)
        {
            if(optionIntClicked == 0)
            {
                if (!chatRunning.traded)
                {
                    chatRunning.traded = true;
                    Inventory.Instance.SwapItem(chatRunning.ItemPlayerTem, chatRunning.ItemNpcTem);
                }
            }
            textBox.text = chatRunning.options[optionIntClicked].message.Substring(0, i);
            yield return new WaitForSeconds(textSpeed);
            if (Input.GetKeyDown(KeyCode.F))
            {

                textBox.text = chatRunning.options[optionIntClicked].message;
                i = chatRunning.options[optionIntClicked].message.Length;

            }
        }

        yield return new WaitForSeconds(0.5f);
        for (float timer = delayBetweenTexts; timer >= 0; timer -= Time.deltaTime)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                yield return null;
                timer = 0;
            }
            yield return null;
        }

        source.Stop();
        textBox.text = "";
        if (!BagController.Instance.forceOpenInvent)
        {
            Player.Instance.ForceAnimation(false);
            Player.Instance.forceKimStop = false;
        }
       

        for (int i = 0; i < speakerImages.Length; i++)
        {

            speakerImages[i].enabled = false;

        }

        ClearChat();
        
    }
    public void OptionClick(int i)
    {

        optionClicked = true;
        optionIntClicked = i;



    }

    IEnumerator SlowlyMessageDisplay (Message[] message)
    {
        
        if (message.Length > 0)
        {
            chatAnimator.SetBool("chatOn", true);

            for (int j = 0; j < message.Length; j++)
            {
                if (!BagController.Instance.forceOpenInvent)
                {
                    if (chatRunning.forceStop)
                    {
                        Player.Instance.forceKimStop = true;
                        Player.Instance.ForceAnimation(true, Player.PlayerStates.Idle);
                    }
                }

                //se tiver animator de fala, ativar anim
                if (message[j].speakerAnim)
                {
                    if (message[j].speaker != Message.Speaker.Kim)
                    {
                        message[j].speakerAnim.SetBool("talking", true);
                    }
                    else
                    {
                       
                            if (chatRunning.forceStop)
                            {
                                Player.Instance.ForceAnimation(true, Player.PlayerStates.Talking);
                            }
                        
                
                    }

                }

                if (message[j].speakSound)
                {

                    source.PlayOneShot(message[j].speakSound);

                }
                // mostrar a fotinho de quem tá falando
                showPic(message[j].speaker);



                
                // escrever a mensagem letra por letra conforme o speed setado na variável pública
                for (int i = 0; i < message[j].message.Length + 1; i++)
                {
                   
                    textBox.text = message[j].message.Substring(0, i);
                    yield return new WaitForSeconds(textSpeed);
                  if (Input.GetKeyDown(KeyCode.F))
                  {

                    textBox.text = message[j].message;
                    i = message[j].message.Length;

                  }
                    
                }
                yield return new WaitForSeconds(0.5f);
                for (float timer = delayBetweenTexts; timer >= 0; timer -= Time.deltaTime)
                {
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        yield return null;
                        timer = 0;
                    }
                    yield return null;
                }
             

                if (j != message.Length - 1) // se não for a última mensagem, ele limpa a caixinha p/ continuar mostrando a sequencias de msgs
                {
                    
                    for (int i = 0; i < speakerImages.Length; i++) // some a fotinho de quem tá falando
                    {
                        speakerImages[i].enabled = false;
                    }
                    source.Stop();
                    textBox.text = "";
                }

                if (message[j].speakerAnim)
                {
                    if (message[j].speaker != Message.Speaker.Kim)
                    {
                        message[j].speakerAnim.SetBool("talking", false);
                    }
                    else
                    {

                        if (chatRunning.forceStop)
                        {
                            Player.Instance.ForceAnimation(false);
                        }


                    }

                  

                }

            }

            // quando a mensagem estiver completa, ela sumirá da tela após o delay entre os textos
            if (textBox.text == message[message.Length - 1].message)
            {
               
                chatRunning.ran = true;
        

                ClearChat();
            }
        }
    }
}
