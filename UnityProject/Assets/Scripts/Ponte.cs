using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ponte : MonoBehaviour
{

    GetWeightButtom peso;
    public float pesoMaximo;
    public float velocity;
    public Transform lado1, lado2;
    public float lado1PosBreak, lado2PosBreak;

    void Start()
    {
        peso = GetComponent<GetWeightButtom>();
    }


    private void OnTriggerStay(Collider other)
    {
        if (peso.totalWeightBarto + peso.totalWeightItem + peso.totalWeightPlayer >= pesoMaximo)
        {

      
            Player.Instance.CastJump();
            lado1.localRotation = Quaternion.Lerp(lado1.localRotation, Quaternion.Euler(lado1PosBreak, 0, 0), Time.deltaTime * velocity);
            lado2.localRotation = Quaternion.Lerp(lado2.localRotation, Quaternion.Euler(lado2PosBreak, 0, 0), Time.deltaTime * velocity);
        }
    }

}
