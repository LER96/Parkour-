using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class Enemy : MonoBehaviour
{
    [SerializeField] float radius;
    [SerializeField] bool islooking;
    [SerializeField] LayerMask isObs; 

    [SerializeField] Transform targert;
    [SerializeField] Transform bullet;
    [SerializeField] bool attacked;
    [SerializeField] float delay;
    [SerializeField] Transform shootPoint;
    [SerializeField] Transform lookPoint;

    //[SerializeField] NavMeshAgent agent;

    [SerializeField] float distance;

    AudioSource shootingEffect;

    Vector3 newPos;
    Vector3 targetDir;

    private void Start()
    {
        shootingEffect = GetComponent<AudioSource>();
    }

    private void Update()
    {
        distance = Vector3.Distance(lookPoint.position, targert.position);
        if (distance <= radius)
        {
            CheckOnSight();
        }
    }

    private void CheckOnSight()
    {
        //set the direction 
        targetDir = (lookPoint.position - targert.position).normalized;
        RaycastHit hit;
        //shoot a raycast and check if the tay hits the target
        if (!Physics.Raycast(lookPoint.position, targetDir, out hit, distance, isObs, QueryTriggerInteraction.Collide))
        {
            islooking = true;
            transform.LookAt(targert);
            shootPoint.LookAt(targert);
            Shooting();
        }
        else
            islooking = false;
    }

    void Shooting()
    {
        //if the player already attacked// then he waits for a couple of seconds// then shoots 
        if(!attacked)
        {
            attacked = true;
            //respawn the bulletpreFab
            Instantiate(bullet, shootPoint.position, shootPoint.rotation);
            shootingEffect.Play();
            //delay and then resets 
            Invoke(nameof(ResetShoot), delay);
        }
    }

    void ResetShoot()
    {
        attacked = false;
    }

    //visible radius
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
