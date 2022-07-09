using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{

    //EnemyObj
    [SerializeField] Transform pointShoot;
    [SerializeField] Transform bulletPrefab;
    [SerializeField] Transform lookpoint;

    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform player;
    [SerializeField] LayerMask whatIsGround, whatIsPlayer;

    [SerializeField] Vector3 walkPoint;
    bool walkPointSet;
    [SerializeField] float walkPointRange;

    [SerializeField] float timeBetweenAttacks;
    bool alreadyAttacked;

    [SerializeField] float sightRange, attackRange;
    [SerializeField] bool playerInSightRange, playerInAttackRange;

    Vector3 targetDir;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {

        var distance = Vector3.Distance(lookpoint.position, player.position);
        if (distance <= sightRange)
        {
            CheckInfront();
        }

    }

    private void Patroling()
    {
        if (!walkPointSet)
            SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);
    }

    private void SearchWalkPoint()
    {
        float randomPointZ = Random.Range(-walkPointRange, walkPointRange);
        float randomPointX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomPointX, transform.position.y, transform.position.z + randomPointZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1)
            walkPointSet = true;
    }

    void CheckInfront()
    {
        transform.LookAt(player);
        targetDir = lookpoint.position - player.position;
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 100;
        //Check for sight and attack Range
        playerInSightRange = Physics.Raycast(transform.position, targetDir, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.Raycast(transform.position, targetDir, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange)
            Patroling();
        if (playerInSightRange && !playerInAttackRange)
        {
            LookFor();
            Debug.DrawRay(transform.position, forward, Color.green);
        }
        if (playerInSightRange && playerInAttackRange)
        {
            AttackPlayer();
            Debug.DrawRay(transform.position, forward, Color.red);
        }
    }

    private void LookFor()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);


        if (!alreadyAttacked)
        {
            alreadyAttacked = true;
            Instantiate(bulletPrefab, pointShoot.position, pointShoot.rotation);
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, walkPointRange);
    }

}
