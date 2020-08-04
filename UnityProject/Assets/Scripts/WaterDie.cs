using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDie : MonoBehaviour
{

    AudioSource waterSound;
    public AudioClip dieSound;

    private void Start()
    {
        waterSound = GetComponent<AudioSource>();

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.transform.CompareTag("Player"))
        {


            CheckPointController.Instance.Respawn(false);
            if (!waterSound.isPlaying)
            {
                waterSound.PlayOneShot(dieSound);
            }


        }


    }

}
