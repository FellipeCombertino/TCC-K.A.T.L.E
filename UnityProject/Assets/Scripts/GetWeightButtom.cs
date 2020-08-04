using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetWeightButtom : MonoBehaviour
{
    public float totalWeightPlayer;
    public float totalWeightBarto;
    public float totalWeightItem;
    public void Update()
    {
        if (transform.parent.parent.GetComponent<Buttom>())
        {
            transform.parent.parent.GetComponent<Buttom>().atualWeight = totalWeightBarto + totalWeightPlayer + totalWeightItem;
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            if(other.GetComponent<BagController>() != null)
            {
                totalWeightBarto = other.GetComponent<BagController>().weight;
            }

            if(other.GetComponent<Player>() != null)
            {
                if (BagController.Instance.asBag)
                {
                    totalWeightPlayer = other.GetComponent<Player>().totalWeight;
                }
                else
                {

                    totalWeightPlayer = other.GetComponent<Player>().weight;

                }
            }
        }

        if (other.transform.CompareTag("Item"))
        {
            print(other.transform.name);
            totalWeightItem += other.GetComponent<Items>().weight;
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.transform.CompareTag("Player"))
        {

            if (other.GetComponent<BagController>() != null)
            {

                totalWeightBarto = 0;


            }
            if (other.GetComponent<Player>() != null)
            {

                totalWeightPlayer = 0;

            }




        }

        if (other.transform.CompareTag("Item"))
        {


            totalWeightItem -= other.GetComponent<Items>().weight;



        }


    }

}
