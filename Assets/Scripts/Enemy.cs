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
            if(inrange)
            {
                Onsight();
            }
            yield return new WaitForSeconds(delay);
        }
    }

    private void Update()
    {
        InRange();
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
        targetDir = (transform.position - target.position).normalized;
        if (!Physics.Raycast(transform.position, targetDir, distance, isObs))
        {
            islooking = true;
            transform.LookAt(target, Vector3.up);
            hand.LookAt(target);
            Debug.Log("Shoot");
            Shoot();
        }
        else
            islooking = false;

        shootAnimation.SetBool("shoot", islooking);
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
