using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseTrigger : MonoBehaviour
{
    public bool removeArrow;
    public Animator cut1Anim;
    public Animator camAnimator;
    public GameObject seta;
    public GameObject CameraS;
    
 
    // Update is called once per frame
    void Update()
    {
        if (!removeArrow)
        {
            seta.SetActive(FindObjectOfType<TutorialAfonzo>().withEgg);
        }
       
            
        
    }
    private void OnTriggerEnter(Collider other)
    {





        if (FindObjectOfType<TutorialAfonzo>().withEgg)
        {

            cut1Anim.Play("Cutscene");
            camAnimator.Play("CameraAnim1Cut");
            CameraS.SetActive(true);
            Destroy(seta);
            Player.Instance.transform.position = Vector3.zero;
            CameraController.Instance.gameObject.SetActive(false);
            Destroy(FindObjectOfType<TutorialAfonzo>().gameObject,0.5f);
            removeArrow = true;
            Destroy(this);


        }


    }
}
