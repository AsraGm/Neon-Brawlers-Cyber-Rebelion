using System.Collections;
using UnityEngine;

public class RobotAttack : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float delayTimeDamage = 3f;
    private EnemyPatrol enemyPatrol;

    private void Start()
    {
        enemyPatrol = GetComponent<EnemyPatrol>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (enemyPatrol.isStunned)
        {
            if (other.CompareTag("Player"))
            {
                enemyPatrol.StopStun();
                StartCoroutine(DelayTime());

            }
        }

        if (other.CompareTag("Player") && enemyPatrol.canAttack)
        {
            other.GetComponent<PlayerDamage>().Die(damage);
        }
    }

    public void StopRobotAttack()
    {
        enemyPatrol.canAttack = false;
    }

    public void RobotCanAttack()
    {
        enemyPatrol.canAttack = true;
    }

    private IEnumerator DelayTime()
    {
        yield return new WaitForSeconds(delayTimeDamage);
        RobotCanAttack();
    }

}
