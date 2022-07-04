using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] float speed;

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.forward * speed*10;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag=="Death")
        {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
}
