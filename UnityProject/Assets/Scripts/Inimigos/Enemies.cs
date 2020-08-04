using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    public enum EnemyType { Chaser, Shooter }
    public enum EnemyState { Patrolling, Chasing, Stealing, Fighting, WinFight, LoseFight, Respawning }
    public EnemyType enemyType;
    public EnemyState enemyState;

    public float speedToChase;
    float speedToPatrol;

    [Tooltip("Quando ativado, o inimigo irá se movimentar aleatoriamente entre os pontos definidos!")]
    public bool randomPath;

    public Transform runAwayPoint;
    public Transform[] path;

    [HideInInspector]
    public Transform whatToChase;

    [HideInInspector]
    public UnityEngine.AI.NavMeshAgent enemyNavMesh;

    Vector3 currentPos;
    int nextPos;

    public GameObject patrollerSkin, chaserSkin1, chaserSkin2;
    public Transform joint1patrol, joint2patrol, jointBartoPatrol, jointchaser1, jointchaser2, jointBartoChaser1, jointBartoChaser2;
    public Transform showItem1, showItem2, showBarto;

    public int timesToWin, timesToLose;

    [Range(0.1f, 1)]
    public float speedFight;

    Quaternion desiredRotation;
    public Animator enemyAnimator;
    CapsuleCollider enemyCollider;

    public ChatSettings chatWin, chatDefeat, chatDefeatBartoSolo, chatDefeatBartoKim;

    void Start()
    {
        enemyNavMesh = gameObject.GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
        currentPos = transform.position;
        speedToPatrol = enemyNavMesh.speed;
        nextPos = 0; // aqui é pra determinar o index do array (path) p/ qual o inimigo vai seguir e fazer o caminho bonitinho

        switch (enemyType)
        {
            case EnemyType.Chaser:
                int sorteio = Random.Range(0, 3);

                if (sorteio == 0) // chaser 1 sorteado
                {
                    Destroy(chaserSkin1);
                    Destroy(chaserSkin2);
                    showItem1.transform.parent = joint1patrol;
                    showItem2.transform.parent = joint2patrol;
                    showBarto.transform.parent = jointBartoPatrol;
                    enemyAnimator = patrollerSkin.GetComponent<Animator>();
                }
                else if (sorteio == 1) // chaser 2 sorteado
                {
                    Destroy(patrollerSkin);
                    Destroy(chaserSkin2);
                    showItem1.transform.parent = jointchaser1;
                    showBarto.transform.parent = jointBartoChaser1;
                    enemyAnimator = chaserSkin1.GetComponent<Animator>();
                }
                else // chaser 3 sorteado
                {
                    Destroy(patrollerSkin);
                    Destroy(chaserSkin1);
                    showItem1.transform.parent = jointchaser2;
                    showBarto.transform.parent = jointBartoChaser2;
                    enemyAnimator = chaserSkin2.GetComponent<Animator>();
                }
                break;
            case EnemyType.Shooter:
                break;
            default:
                break;
        }

        showItem1.transform.localPosition = Vector3.zero;
        showItem1.transform.localScale = Vector3.one;
        showItem1.transform.localRotation = Quaternion.identity;

        showItem2.transform.localPosition = Vector3.zero;
        showItem2.transform.localScale = Vector3.one;
        showItem2.transform.localRotation = Quaternion.identity;

        showBarto.transform.localPosition = Vector3.zero;
        showBarto.transform.localScale = Vector3.one;
        showBarto.transform.localRotation = Quaternion.identity;

        enemyCollider = enemyNavMesh.GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        switch (enemyType)
        {
            case EnemyType.Chaser:
                ChaserEnemy();
                break;
            case EnemyType.Shooter:
                ShooterEnemy();
                break;
            default:
                break;
        }

        enemyAnimator.SetInteger("currentState", (int)enemyState);

    }

    void ChaserEnemy()
    {
        switch (enemyState)
        {
            case EnemyState.Patrolling:
                PatrolBehaviour();
                enemyNavMesh.stoppingDistance = 0;
                enemyCollider.enabled = true;
                break;
            case EnemyState.Chasing:
                ChaseBehaviour();
                //enemyNavMesh.stoppingDistance = 3;
                enemyCollider.enabled = false;
                break;
            case EnemyState.Stealing:
                StealBehaviour();
                enemyNavMesh.stoppingDistance = 0;
                enemyCollider.enabled = false;
                break;
            case EnemyState.Fighting:
                FightBehaviour();
                //enemyNavMesh.stoppingDistance = 3;
                enemyNavMesh.velocity = Vector3.zero;
                enemyNavMesh.transform.localRotation = Quaternion.Slerp(enemyNavMesh.transform.localRotation, Quaternion.Euler(0, desiredRotation.eulerAngles.y, 0), 15 * Time.deltaTime);
                enemyCollider.enabled = false;
                break;
            case EnemyState.WinFight:
                if (patrollerSkin)
                {
                    WinFight(1);
                }
                else if (chaserSkin1)
                {
                    WinFight(2);
                }
                else if (chaserSkin2)
                {
                    WinFight(3);
                }
                break;
            case EnemyState.LoseFight:
                if (patrollerSkin)
                {
                    LoseFight(1);
                }
                else if (chaserSkin1)
                {
                    LoseFight(2);
                }
                else if (chaserSkin2)
                {
                    LoseFight(3);
                }
                break;
            case EnemyState.Respawning:
                StartCoroutine(ResetEnemy());
                break;
            default:
                break;
        }        
    }

    public bool HasAnimationEnded(string animation, float timeToEnd)
    {
        return enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName(animation) && enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= timeToEnd;
    }

    void WinFight(int enemyNumber)
    {
        if (HasAnimationEnded("Inim" + enemyNumber + "@FightWin", 0.3f))
        {
            BagController.Instance.forceScare = true;
        }

        if (HasAnimationEnded("Inim" + enemyNumber + "@FightWin", 0.8f))
        {
            enemyState = EnemyState.Stealing;
            if (enemyAnimator.GetBool("withBarto")) // se roubou o barto
            {
                BagController.Instance.bartoCurrentState = BagController.BartoStates.Death;
                BagController.Instance.transform.parent = showBarto;
                BagController.Instance.transform.localPosition = Vector3.zero;
                BagController.Instance.transform.localRotation = Quaternion.identity;
            }
            else // se roubou um item
            {
                StartCoroutine(ResetPlayerToNormalCondition());
            }
        }
    }

    void LoseFight(int enemyNumber)
    {
        if (HasAnimationEnded("Inim" + enemyNumber + "@FightLose", 0.8f))
        {
            enemyState = EnemyState.Stealing;
        }
    }

    void ShooterEnemy()
    {
        // será que algum dia vai ser construido?
    }

    // fica alternando o destino da navmesh para o próximo ponto em sua trilha
    void PatrolBehaviour()
    {
        if (enemyNavMesh.isStopped)
        {
            enemyNavMesh.isStopped = false;
        }

        if (enemyNavMesh.remainingDistance < 0.5f)
        {
            if (randomPath)
            {
                nextPos = Random.Range(0, path.Length);
            }
            else
            {
                nextPos++;
                if (nextPos == path.Length)
                {
                    nextPos = 0;
                }
            }
            currentPos = path[nextPos].position;
        }
        enemyNavMesh.SetDestination(currentPos);
    }

    // alterna o destino da navmesh para perseguir o player
    void ChaseBehaviour()
    {
        if (enemyNavMesh.isStopped)
        {
            enemyNavMesh.isStopped = false;
        }

        enemyNavMesh.SetDestination(whatToChase.position);

        if (enemyNavMesh.speed > speedToPatrol)
        {
            enemyNavMesh.speed -= Time.deltaTime;
        }
    }

    public void StartDuel()
    {
        enemyState = EnemyState.Fighting;
        EnemyHUD.Instance.ResetChallenge(timesToLose, timesToWin, speedFight);

        if (BagController.Instance.forceOpenInvent)
        {
            Inventory.Instance.ExitInventory();
        }

        Player.Instance.LookAtDirection(true, enemyNavMesh.transform);

        desiredRotation = Quaternion.LookRotation(Vector3.Normalize(Player.Instance.transform.position - enemyNavMesh.transform.position));

        Player.Instance.forceKimStop = true;
        Player.Instance.ForceAnimation(true, Player.PlayerStates.Fight);
        // Player.Instance.rb.isKinematic = true;
        BagController.Instance.forceBartoStop = true;
    }

    void FightBehaviour()
    {
        if (!enemyNavMesh.isStopped)
        {
            enemyNavMesh.isStopped = true;
        }

        Player.Instance.LookAtDirection(false);

        if (EnemyHUD.Instance.wonXTimes >= EnemyHUD.Instance.winGameWith)
        {
            if (!Player.Instance.IsAnimationPlaying("Kim@WinFight"))
            {
                Player.Instance.ForceAnimation(true, Player.PlayerStates.WinFight);
            }

            if (Player.Instance.HasAnimationEnded("Kim@WinFight", 0.4f))
            {
                enemyState = EnemyState.LoseFight; // apenas espanto o inimigo
                Player.Instance.forceKimStop = false;
                Player.Instance.rb.isKinematic = false;
                BagController.Instance.forceBartoStop = false;
                Player.Instance.ForceAnimation(false);
                ChatSystem.Instance.DisplayMessage(chatWin);
            }
        }
        else if (EnemyHUD.Instance.lostXTimes >= EnemyHUD.Instance.loseGameWith)
        {
            if (!Player.Instance.IsAnimationPlaying("Kim@LoseFight"))
            {
                Player.Instance.ForceAnimation(true, Player.PlayerStates.LoseFight);
            }

            if (Player.Instance.HasAnimationEnded("Kim@LoseFight", 0.2f))
            {
               // BagController.Instance.forceScare = true;

                if (Inventory.Instance.itemStored.Count > 0) // se houver itens a serem roubados, ele ficará com os itens
                {
                    StealItem();
                }
                else // senão, ficará com o barto
                {
                    StealBarto();
                }
            }
        }
    }

    void StealBehaviour()
    {
        if (enemyNavMesh.isStopped)
        {
            enemyNavMesh.isStopped = false;
        }
        enemyNavMesh.SetDestination(runAwayPoint.position);
        enemyNavMesh.speed = 8;

        // apenas quando o inimigo chega ao final do seu percurso
        if (enemyNavMesh.remainingDistance > 0 && enemyNavMesh.remainingDistance < 0.2f && enemyState == EnemyState.Stealing)
        {
            if (BagController.Instance.bartoCurrentState != BagController.BartoStates.Death) // se ele roubou um item ou perdeu o desafio, ele é apenas destruido
            {
                FindObjectOfType<PauseMenu>().Sfxs.Remove(GetComponentInChildren<AudioSource>());
                Destroy(gameObject);
            }
            else // senão, ele roubou o barto... e ai condições acontecem para o respawn do barto
            {
                // reseto o barto para um estado comum no chão e sua animação tbm
                BagController.Instance.bartoCurrentState = BagController.BartoStates.IdleGround;
                BagController.Instance.ResetBartoAnimation("Ground");
                BagController.Instance.forceExitInvent = false;

                // deixo o barto e a kim se mexerem novamente
                BagController.Instance.forceBartoStop = false;
                BagController.Instance.rb.isKinematic = false;
                if (BagController.Instance.bartoNavMesh.enabled)
                {
                    BagController.Instance.bartoNavMesh.isStopped = false;
                }
                Player.Instance.forceKimStop = false;
                Player.Instance.rb.isKinematic = false;
                Player.Instance.ForceAnimation(false); // reseto a anim da kim

                // o barto fica livre no chão e reseta sua posição para o último checkpoint, assim como a Kim
                BagController.Instance.transform.parent = null;
                CheckPointController.Instance.Respawn(true);
                BagController.Instance.transform.rotation = Quaternion.identity;
                BagController.Instance.desiredRotation = Quaternion.Euler(0, BagController.Instance.transform.localRotation.y, BagController.Instance.transform.localRotation.z);

                // o próprio inimigo também reseta para sua posição original
                enemyState = EnemyState.Respawning;
            }
        }
    }

    IEnumerator ResetEnemy()
    {
        yield return new WaitForSeconds(2);
        enemyState = EnemyState.Patrolling;
        enemyNavMesh.transform.localPosition = Vector3.zero;
        enemyAnimator.SetBool("withBarto", false);
    }

    void StealItem()
    {
        Items stolenItem = Inventory.Instance.itemStored[Random.Range(0, Inventory.Instance.itemStored.Count)];
        stolenItem.transform.parent = null;
        //stolenItem.particle.gameObject.SetActive(true);
        stolenItem.transform.position = showItem1.position;
        stolenItem.transform.parent = showItem1;
        stolenItem.transform.localScale = Vector3.one;
        Inventory.Instance.itemStored.Remove(stolenItem);
        BagController.Instance.weight -= stolenItem.weight;

        // aqui eu checo dnv se tem item no inventário, pq o inimigo de patrulha rouba 2 itens.
        if (patrollerSkin && Inventory.Instance.itemStored.Count > 0)
        {
            Items stolenItem2 = Inventory.Instance.itemStored[Random.Range(0, Inventory.Instance.itemStored.Count)];
            stolenItem2.transform.parent = null;
            //stolenItem2.particle.gameObject.SetActive(true);
            stolenItem2.transform.position = showItem2.position;
            stolenItem2.transform.parent = showItem2;
            stolenItem2.transform.localScale = Vector3.one;
            Inventory.Instance.itemStored.Remove(stolenItem2);
            BagController.Instance.weight -= stolenItem2.weight;
        }

        enemyState = EnemyState.WinFight;
        enemyAnimator.SetBool("withBarto", false);

        Inventory.Instance.UpdateBartoSpikes();
        Inventory.Instance.UpdateExistingItens();

        ChatSystem.Instance.DisplayMessage(chatDefeat);
        //ResetPlayerToNormalCondition();
    }

    public void StealBarto()
    {
        PlayerSwitcher.Instance.CameraFocus(false); // a camera foca o barto
        BagController.Instance.forceBartoStop = true;
        BagController.Instance.rb.isKinematic = true;

        if (BagController.Instance.bartoNavMesh.enabled)
        {
            BagController.Instance.bartoNavMesh.isStopped = true;
            BagController.Instance.bartoNavMesh.enabled = false;
        }

        bool bartoWithKim = false;

        if (BagController.Instance.asBag)
        {
            BagController.Instance.asBag = false;
            bartoWithKim = true;
        }

        Player.Instance.forceKimStop = true;
        

        if (bartoWithKim)
        {
            enemyState = EnemyState.WinFight;
            enemyAnimator.SetBool("withBarto", true);
            ChatSystem.Instance.DisplayMessage(chatDefeatBartoKim);
        }
        else
        {
            enemyState = EnemyState.Stealing;
            enemyAnimator.SetBool("withBarto", true);
            BagController.Instance.bartoCurrentState = BagController.BartoStates.Death;
            BagController.Instance.transform.parent = showBarto;
            BagController.Instance.transform.localPosition = Vector3.zero;
            BagController.Instance.transform.localRotation = Quaternion.identity;
            ChatSystem.Instance.DisplayMessage(chatDefeatBartoSolo);
        }

    }

    IEnumerator ResetPlayerToNormalCondition()
    {
        yield return new WaitForSeconds(1);
        Player.Instance.forceKimStop = false;
        Player.Instance.rb.isKinematic = false;
        Player.Instance.ForceAnimation(false);
        BagController.Instance.forceBartoStop = false;
    }
}
