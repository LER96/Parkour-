using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] float speed;
    [SerializeField] float timeToDie = 4;
    float time;
    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.forward * speed*10;
        time = timeToDie;
    }

    private void Update()
    {
        if (timeToDie > 0)
        {
            timeToDie -= Time.deltaTime;
        }
        else
        {
            Destroy(this.gameObject);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player" || other.transform.tag == "Death" || other.transform.tag == "Ground")
        {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
}