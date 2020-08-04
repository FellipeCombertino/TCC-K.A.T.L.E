using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    public AnimationCurve curvaVelocidade_Peso;
    public AnimationCurve curvaPulo_Peso;
    public float rotationSpeed = 15f;
    public float speed = 8f;
    public Vector3 velocity;
    public float weight;
    public float totalWeight;
    public float maxWeight;
    public bool canControll;
    //public float gravity = 9.8f;
    public float timeJump;
    public bool infinityJump;
    private float timeToJumpAgain;
    public bool kimTutorial;
    public GameObject Cesta;
    public SkinnedMeshRenderer bartoSkinnedMesh;

    public float jumpSpeed;
    public float moveSpeed;

    private float firstWeight;
    private float firstJumpSpeed;
    private float firstSpeed;
    public float jumpWeightSensi;
    public float SpeedWeightSensi;
    public bool canJump;
    public GameObject Interact;
    public bool showInteract;
    public GameObject otherPlayer;
    public Rigidbody rb;
    public Quaternion desiredRotation;
    public bool forceLookAt;
    public Transform lookTo;


    public float distanceToStop;
    public static Player Instance { get; set; }

    public Animator kimAnimator;

    public enum PlayerStates { Idle, Walk, Jump, GetDown, Fight, WinFight, LoseFight, PickUp, Falling, IdleBreath, Talking, None }
    public PlayerStates currentState;

    public enum ItemBeingUsed { None, InvisibleStone, LightFruit }
    public ItemBeingUsed currentItemUsed;

    public bool forceKimStop, overrideAnimation;
    [HideInInspector]
    public bool forceJump, forceGetDown, forcePickUp, forceFall;

    public Transform kimFeet;
    public bool isGrounded, hasCoroutineStarted;
    public LayerMask ignoreCollisionGround;

    public AudioSource playerSound;
    public AudioClip errorSound;

    private void Start()
    {
        firstWeight = weight;
        firstJumpSpeed = jumpSpeed;
        firstSpeed = speed;
        rb = GetComponent<Rigidbody>();
        desiredRotation = transform.rotation;
        ignoreCollisionGround = ~ignoreCollisionGround;
        Instance = this;
        playerSound = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (kimTutorial)
        {
            bartoSkinnedMesh.enabled = false;
            Cesta.SetActive(true);
        }
        else
        {
            bartoSkinnedMesh.enabled = true;
            Cesta.SetActive(false);

        }
        Interact.SetActive(showInteract);
        timeToJumpAgain += Time.deltaTime;
        if (BagController.Instance.asBag)
        {
            totalWeight = weight + BagController.Instance.weight;
          

        }
        else
        {
            totalWeight = weight;

        }

        if (!forceKimStop)
        {
            Movement();
        }
        else
        {

            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, desiredRotation.eulerAngles.y, 0), rotationSpeed * Time.deltaTime);
        }

        if (forceLookAt)
        {
            desiredRotation = Quaternion.LookRotation(Vector3.Normalize(lookTo.position - transform.position));
        }

        WeightControll();
        AnimationKim();
        RayCastGround();
    }

    public void WeightControll()
    {
        float difWeight = Mathf.Clamp(totalWeight - firstWeight , 0 , 999999f);

        //com curvas
        speed = firstSpeed- difWeight* curvaVelocidade_Peso.Evaluate(totalWeight/maxWeight);
        jumpSpeed = firstJumpSpeed - difWeight* curvaPulo_Peso.Evaluate(totalWeight / maxWeight);

        //sem curvas --LEGACY--
        //speed = firstSpeed- difWeight/ SpeedWeightSensi;
        //jumpSpeed = firstJumpSpeed - difWeight/ jumpWeightSensi;
    }

    public void Movement()
    {
        
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        //impedir de forcar contra uma parede
    
        
        if (canControll)
        {
            Vector3 moveDir = (Camera.main.transform.forward * inputY) + (Camera.main.transform.right * inputX);
            moveDir.y = 0f;
            moveDir.Normalize();

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, desiredRotation.eulerAngles.y, 0), rotationSpeed * Time.deltaTime);

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

            if (canJump)
            {
                RaycastHit hit;
                if (!Physics.Raycast(transform.position, transform.forward, out hit, distanceToStop, ignoreCollisionGround))
                {


                   
                        velocity = transform.forward * speed * Mathf.Clamp(Mathf.Abs(inputY) + Mathf.Abs(inputX), -1, 1) * speed;
                    

                }
                else
                {
                if (!hit.collider.isTrigger)
                {
                    velocity = Vector3.zero;

                    }
                }
            }

            else
            { RaycastHit hit;
                if (!Physics.Raycast(transform.position, transform.forward, out hit, distanceToStop, ignoreCollisionGround))
                {

                    velocity = new Vector3((transform.forward * speed * Mathf.Clamp(Mathf.Abs(inputY) + Mathf.Abs(inputX), -1, 1) * speed).x, velocity.y, (transform.forward * speed * Mathf.Clamp(Mathf.Abs(inputY) + Mathf.Abs(inputX), -1, 1) * speed).z);
                }
                else
                {

                    velocity = Vector3.zero;

                }
            }
            if (!kimTutorial)
            {
                if (canJump)
                {

                    if (Input.GetButtonDown("Jump") && timeToJumpAgain > 0.5f)
                    {
                        // velocity.y = jumpSpeed;
                        if (!infinityJump)
                        {
                            canJump = false;
                            StopCoroutine(TimeJump());
                        }
                        
                        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                        rb.AddForce(Vector3.up * jumpSpeed * 50);
                        forceJump = true;
                        timeToJumpAgain = 0;
                    }
                }
                else
                {

                    if (infinityJump)
                    {
                        if (Input.GetButtonDown("Jump"))
                        {


                            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                            rb.AddForce(Vector3.up * jumpSpeed * 50);
                            forceJump = true;
                            timeToJumpAgain = 0;
                        }
                    }


                }
              
            }

        }
        else
        {

            velocity.x = 0;
            velocity.z = 0;
        }


        velocity.y = rb.velocity.y;

        rb.velocity = velocity;
       // controller.Move((velocity * Time.deltaTime) + externalMoviment);
    }
    public void CastJump()
    {

        if (!infinityJump)
        {
            canJump = false;
            StopCoroutine(TimeJump());
        }
        forceJump = true;
        timeToJumpAgain = 0;

    }
    public void LookAtDirection(bool active,Transform lookTransform=null)
    {
        if (active)
        {
           lookTo= lookTransform;
           forceLookAt = true;
        }
        else
        {
            lookTo = null;
            forceLookAt = false;

        }
    }

    void RayCastGround()
    {
       RaycastHit hit;
       if (Physics.Raycast(kimFeet.position, Vector3.down, out hit, 1f, ignoreCollisionGround))
       {
            isGrounded = true;
            if (timeToJumpAgain > 0.5f)
            {
                canJump = true;
            }
            forceFall = false;

            hasCoroutineStarted = false;
            StopCoroutine(TimeJump());
       }
       else
       {
            isGrounded = false;
            forceFall = true;

            if (!hasCoroutineStarted)
            {
                hasCoroutineStarted = true;
                StartCoroutine(TimeJump()); // seta o canJump pra false apos x TEMPO
            }
       }
    }

    void AnimationKim()
    {
        kimAnimator.SetInteger("currentState", (int)currentState);
        
        if (overrideAnimation)
        {
            //currentState = PlayerStates.None;
            // não faz nada, apenas impede de o código ir p/ baixo enquanto isso estiver ativo, assim impedindo que a kim troque de estado.
        }
        else if (BagController.Instance.forceOpenInvent)
        {
            currentState = PlayerStates.IdleBreath;
        }
        else if(forcePickUp)
        {
            currentState = PlayerStates.PickUp;
            if (HasAnimationEnded("Kim@PickUp", 0.8f))
            {
                forcePickUp = false;
            }
        }
        else if (forceGetDown)
        {
            currentState = PlayerStates.GetDown;
            if (HasAnimationEnded("Kim@GetDown", 0.85f))
            {
                forceGetDown = false;
                forceKimStop = false;
            }
        }
        else if (forceFall)
        {
            currentState = PlayerStates.Falling;
        }
        else if (forceJump)
        {
            currentState = PlayerStates.Jump;
            if (isGrounded)
            {
                forceJump = false;
            }
        }
        else if ((velocity.x != 0 || velocity.z != 0)) // andar 
        {
            currentState = PlayerStates.Walk;
        }
        else
        {
            currentState = PlayerStates.Idle;
        }
    }

    public bool HasAnimationEnded(string animation, float timeToEnd)
    {
        return kimAnimator.GetCurrentAnimatorStateInfo(0).IsName(animation) && kimAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= timeToEnd;
    }

    public bool IsAnimationPlaying(string animation)
    {
        return kimAnimator.GetCurrentAnimatorStateInfo(0).IsName(animation);
    }

    public void ForceAnimation(bool animOverride, PlayerStates state = PlayerStates.Idle)
    {
        overrideAnimation = animOverride;
        if (overrideAnimation)
        {
            currentState = state;
            StartCoroutine(SetOverrideAnimationCond());
        }
        else
        {
            kimAnimator.SetBool("resetAnim", true);
            currentState = PlayerStates.Idle;
            StartCoroutine(ResetKimAnimation());
        }
    }

    IEnumerator SetOverrideAnimationCond ()
    {
        yield return new WaitForSeconds(0.2f);
        currentState = PlayerStates.None;
    }

    IEnumerator ResetKimAnimation()
    {
        yield return new WaitForSeconds(0.2f);
        kimAnimator.SetBool("resetAnim", false);
    }

    IEnumerator TimeJump()
    {
        yield return new WaitForSeconds(timeJump);
        canJump = false;
    }
}