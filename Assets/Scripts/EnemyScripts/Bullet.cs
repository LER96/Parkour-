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
        //set bullet speed
        rb.velocity = transform.forward * speed*10;
        time = timeToDie;
    }

    private void Update()
    {
        //timer/ if the timer is up the object is being destroy
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
        // check the trigger of the bullet
        if (other.transform.tag == "Player" || other.transform.tag == "Ground" || other.transform.tag == "Death")
        {
            Destroy(this.gameObject); 
        }
    }

    // Update is called once per frame
}
