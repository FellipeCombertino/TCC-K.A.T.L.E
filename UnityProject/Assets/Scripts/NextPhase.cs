using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextPhase : MonoBehaviour
{
    public string levelName;
    public FadeOutInForCut fade;
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {

           fade.FadeIn();
           StartCoroutine(goLevel());


        }




    }
    IEnumerator goLevel()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(levelName);
               


    }
}
