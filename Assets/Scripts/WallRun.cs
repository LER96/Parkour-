using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    [SerializeField] Transform orintation;
    [SerializeField] Rigidbody rb;
    [SerializeField] float wallPull = 10;

    [Header("Wall/ Run")]
    [SerializeField] float walldistance=0.5f;
    [SerializeField] float minimumJumpHeight= 1.5f;
    [SerializeField] float wallJumpForce = 10;

    bool wallLeft = false;
    bool wallRight = false;

    RaycastHit leftHit;
    RaycastHit rightHit;


    bool CanRunWall()
    {
        return !Physics.Raycast(transform.position, Vector3.down,minimumJumpHeight);
    }

    void CheckWall()
    {
        wallLeft = Physics.Raycast(transform.position, -orintation.right, out leftHit ,walldistance);
        wallRight = Physics.Raycast(transform.position, orintation.right, out rightHit ,walldistance);
    }

    // Update is called once per frame
    void Update()
    {
        CheckWall();
        if (CanRunWall())
        {
            if (wallLeft)
            {
                StartWallRun();
                Debug.Log("wallleft= " + wallLeft);
            }
            else if(wallRight)
            {
                StartWallRun();
                Debug.Log("wallright= " + wallRight);
            }
            else
            {
                StopWallRun();
            }
        }

    }

    void StartWallRun()
    {
        rb.useGravity = false;
        rb.AddForce(Vector3.down * wallPull, ForceMode.Force);

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(wallLeft)
            {
                Vector3 wallJumpDir = transform.up + leftHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallJumpDir * wallJumpForce*100, ForceMode.Force);
            }
            if(wallRight)
            {
                Vector3 wallJumpDir = transform.up + rightHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallJumpDir * wallJumpForce * 100, ForceMode.Force);
            }
        }
    }

    void StopWallRun()
    {
        rb.useGravity = true;
    }
}
