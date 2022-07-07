using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float radiusVis;
    [SerializeField] float delay;
    [SerializeField] LayerMask isObs;
    [SerializeField] Animator shootAnimation;
    [SerializeField] Transform target;
    [SerializeField] Transform hand;
    [SerializeField] Transform pointShoot;
    [SerializeField] Transform bulletPrefab;
    [SerializeField] Transform lookpoint;
    bool inrange;
    bool islooking;
    float distance;

    Vector3 targetDir;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("LookingFor");
    }

    IEnumerator LookingFor()
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            if(inrange)
            {
                Onsight();
            }
        }
    }

    private void Update()
    {
        InRange();
        if(inrange)
        {
            transform.LookAt(target);
        }
    }

    void InRange()
    {
        distance = Vector3.Distance(transform.position, target.position);
        if (distance <= radiusVis)
        {
            inrange = true;
        }
        else
            inrange = false;
    }

    void Onsight()
    {
        targetDir = (lookpoint.position - target.position).normalized;
        if (Physics.Raycast(lookpoint.position, targetDir, distance, isObs))
        {
            return;
        }
        else
        {
            hand.LookAt(target);
            Shoot();

        }
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, pointShoot.position, pointShoot.rotation);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radiusVis);
    }
}
