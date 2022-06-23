using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    //[SerializeField] Transform centerbody;
    //[SerializeField] Rigidbody rb;
    //[SerializeField] float wallPull = 10;

    //[Header("Wall/ Run")]
    //[SerializeField] float walldistance=0.5f;
    ////[SerializeField] float radius;
    //[SerializeField] float minimumJumpHeight= 1.5f;
    //[SerializeField] float wallJumpForce = 10;

    //[SerializeField] LayerMask maskWall;
    //[SerializeField] LayerMask maskGround;
    //RaycastHit righthitWall;
    //RaycastHit lefthitWall;
    //Ray ray;
    //public bool wallRight;
    //public bool wallLeft;
    //public float radius;


    ////bool CanRunWall()
    ////{
    ////    return !Physics.Raycast(transform.position, Vector3.one, minimumJumpHeight);
    ////}

    //void CheckWall()
    //{
    //    wallRight = Physics.Raycast(transform.position, centerbody.right, out righthitWall, walldistance, maskWall);
    //    wallLeft = Physics.Raycast(transform.position, -centerbody.right, out lefthitWall, walldistance, maskWall);

    //    Debug.Log($"right:{wallRight}, left{wallLeft}");
    //    //ray = new Ray(centerbody.position, Vector3.one);
    //    //if (Physics.SphereCast(ray, radius,  walldistance, mask))
    //    //{
    //    //    Debug.DrawLine(transform.position, hitWall.point, Color.red);
    //    //    isWall = true;
    //    //}
    //    //else
    //    //{
    //    //    Debug.DrawLine(ray.origin, ray.origin + ray.direction * walldistance, Color.green);
    //    //    isWall = false;
    //    //}
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    CheckWall();
    //    WallRunningState();
    //    //if (isWall)
    //    //{
    //    //    StartWallRun();
    //    //}
    //    //else
    //    //{
    //    //    StopWallRun();
    //    //}

    //}

    //private void WallRunningState()
    //{
    //    var horizontalInput = Input.GetAxis("Horizontal");
    //    var verticalInput = Input.GetAxis("Vertical");

    //    if ((wallLeft || wallRight) && verticalInput > 0 && AbovGround() && !_exitingWall)
    //    {
            
    //         StartWallRun();
            

    //        if (Input.GetKeyDown(KeyCode.Space))
    //        {
    //            WallJump();
    //        }
    //    }

    //    else if (_exitingWall)
    //    {
    //        StopWallRun();
    //    }
    //    else
    //    {
    //        StopWallRun();         
    //    }

    //}

    //void StartWallRun()
    //{
    //    rb.useGravity = false;
    //    rb.AddForce(Vector3.down * wallPull, ForceMode.Force);

    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
            
    //            Vector3 wallJumpDir = transform.up + hitWall.normal;
    //            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
    //            rb.AddForce(wallJumpDir * wallJumpForce * 100, ForceMode.Force);
            
    //    }

        
    //}
    //void StopWallRun()
    //{
    //    rb.useGravity = true;
    //}
}
