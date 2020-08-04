using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttom : MonoBehaviour
{
    public bool lockWhenTrigger;
    private bool locked;
    public float percentPressed;
    public float weightToOpen;
    public float atualWeight;
    public float minY,maxY;
    private float YtoGo;
    public bool isTriggered;
    public GameObject seta;
    public Doors porta;
    public bool goToScene;
    public string scene;
    public void Start()
    {
        atualWeight = 0;
    }
    
    public void Update()
    {
       
        if (locked)
        {
            if (seta)
            {
                seta.SetActive(false);

            }
            if (goToScene)
            {

                SceneManager.LoadScene(scene);

            }

            if(porta != null)
            {
                
                if (!porta.isOpen)
                {

                    porta.Open();


                }


            }
            isTriggered = true;
            transform.GetChild(0).localPosition = new Vector3(0, minY, 0);
            return;

        }
        YtoGo = Mathf.Clamp(percentPressed * minY, minY, maxY);
        percentPressed = atualWeight / weightToOpen;

        transform.GetChild(0).localPosition = new Vector3(0, Mathf.Lerp(transform.GetChild(0).localPosition.y, YtoGo, Time.deltaTime*5), 0);

        if (Math.Round(transform.GetChild(0).localPosition.y,2) == Math.Round(minY, 2))
        {
            if (lockWhenTrigger)
            {
                locked = true;


            }
        }

        if (YtoGo == minY)
        {

            isTriggered = true;
           
        }
        else
        {

                isTriggered = false;

                       
        }


    }



}
