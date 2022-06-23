using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFps : MonoBehaviour
{
    float horizontal;
    float vertical;
    Vector3 moveDir;
    Vector3 slopeDir;

    [SerializeField] WallRun wallrun;
    [SerializeField] Animator animator;

    [Header("Movement")]
    public float speed=5;
    public float moveMultiplier = 10;

    [Header("MoveMode")]
    [SerializeField] float walkSpeed = 4;
    [SerializeField] float sprintSpeed = 6;
    [SerializeField] float acceleration = 10;

    [Header("Drag Control")]
    public float groundDrag = 5;
    public float airDrag = 1.5f;

    [Header("Jump")]
    //how strong is the jump
    public float jumpForce = 5;
    //how storng is the push
    public float airMultiplier = 0.4f;
    public Rigidbody rb;

    [Header ("Ground Check")]
    public Transform groundCheck;
    public float groundDist = 0.3f;
    [SerializeField] float currentGroundDist;
    //Ground Layer
    public LayerMask groundMask;
    public bool isGrounded;

    [Header("On slope")]
    [SerializeField] float maxAngle;
    RaycastHit slopeHit;
    private void Start()
    {
        rb.freezeRotation = true;
    }
    private void Update()
    {
        slopeDir = Vector3.ProjectOnPlane(moveDir, slopeHit.normal);

        if(isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        MyInput();
        CheckDist();
        ControlDrag();
        ControlSpeed();
        
        CheckGround();
        
    }

    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDist, groundMask);
    }

    private void CheckDist()
    {
        RaycastHit hit;
        if (Physics.Raycast(groundCheck.position, Vector3.down, out hit, groundMask))
        {
            currentGroundDist = hit.distance;
        }

        if (currentGroundDist > 1)
        {
            animator.SetFloat("jump", currentGroundDist);
        }
    }

    private void ControlSpeed()
    {
        if (Input.GetKey(KeyCode.LeftShift) && isGrounded)
        {
            speed = Mathf.Lerp(speed, sprintSpeed, acceleration * Time.deltaTime);          
        }
        else if (horizontal == 0 && vertical==0)
        {
            speed = 0;
       
        }
        else
        {
            speed = Mathf.Lerp(speed, walkSpeed, acceleration * Time.deltaTime);
        }
        animator.SetFloat("speed", speed);
    }

    bool OnSlope()
    {
        if (Physics.Raycast(groundCheck.position, Vector3.down, out slopeHit, groundMask))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxAngle && angle != 0;
        }
        return false;
    }
    private void Jump()
    {
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ControlDrag()
    {
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
    }

    private void MyInput()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        moveDir = transform.forward * vertical + transform.right * horizontal;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        if (isGrounded && ! OnSlope())
        {
            rb.AddForce(moveDir.normalized * speed * moveMultiplier, ForceMode.Acceleration);
        }
        else if(isGrounded && OnSlope())
        {
            rb.AddForce(slopeDir.normalized * speed * moveMultiplier, ForceMode.Acceleration);
        }
        else if(!isGrounded)
        {
            rb.AddForce(moveDir.normalized * speed * moveMultiplier* airMultiplier, ForceMode.Acceleration);
        }

    }
}
