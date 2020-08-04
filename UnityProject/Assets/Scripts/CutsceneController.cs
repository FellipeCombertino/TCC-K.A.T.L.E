using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering;


public class CutsceneController : MonoBehaviour
{
    public PostProcessProfile profileDay;
    public PostProcessProfile profileNight;

    public Material matDay;
    public Material matNight;
   
    public ChatSettings Cut2Chat;
    public ChatSettings Cut3Chat;
    public AudioClip musicaTutorial;
    public AudioSource audioFont;
    public Transform initialPos;
    public GameObject MainCamera;
    public GameObject CameraCut;
    public Animator cut2;
    public Animator cut3;

    public bool canLoop;
    void Start()
    {
        
    }
    public void EndFirstCut()
    {

        StartCoroutine(CutsceneContinue());

    }
    IEnumerator CutsceneContinue()
    {
       
        ActiveCut2();
        InitChat2();
        CameraCut.GetComponent<Animator>().SetInteger("AnimStage", 1);
        yield return new WaitForSeconds(2);
        while (ChatSystem.Instance.chatIsRunning)
        {
            yield return null;

        }
      
        cut2.SetBool("loop", false);
        yield return new WaitForSeconds(1f);
        while (cut2.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f)
        {

            yield return null;

        }
        GetComponent<FadeOutInForCut>().FadeIn();
        yield return new WaitForSeconds(1.5f);
        InitChat3();
        ActiveCut3();
        GetComponent<FadeOutInForCut>().FadeOut();
        yield return new WaitForSeconds(2);
        while (ChatSystem.Instance.chatIsRunning)
        {
            yield return null;

        }
        cut3.SetBool("loop", false);
        yield return new WaitForSeconds(1);
        while (cut3.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.8f)
        {

            yield return null;

        }
        MoveKimToInitialPos();

    }


    public void ToDay()
    {
        MainCamera.GetComponent<PostProcessVolume>().profile = profileDay;
        CameraCut.GetComponent<PostProcessVolume>().profile = profileDay;
        RenderSettings.skybox = matDay;
    }
    public void ToNight()
    {
        MainCamera.GetComponent<PostProcessVolume>().profile = profileNight;
        CameraCut.GetComponent<PostProcessVolume>().profile = profileNight;
        RenderSettings.skybox = matNight;
    }
    public void InitChat2()
    {

        ChatSystem.Instance.DisplayMessage(Cut2Chat);

    }
    public void InitChat3()
    {

        ChatSystem.Instance.DisplayMessage(Cut3Chat);

    }
    public void ActiveCut2()
    {

        cut2.gameObject.SetActive(true);


    }
    public void ActiveCut3()
    {

        cut3.gameObject.SetActive(true);
        cut2.gameObject.SetActive(false);

    }
    public void DesactiveCut2()
    {

        cut2.gameObject.SetActive(false);


    }
    public void DesactiveCut3()
    {

        cut3.gameObject.SetActive(false);


    }
    public void PauseAnim2()
    {

        cut2.speed = 0;

    } public void PauseAnim3()
    {
        cut3.speed = 0;


    }
    public void ResumeAnim2()
    {

        cut2.speed = 1;

    } public void ResumeAnim3()
    {
        cut3.speed = 1;


    }
    public void setIntAnimator()
    {



    }

    public void MoveKimToInitialPos()
    {


        
        DesactiveCut3();
        Player.Instance.transform.position = initialPos.position;
        Player.Instance.kimTutorial = false;
        Player.Instance.rb.velocity = Vector3.zero;
        MainCamera.SetActive(true);
        CameraCut.SetActive(false);
    }
    public void ChangeMusic()
    {
        audioFont.clip = musicaTutorial;



    }
}
