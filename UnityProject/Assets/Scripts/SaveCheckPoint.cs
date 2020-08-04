using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveCheckPoint : MonoBehaviour
{

    public bool kim;
    public bool barto;

    private void OnTriggerEnter(Collider other)
    {

        if (other.transform.CompareTag("Player") && BagController.Instance.bartoCurrentState != BagController.BartoStates.Death)
        {

            if (BagController.Instance.canControll)
            {

                GetComponentInParent<CheckPointController>().LastCheckPointBarto = transform.position;


            }
            else
            {
                if (BagController.Instance.asBag)
                {
                    GetComponentInParent<CheckPointController>().LastCheckPointKim = transform.position;
                    GetComponentInParent<CheckPointController>().LastCheckPointBarto = transform.position;

                }
                GetComponentInParent<CheckPointController>().LastCheckPointKim = transform.position;

            }
            if (barto && kim)
            {
               
                Destroy(gameObject);

            }

        }
               

    }




}
