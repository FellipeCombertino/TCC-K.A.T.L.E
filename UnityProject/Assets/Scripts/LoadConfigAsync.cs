using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadConfigAsync : MonoBehaviour
{
    public PauseMenu pause;
    void Awake()
    {

        StartCoroutine(loadPause());


    }
    IEnumerator loadPause()
    {
        yield return new WaitForEndOfFrame();

        pause.Start();

    }
 
}
