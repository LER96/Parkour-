using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float visionRadius = 10f;

    Transform target;
    NavMeshAgent enemyAgent;
    
    void Start()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        target = PlayerComponent.instance.player.transform;
    }

    void Update()
    {
        float distance = Vector3.Distance(target.position, transform.position);
        if (distance <= visionRadius)
        {
            Debug.Log("Shooting");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
    }
}
