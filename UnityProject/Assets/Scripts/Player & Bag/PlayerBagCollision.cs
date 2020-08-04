using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBagCollision : MonoBehaviour
{
    void EnemyDetection(Transform enemy)
    {
        // não esteja usando um item de invisibilidade e o barto não esteja morto e o player não esteja lutando
        if (Player.Instance.currentItemUsed != Player.ItemBeingUsed.InvisibleStone 
            && BagController.Instance.bartoCurrentState != BagController.BartoStates.Death && (Player.Instance.currentState != Player.PlayerStates.None || BagController.Instance.forceOpenInvent))
        {
            // se o inimigo avistar o player, começará a seguir ele com um boost rápido de velocidade
            if (enemy.CompareTag("Chase") && this.enabled)
            {
                Enemies chaserEnemy = enemy.GetComponentInParent<Enemies>();

                if (chaserEnemy.enemyType == Enemies.EnemyType.Chaser && (int)chaserEnemy.enemyState < 2)
                {
                    chaserEnemy.enemyState = Enemies.EnemyState.Chasing;
                    chaserEnemy.whatToChase = transform;
                    chaserEnemy.enemyNavMesh.speed = chaserEnemy.speedToChase;
                }
            }

            // se o inimigo encostar no player, o player será roubado.
            if (enemy.CompareTag("Steal"))
            {
                Enemies stealerEnemy = enemy.GetComponentInParent<Enemies>();

                if ((int)stealerEnemy.enemyState < 2)
                {
                    // se o barto estiver sozinho, é roubado na mesma hora c/ game over.
                    if ((BagController.Instance.bartoNavMesh.enabled || (!BagController.Instance.asBag && Vector3.Distance(enemy.position, BagController.Instance.transform.position) < 3f)))
                    {
                        stealerEnemy.StealBarto();
                        if (BagController.Instance.forceOpenInvent)
                        {
                            Inventory.Instance.ExitInventory();
                        }
                    }
                    else if (this.enabled && BagController.Instance.asBag) // se estiver com a Kim, o duelo começa
                    {
                        stealerEnemy.StartDuel();
                    }
                }
            }
        }
    }

    void ResetEnemyPatrol(Transform enemy)
    {
        // o inimigo volta a patrulhar caso o player saia do range de visão dele
        if (enemy.CompareTag("Chase") && this.enabled && Player.Instance.currentItemUsed != Player.ItemBeingUsed.InvisibleStone)
        {
            Enemies chaserEnemy = enemy.GetComponentInParent<Enemies>();
            if (chaserEnemy.enemyType == Enemies.EnemyType.Chaser && (int)chaserEnemy.enemyState < 2)
            {
                chaserEnemy.enemyState = Enemies.EnemyState.Patrolling;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyDetection(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        ResetEnemyPatrol(other.transform);
    }
}
