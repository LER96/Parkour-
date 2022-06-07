using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFps : MonoBehaviour
{
    float horizontal;
    float vertical;
    Vector3 moveDir;
    public float speed=5;
    public float drag = 5;
    public float jump = 5;
    public Rigidbody rb;

    public Transform groundCheck;
    public float groundDist = 0.3f;
    public LayerMask groundMask;
    public bool isGrounded;

    private void Start()
    {
        rb.freezeRotation = true;
    }
    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDist, groundMask);
        if(isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(transform.up * jump, ForceMode.Impulse);
        }
        MyInput();
        ControlDrag();
    }

    private void ControlDrag()
    {
        rb.drag = drag;
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
        rb.AddForce(moveDir.normalized * speed, ForceMode.Acceleration);
    }
}
