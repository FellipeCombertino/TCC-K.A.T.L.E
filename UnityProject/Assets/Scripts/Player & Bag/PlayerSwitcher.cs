using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class PlayerSwitcher : MonoBehaviour
{
    public Player PlayerController;
    public BagController BagController;
    public CameraController CamControll;
    public Transform BartoParent, bartoLandSpot, bartoGetUpSpot;
    public Vector3 firstPos;
    public Quaternion firstRot;
    public LayerMask checkTerrainCollision;
    public enum state {mochila,gato,juntos};

    public state AtualState;

    public LayerMask getDownCollision;
  

    public static PlayerSwitcher Instance;

    private void Start()
    {
        Instance = this;
        firstRot = BagController.transform.localRotation;
        firstPos = BagController.transform.localPosition;
        getDownCollision = ~getDownCollision;
        //checkTerrainCollision = ~checkTerrainCollision;
    }

    private void Update()
    {
        if (BagController.Instance.bartoCurrentState != BagController.BartoStates.Death)
        {
            if (!Player.Instance.kimTutorial)
            {
                
                Divide();
            }
        }

    }

    //timer para o clique rapido ou longo
    float timer;
    public void Divide()
    {
        timer += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.R))
        {
            timer = 0;
        }

        if (Input.GetKeyUp(KeyCode.R) && timer <= 0.2f)
        {
            if (AtualState == state.gato)
            {


               
                TransformIn(state.mochila);
            }
            else if (AtualState == state.mochila)
            {
                if (BagController.Instance.forceOpenInvent)
                {
                    Inventory.Instance.ExitInventory();
                }
                TransformIn(state.gato);
              
            }
            else if (AtualState == state.juntos)
            {
               
                if (!Player.Instance.forceKimStop && BagController.Instance.bartoCurrentState != BagController.BartoStates.GetUp
                    && Player.Instance.currentState != Player.PlayerStates.Jump && Player.Instance.currentState != Player.PlayerStates.Falling && Player.Instance.transform.parent == null)
                {
                   
                    RaycastHit hit;
                    if (Physics.Linecast(transform.position, bartoLandSpot.position, out hit, getDownCollision))
                    {
                        //Debug.Log(hit.transform.name);
                        if (!Player.Instance.playerSound.isPlaying)
                            Player.Instance.playerSound.PlayOneShot(Player.Instance.errorSound);
                    }
                    else
                    {
                      
                        Separate();
                       
                        AtualState = state.mochila;
                    }
                }
            }

            BagController.GetComponent<NavMeshAgent>().enabled = false;
        }
   
        if (Input.GetKeyUp(KeyCode.R))
        {
            BagController.GetComponent<NavMeshAgent>().enabled = false;

            if(AtualState == state.mochila)
            {
                BagController.Instance.canControll = true;
                BagController.canControll = true;
            }

            if (AtualState != state.mochila)
            {
                BagController.canControll = false;
            }

        }
        if (Input.GetKey(KeyCode.R) && timer > 0.2f && ShowDirection.Instance.distance < ShowDirection.Instance.maxDistance && BagController.Instance.canBeCalled)
        {
            if(AtualState != state.juntos)
            BagController.GetComponent<NavMeshAgent>().enabled = true;

            if (AtualState == state.gato || AtualState == state.mochila)
            {
                BagController.Instance.canControll = false;
               
                BagController.GetComponent<NavMeshAgent>().SetDestination(PlayerController.transform.position - PlayerController.transform.forward* 1.5f);
                RaycastHit hit;
                
                if (Physics.Raycast(BagController.Instance.transform.position + Vector3.up * 0.2f, Vector3.down, out hit, 1f, checkTerrainCollision))
                {

                    BagController.Instance.bartoNavMesh.baseOffset = -0.1f;


                }
                else
                {
                    BagController.Instance.bartoNavMesh.baseOffset = -0.1f;

                }

                Debug.DrawLine(BagController.Instance.transform.position + Vector3.up*0.2f, hit.point,Color.blue);

                if (BagController.GetComponent<NavMeshAgent>().remainingDistance <= 0.3f && BagController.GetComponent<NavMeshAgent>().remainingDistance > 0)
                {

                    ReturnBag();

                }
                if (AtualState == state.mochila)
                {
                    BagController.Instance.canControll = false;
                    BagController.Instance.GetComponent<PlayerBagCollision>().enabled = false;
                }

            }
        }
    }

     
    void TransformIn(state s)
    {

        switch (s)
        {
            case state.mochila:
                CameraFocus(false); // foca o barto
                break;
            case state.gato:
                CameraFocus(true); // foca o player
                break;
            case state.juntos:
                break;
            default:
                break;
        }

    }

    void Separate()
    {
        BagController.Instance.asBag = false;
        PlayerController.canControll = false;
        Player.Instance.GetComponent<PlayerBagCollision>().enabled = false;
        BagController.Instance.GetComponent<PlayerBagCollision>().enabled = true;
        StopAllCoroutines();

        // as var abaixo eu transferi pro método de animação do BagController
        //BagController.canControll = true;
        //BagController.transform.parent = null;
        //BagController.controller.enabled = true;
        //CamControll.cameraFocus = BagController.transform;

        // Animation da kim e do barto descendo
        Player.Instance.forceGetDown = true;
        Player.Instance.forceKimStop = true;
        BagController.Instance.forceGetDown = true;
        BagController.Instance.forceBartoStop = true;
    }

    void ReturnBag()
    {
      

        
        AtualState = state.juntos;
        PlayerController.canControll = true;
        BagController.canControll = false;

        Player.Instance.GetComponent<PlayerBagCollision>().enabled = true;
        BagController.Instance.GetComponent<PlayerBagCollision>().enabled = false;

        //BagController.transform.parent = BartoParent;
        BagController.GetComponent<NavMeshAgent>().enabled = false;
        BagController.Instance.asBag = true;

        // animações
        Player.Instance.forceGetDown = true;
        Player.Instance.forceKimStop = true;
        BagController.Instance.forceGetUp = true;
        BagController.Instance.forceBartoStop = true;
    }

    public void CameraFocus(bool focusPlayer)
    {
        if (focusPlayer)
        {
            PlayerController.canControll = true;
            BagController.canControll = false;
            BagController.Instance.GetComponent<PlayerBagCollision>().enabled = false;
            CamControll.cameraFocus = PlayerController.transform;
            AtualState = state.gato;
        }
        else
        {
            PlayerController.canControll = false;
            Player.Instance.GetComponent<PlayerBagCollision>().enabled = false;
            BagController.canControll = true;
            BagController.Instance.GetComponent<PlayerBagCollision>().enabled = true;
            CamControll.cameraFocus = BagController.transform;
            AtualState = state.mochila;
        }
    }
}
