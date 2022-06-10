using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement settings")]
    private float _moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public Transform orientation;

    [Header("Jump")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplayer;
    bool canJump = true;

    public float wallRunSpeed;

    [Header("GroundCheck")]
    public float playerHeight;
    public LayerMask whatIsGrounded;
    bool grounded = true;
    public float groundDrag;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float _startYSale;

    [Header("SlopeHandling")]
    public float maxSlopAngle;
    private RaycastHit _slopHit;
    private bool _exitingSlop = false;

    [Header("Inputs")]
    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;

    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air,
        wallRunning,
    }
    public bool wallRunning;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        _startYSale = transform.localScale.y;
    }
    private void Update()
    {
        Inputs();
        SpeedControl();
        StateHandler();

        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f);
        
        if (grounded == true)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }

    }

    private void FixedUpdate()
    {
        PlayerMoving();
    }

    private void StateHandler()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            state = MovementState.crouching;
            _moveSpeed = crouchSpeed;
        }

        if (grounded && Input.GetKey(KeyCode.C))
        {
            state = MovementState.sprinting;
            _moveSpeed = sprintSpeed;
        }
        else if (grounded)
        {
            state = MovementState.walking;
            _moveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
        }
    }

    private void Inputs()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.Space) && canJump == true && grounded)
        {
            canJump = false;
            Jump();
            Invoke(nameof(JumpReset), jumpCooldown);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            transform.localScale = new Vector3(transform.localScale.x, _startYSale, transform.localScale.z);

        }
    }

    private void PlayerMoving()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (OnSlop() && !_exitingSlop)
        {
            rb.AddForce(SlopDirection(moveDirection) * _moveSpeed * 20f, ForceMode.Force);
            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }

        }

        else if (grounded == true)
        {
            rb.AddForce(moveDirection.normalized * _moveSpeed * 10f, ForceMode.Force);
        }
        else if (grounded == false)
        {
            rb.AddForce(moveDirection.normalized * _moveSpeed * 10f * airMultiplayer, ForceMode.Force);
        }

        rb.useGravity = !OnSlop();
    }

    public void SpeedControl()
    {
        if (OnSlop() && _exitingSlop == false)
        {
            if (rb.velocity.magnitude > _moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * _moveSpeed;
            }
        }
        else
        {
            Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVelocity.magnitude > _moveSpeed)
            {
                Vector3 limitVelocity = flatVelocity.normalized * _moveSpeed;
                rb.velocity = new Vector3(limitVelocity.x, rb.velocity.y, limitVelocity.z);
            }
        }
    }

    private void Jump()
    {
        _exitingSlop = true;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void JumpReset()
    {
        canJump = true;
        _exitingSlop = false;
    }

    public bool OnSlop()
    {
        if (Physics.Raycast(transform.position,Vector3.down, out _slopHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, _slopHit.normal);
            return angle < maxSlopAngle && angle!=0;
        }
        return false;
    }

    public Vector3 SlopDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, _slopHit.normal).normalized;
    }
}
