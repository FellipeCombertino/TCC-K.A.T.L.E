using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    public bool triggerByButtom;
    
    public Vector3 positionOpened; 
    public bool isOpen;
    private Vector3 startPos;
    private bool isRunning;



    public void Start()
    {

        startPos = transform.position;
        isRunning = true;
    }
    public void Open()
    {

        StartCoroutine(openDoor());

    }
    IEnumerator openDoor()
    {
        isOpen = true;
        while (transform.position != positionOpened)
        {
            yield return null;
            transform.position = Vector3.Lerp(transform.position, startPos + positionOpened, Time.deltaTime*5);




        }
    


    }
    private void OnDrawGizmos()
    {
        if(!isRunning)
        startPos = transform.position;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, startPos + positionOpened);
        Gizmos.DrawSphere(transform.position, 0.1f);
        Gizmos.DrawSphere(startPos + positionOpened, 0.1f);



    }



}
