using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundBetweenTime : MonoBehaviour
{
    public AudioSource source;
    public Vector2 RandomBetweenSeconds;
    public AudioClip soudToPlay;
    void Start()
    {

        
        StartCoroutine(timer());

    }

    IEnumerator timer()
    {
        while (0 == 0)
        {

            yield return new WaitForSeconds(Random.Range(RandomBetweenSeconds.x, RandomBetweenSeconds.y));

            source.pitch = Random.Range(.75f, 1.2f);
           //source.PlayOneShot(soudToPlay);
           

            yield return null;
        }
    }



}
