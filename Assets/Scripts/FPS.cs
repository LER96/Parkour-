using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS : MonoBehaviour
{

    public CharacterController controller;
    public Animator animator;
    [SerializeField] float speed = 10;
    [SerializeField] float jump;
    [SerializeField] float gravity;

    Vector3 moveDir;
    Vector3 velocity;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDist = 0.3f;
    public LayerMask groundMask;
    public LayerMask WallMask;
    public bool isGrounded;
    public bool isWall;

    private void Update()
    {
        CheckGround();
        MyInput();
    }
    void Jump(float magnitude)
    {
        velocity.y += Mathf.Sqrt(magnitude);
    }
    void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDist, groundMask);
        isWall = Physics.CheckBox(transform.position, new Vector3(1.2f, 0, 1.2f), Quaternion.identity, WallMask);
    }
    void MyInput()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if(x!=0 || z!=0)
        {
            animator.SetInteger("run", 1);
        }
        else if(x==0 && z==0)
        {
            animator.SetInteger("run", 0);
        }
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);
        if (moveDir.magnitude >= 1f)
        {
            moveDir.Normalize();
        }
        controller.Move(moveDir * speed * Time.deltaTime);


        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump(jump * -3f * gravity);
        }
        if(isWall)
        {
            velocity.y = 0;
        }
        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        controller.Move(velocity * Time.deltaTime);
    }

}
