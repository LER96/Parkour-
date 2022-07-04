using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float radiusVis;
    [SerializeField] float delay;
    [SerializeField] LayerMask isPlayer;
    [SerializeField] Transform target;
    [SerializeField] Transform hand;
    [SerializeField] Transform pointShoot;
    [SerializeField] Transform bulletPrefab;
    bool inrange;
    float distance;

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
    }

    void InRange()
    {
        distance = Vector3.Distance(transform.position, target.position);
        if (distance <= radiusVis)
        {
            transform.LookAt(target, Vector3.up);
            inrange = true;
        }
        else
            inrange = false;
    }

    void Onsight()
    {
        Vector3 targetDir = (transform.position - target.position).normalized;
        if(!Physics.Raycast(transform.position, targetDir, distance, isPlayer))
        {
            hand.LookAt(target);
            Debug.Log("Shoot");
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
