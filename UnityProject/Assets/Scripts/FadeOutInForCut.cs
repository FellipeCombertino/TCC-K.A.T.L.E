using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutInForCut : MonoBehaviour
{

    public Animator anim;

    public void FadeOut()
    {
        anim.Play("Fadeout");


    }


    public void FadeIn()
    {

        anim.Play("Fadein");

    }



}
