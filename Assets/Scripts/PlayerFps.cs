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

    [Header("Movement")]
    public float speed=5;
    public float moveMultiplier = 10;

    [Header("Drag Control")]
    public float groundDrag = 5;
    public float airDrag = 1.5f;

    [Header("Jump")]
    public float jumpForce = 5;
    public float airMultiplier = 0.4f;
    public Rigidbody rb;

    [Header ("Ground Check")]
    public Transform groundCheck;
    public float groundDist = 0.3f;
    public LayerMask groundMask;
    public bool isGrounded;

    RaycastHit slopeHit;
    private void Start()
    {
        rb.freezeRotation = true;
    }
    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDist, groundMask);
        if(isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        MyInput();
        ControlDrag();

        slopeDir = Vector3.ProjectOnPlane(moveDir, slopeHit.normal);
    }

    bool OnSlope()
    {
        if (Physics.Raycast(groundCheck.position, Vector3.down, out slopeHit, groundMask))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
                return false;
        }
        else
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
