using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class BagController : MonoBehaviour
{
    public AnimationCurve curvaVelocidade_Peso;
    public AnimationCurve curvaPulo_Peso;
    public float rotationSpeed = 15f;
    public float speed = 8f;
    public Vector3 velocity;
    public float moveSpeed;
    public float jumpSpeed;
    public float weight;
    //public float gravity = 9.8f;
    //public bool canJump;
    public bool canControll;
    public float maxWeight;
    private float firstWeight;
    public bool asBag;
    private float firstJumpSpeed;
    private float firstSpeed;
    public float jumpWeightSensi;
    public float SpeedWeightSensi;
    public bool canBeCalled;
    public float offsetCanBeCalled;
 
    public Vector3 externalMoviment;

    public Rigidbody rb;

    public Quaternion desiredRotation;

    public Material bartoNormalMat, bartoInviMat;

    public static BagController Instance { get; set; }
    private NavMeshPath path;

    public enum BartoStates { IdleBag, IdleGround, Walk, Scared, InventoryOpen, InventoryLoop, InventoryExit, PickUp, GetUp, GetDown, Drop, Death }
    public BartoStates bartoCurrentState;

    [HideInInspector]
    public NavMeshAgent bartoNavMesh;
    Animator bartoAnimator;
    string size;

    [HideInInspector]
    public bool forceDrop, forceScare, forceOpenInvent, forceExitInvent, forcePickUp, forceGetUp, forceGetDown;
   // [HideInInspector]
    public bool forceBartoStop;

    private void Awake()
    {
        Instance = this;
        firstWeight = weight;
        firstJumpSpeed = jumpSpeed;
        firstSpeed = speed;
    }

    private void Start()
    {
        path = new NavMeshPath();
        rb = GetComponent<Rigidbody>();
        desiredRotation = transform.rotation;
        bartoAnimator = GetComponentInChildren<Animator>();
        bartoNavMesh = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (bartoCurrentState == BartoStates.Death)
        {
            forceBartoStop = true;
            Player.Instance.forceKimStop = true;
        }

        if (!forceBartoStop)
        {
            Movement();
        }

        WeightControll();
        BartoAnimation();

        if (asBag)
        {
         //   controller.enabled = false;
        }
        else
        {
            canBeCalled = IsAgentOnNavMesh();
        }
 
    }
    public bool IsAgentOnNavMesh()
    {
        Vector3 agentPosition = transform.position;
        NavMeshHit hit;

      
        if (NavMesh.SamplePosition(agentPosition, out hit, 5, NavMesh.AllAreas))
        {

            //print("AchouNavMesh");
            if (Mathf.Approximately(agentPosition.x, hit.position.x)
                && Mathf.Approximately(agentPosition.z, hit.position.z))
            {





                    bool nav = NavMesh.CalculatePath(agentPosition, Player.Instance.transform.position, NavMesh.AllAreas, path);
                    for (int i = 0; i < path.corners.Length - 1; i++)
                    {
                        Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
                    }

                    if (path.corners.Length > 0)
                    {
                        if (Vector3.Distance(path.corners[path.corners.Length - 1], Player.Instance.transform.position) < offsetCanBeCalled)
                        {
                            return true;

                        }
                    }
                    return false;


                
            }
                    

                
        }
       

        return false;
    }

    public void WeightControll()
    {

        float difWeight = Mathf.Clamp(weight - firstWeight, 0, 999999f);

        //com curvas
        speed = firstSpeed - difWeight * curvaVelocidade_Peso.Evaluate(weight / maxWeight);
        jumpSpeed = firstJumpSpeed - difWeight * curvaPulo_Peso.Evaluate(weight / maxWeight);

        //sem curvas --LEGACY--
        //speed = firstSpeed- difWeight/ SpeedWeightSensi;
        //jumpSpeed = firstJumpSpeed - difWeight/ jumpWeightSensi;
    }

    public void Movement()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        if (canControll)
        {
            Vector3 moveDir = (CameraController.Instance.transform.forward * inputY) + (CameraController.Instance.transform.right * inputX);
            moveDir.y = 0f;
            moveDir.Normalize();

            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
           
            if (moveDir != Vector3.zero)
            {
                desiredRotation = Quaternion.LookRotation(moveDir);
                moveSpeed = (Mathf.Abs(inputX) + Mathf.Abs(inputY)) * speed;
                moveSpeed = Mathf.Abs(moveSpeed);
            }
            else
            {
                moveSpeed = 0;
            }

           // canJump = true;  desativado pq o barto n pula mais

                velocity = transform.forward * speed * Mathf.Clamp(Mathf.Abs(inputY) + Mathf.Abs(inputX), -1, 1) * speed;
          
         

            //if (canJump)        // desativado pq o barto n pula mais
            //{
            //    if (Input.GetButton("Jump"))
            //    {
            //        canJump = false;
            //        velocity.y = jumpSpeed;
            //    }
            //}
        }
        else
        {

            velocity.x = 0;
            velocity.z = 0;

        }

        if (asBag == false)
        {

            rb.isKinematic = false;
            rb.useGravity = true;


        }
        else{

            rb.isKinematic = true;
            rb.useGravity = false;



        }

       
            velocity.y = rb.velocity.y;
            rb.velocity = velocity+externalMoviment;
       
    }

    void BartoAnimation()
    {
        bartoAnimator.SetInteger("currentState", (int)bartoCurrentState);
        ChangeBartoSize();

        if (bartoCurrentState == BartoStates.Death)
        {
            bartoAnimator.Play("Bart" + size + "@Death");
        }
        else if (forceGetUp) // animação de subir
        {
            bartoCurrentState = BartoStates.GetUp;
            transform.parent = PlayerSwitcher.Instance.bartoGetUpSpot;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            rb.isKinematic = true;

            if (HasAnimationEnded("Bart" + size + "@GetUp", 0.9f))
            {
                bartoAnimator.Play("Bart" + size + "@IdleBag");
                forceGetUp = false;
                forceBartoStop = false;
                transform.parent = PlayerSwitcher.Instance.BartoParent;
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                rb.isKinematic = false;
                CameraController.Instance.cameraFocus = Player.Instance.transform;
            }
        }
        else if (forceGetDown) // animação de descer 
        {
            bartoCurrentState = BartoStates.GetDown;

            if (HasAnimationEnded("Bart" + size + "@GetDown", 0.8f))
            {
                bartoAnimator.Play("Bart" + size + "@IdleGround");

                forceGetDown = false;
                forceBartoStop = false;

                transform.parent = PlayerSwitcher.Instance.bartoLandSpot;
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                desiredRotation = Quaternion.LookRotation(transform.forward);

                canControll = true;
                Player.Instance.canControll = false;
                CameraController.Instance.cameraFocus = transform;
                transform.parent = null;

            }
        }
        else if (forceOpenInvent) // abrir o inventário
        {
            if (!IsAnimationPlaying("Bart" + size + "@InventoryLoop"))
            {
                bartoCurrentState = BartoStates.InventoryOpen;
            }

            if (HasAnimationEnded("Bart" + size + "@InventoryOpen", 0.8f))
            {
                bartoCurrentState = BartoStates.InventoryLoop;
            }
        }
        else if (forceExitInvent) // fechar inventário
        {
            bartoCurrentState = BartoStates.InventoryExit;
            if (HasAnimationEnded("Bart" + size + "@InventoryExit", 0.7f))
            {
                forceExitInvent = false;
            }
        }
        else
        {
            if (asBag) // animações enquanto estiver em cima da kim
            {
                if (Input.GetKeyDown(KeyCode.R)) // se clicar pra descer
                {
                   // bartoCurrentState = BartoStates.GetDown;
                }
                else if (forceDrop)
                {
                    bartoCurrentState = BartoStates.Drop;
                    forceDrop = false;
                }
                else if (forceScare)
                {
                    bartoCurrentState = BartoStates.Scared;
                    forceScare = false;
                }
                else
                {
                    bartoCurrentState = BartoStates.IdleBag;
                }
            }
            else // animações enquanto estiver no chão
            {
                if (bartoNavMesh.enabled)
                {
                    bartoCurrentState = BartoStates.Walk;
                }
                else if (forcePickUp)
                {
                    bartoCurrentState = BartoStates.PickUp;
                    forcePickUp = false;
                }
                else if (forceDrop)
                {
                    bartoCurrentState = BartoStates.Drop;
                    forceDrop = false;
                }
                else if ((velocity.x != 0 || velocity.z != 0)) // andar
                {
                    bartoCurrentState = BartoStates.Walk;
                }
                else // idle
                {
                    bartoCurrentState = BartoStates.IdleGround;
                }
            }
        }
    }

    public void ResetBartoAnimation(string whereToReset)
    {
        bartoAnimator.Play("Bart" + size + "@Idle" + whereToReset);
    }

    void ChangeBartoSize()
    {
        switch (weight)
        {
            case 4:
                size = "M";
                break;
            case 5:
                size = "G";
                break;
            case 6:
                size = "GG";
                break;
            default:
                size = "P";
                break;
        }

        if (bartoAnimator.GetCurrentAnimatorClipInfo(0).Length > 0)
        {
            if (GetAnimationName(size) != bartoAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name)
            {
                float jumpToTime = bartoAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                bartoAnimator.Play(GetAnimationName(size), 0, jumpToTime);
            }
        }
    }

    bool IsAnimationPlaying(string animation)
    {
        return bartoAnimator.GetCurrentAnimatorStateInfo(0).IsName(animation);
    }

    bool HasAnimationEnded(string animation, float timeToEnd)
    {
        return bartoAnimator.GetCurrentAnimatorStateInfo(0).IsName(animation) && bartoAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= timeToEnd;
    }

    string GetAnimationName(string bartoSize)
    {
        if (bartoAnimator.GetCurrentAnimatorClipInfo(0).Length > 0)
        {
            string currentClip = bartoAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            if (currentClip.Substring(5, 1) == "@")
            {
                return currentClip.Substring(0, 4) + bartoSize + currentClip.Substring(5);
            }
            else
            {
                return currentClip.Substring(0, 4) + bartoSize + currentClip.Substring(6);
            }
        }
        else
        {
            return null;
        }
    }
}