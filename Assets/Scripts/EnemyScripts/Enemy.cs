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
        targetDir = (lookPoint.position - targert.position).normalized;
        RaycastHit hit;
        if (!Physics.Raycast(lookPoint.position, targetDir, out hit, distance, isObs))
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
        
        if(!attacked)
        {
            attacked = true;
            Instantiate(bullet, shootPoint.position, shootPoint.rotation);
            shootingEffect.Play();
            Invoke(nameof(ResetShoot), delay);
        }
    }

    void ResetShoot()
    {
        attacked = false;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
