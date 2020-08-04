using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHUD : MonoBehaviour
{
    Quaternion originalRotation;

    public Image hudButton;
    Text textButton;
    Animator hudButtonAnimator;

    // public Sprite[] spritesButton;
    public Color[] spritesColor;
    public KeyCode[] buttonKeys;
    public KeyCode correctKey;

    public int loseGameWith;
    public int winGameWith;
    public int lostXTimes;
    public int wonXTimes;
    float animSpeed;

    public static EnemyHUD Instance;

    AudioSource bubbleAudio;
    public AudioClip[] bubblePop, bubbleWin;

    void Start()
    {
        Instance = this;
        originalRotation = transform.rotation;
        hudButtonAnimator = hudButton.GetComponentInChildren<Animator>();
        textButton = hudButton.GetComponentInChildren<Text>();

        correctKey = buttonKeys[Random.Range(0, buttonKeys.Length)];
        textButton.text = "";
        if (correctKey.ToString().Length > 1)
        {
            textButton.text = correctKey.ToString().Substring(correctKey.ToString().Length - 1);
        }
        else
        {
            textButton.text = correctKey.ToString();
        }
        animSpeed = 0.5f;

        bubbleAudio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (lostXTimes < loseGameWith && wonXTimes < winGameWith) // se ainda não tiver acabado o game
        {
            if (!hudButton.gameObject.activeInHierarchy)
            {
                hudButton.gameObject.SetActive(true);
            }

            transform.rotation = CameraController.Instance.transform.rotation * originalRotation; // billboard

            if (Input.GetKeyDown(correctKey)) // se acertou a tecla
            {
                if (wonXTimes < winGameWith)
                {
                    ChangeKey(true);
                    bubbleAudio.PlayOneShot(bubbleWin[Random.Range(0, bubbleWin.Length)]);
                }
            }
            else if (Input.anyKeyDown) // se errou
            {
                if (IgnoreKeysToChallenge() && hudButtonAnimator.GetCurrentAnimatorStateInfo(0).IsName("ButtonAnim"))
                {
                    correctKey = KeyCode.F7;
                    hudButtonAnimator.SetBool("wrongKey", true);
                    hudButtonAnimator.speed = 1; // o speed da anim volta ao normal para executar a derrota
                    bubbleAudio.PlayOneShot(bubblePop[Random.Range(0, bubblePop.Length)]);
                }
            }

            // se tiver rodando a animação normal
            if (hudButtonAnimator.GetCurrentAnimatorStateInfo(0).IsName("ButtonAnim"))
            {
                if (hudButtonAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1 && !hudButtonAnimator.GetBool("wrongKey"))
                {
                    hudButtonAnimator.speed = animSpeed; // o speed da animação é setado pelo inimigo, conforme a dificuldade do inimigo
                }

                // quando terminar a animação de aparecer o botão, se o player n tiver clicado ainda, a bolha estoura automaticamente
                if (hudButtonAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f)
                {
                    correctKey = KeyCode.F7;
                    hudButtonAnimator.SetBool("wrongKey", true);
                    hudButtonAnimator.speed = 1; // o speed da anim volta ao normal para executar a derrota
                    if (!bubbleAudio.isPlaying)
                        bubbleAudio.PlayOneShot(bubblePop[Random.Range(0, bubblePop.Length)]);
                }
            }

            if (hudButtonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Bubble Pop"))
            {
                // quando o botão fizer a animação de derrota, ele muda para o próximo
                if (hudButtonAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f)
                {
                    ChangeKey(false);
                }
            }
        }
        else // senão o game acaba
        {
            if (hudButton.gameObject.activeInHierarchy)
            {
                if (hudButtonAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f || hudButtonAnimator.GetCurrentAnimatorStateInfo(0).IsName("ButtonAnim"))
                {
                    hudButton.gameObject.SetActive(false);
                }
            }
        }
    }

    public void ResetChallenge(int loseGameInt, int winGameInt, float speedAnim)
    {
        loseGameWith = loseGameInt;
        winGameWith = winGameInt;
        lostXTimes = 0;
        wonXTimes = 0;
        animSpeed = speedAnim;
    }

    void ChangeKey(bool wonGame)
    {
        //hudButton.sprite = spritesButton[Random.Range(0, spritesButton.Length)];
        hudButton.color = spritesColor[Random.Range(0, spritesColor.Length)];
        correctKey = buttonKeys[Random.Range(0, buttonKeys.Length)];

        textButton.text = "";

        if (correctKey.ToString().Length > 1)
        {
            textButton.text = correctKey.ToString().Substring(correctKey.ToString().Length-1);
        }
        else
        {
            textButton.text = correctKey.ToString();
        }

        Vector3 positionToGo = new Vector3(Random.Range(-1, 1.1f), Random.Range(0, 1.1f), 0);
        hudButton.rectTransform.anchoredPosition = positionToGo;

        hudButtonAnimator.Play("ButtonAnim", 0, 0);
        hudButtonAnimator.SetBool("wrongKey", false);

        if (wonGame)
        {
            wonXTimes++;
        }
        else
        {
            lostXTimes++;
        }
    }

    bool IgnoreKeysToChallenge()
    {
        // todas as teclas no if abaixo serão ignoradas, pq o player pode clicar sem querer nela e perder o desafio por ter clicado na tecla errada
        if (!Input.GetKeyDown(KeyCode.W) && !Input.GetKeyDown(KeyCode.A) && !Input.GetKeyDown(KeyCode.S) && !Input.GetKeyDown(KeyCode.D)
             && !Input.GetKeyDown(KeyCode.LeftArrow) && !Input.GetKeyDown(KeyCode.RightArrow) && !Input.GetKeyDown(KeyCode.DownArrow) && !Input.GetKeyDown(KeyCode.UpArrow)
             && !Input.GetKeyDown(KeyCode.Escape) && !Input.GetKeyDown(KeyCode.Tab) && !Input.GetKeyDown(KeyCode.Space) && !Input.GetKeyDown(KeyCode.F)
             && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
