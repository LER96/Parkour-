using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement1 : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;
    [Header("UI TEST delete later")]
    public TMP_Text speedText;

    [Header("Movement settings")]
    [SerializeField] Transform orientation;
    [SerializeField] float _moveSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float slideSpeed;
    [SerializeField] float wallRunSped;

    private float _desiredSpeed;
    private float _lastDesiredSpeed;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    [Header("Jump")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplayer;
    bool canJump = true;

    [Header("GroundCheck")]
    public float height;
    public LayerMask groundMask;
    public bool grounded = true;
    public float groundDrag;
    [SerializeField] float groundDist = 0.3f;
    [SerializeField] float currentGroundDist;
    [SerializeField] Transform groundCheck;


    [Header("SlopeHandling")]
    public float maxSlopAngle;
    private RaycastHit _slopHit;
    private bool _exitingSlop = false;


    [Header("Save")]
    Vector3 lastPos;

    [Header("Inputs")]
    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    [SerializeField] Rigidbody rb;

    public MovementState state;

    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air,
        sliding,
        wallRunning,
    }


    public bool sliding;
    public bool wallRunning;

    private void Start()
    {
        speedText.text = "Speed: " + _moveSpeed + ": " + state.ToString();
        rb.freezeRotation = true;
        canJump = true;
        lastPos = transform.position;
        //_startYSale = transform.localScale.y;
    }
    private void Update()
    {
        speedText.text = "Speed: " + (int)_moveSpeed + ": " + state.ToString();
        CheckGround();
        Inputs();
        SpeedControl();
        StateHandler();
        CheckDist();

        animator.SetBool("slide", sliding);

        //grounded = Physics.Raycast(groundCheck.position, Vector3.down, height, groundMask);
    }

    private void FixedUpdate()
    {
        PlayerMoving();
    }

    private void CheckGround()
    {
        grounded = Physics.CheckSphere(groundCheck.position, groundDist, groundMask);
        animator.SetBool("isground", grounded);
        if (grounded == true)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void StateHandler()
    {
        if (wallRunning)
        {
            state = MovementState.wallRunning;
            _desiredSpeed = wallRunSped;
        }
        else if (sliding)
        {
            state = MovementState.sliding;
            if (OnSlop() && rb.velocity.y < 0.1f)
            {
                _desiredSpeed = slideSpeed;
            }
            else
            {
                _desiredSpeed = sprintSpeed;
            }
        }

        else if (grounded && Input.GetKey(KeyCode.LeftShift))
        {
            state = MovementState.sprinting;
            _desiredSpeed = sprintSpeed;
        }
        else if (grounded)
        {
            state = MovementState.walking;
            _desiredSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
        }

        if (Mathf.Abs(_desiredSpeed - _lastDesiredSpeed) > 5f && _moveSpeed !=0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothMovementSpeed());
        }
        else
        {
            _moveSpeed = _desiredSpeed;
        }
        _lastDesiredSpeed = _desiredSpeed;
    }

    private IEnumerator SmoothMovementSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(_desiredSpeed - _moveSpeed);
        float startValue = _moveSpeed;

        while (time < difference)
        {
            _moveSpeed = Mathf.Lerp(startValue, _desiredSpeed, time / difference);
            if (OnSlop())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, _slopHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
                time += Time.deltaTime * speedIncreaseMultiplier;
            yield return null;
        }
        _moveSpeed = _desiredSpeed;
    }

    private void Inputs()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.Space) && canJump && grounded)
        {
            canJump = false;
            Jump();
            Invoke(nameof(JumpReset), jumpCooldown);
        }

        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    transform.localScale = new Vector3(transform.localScale.x, _startYSale, transform.localScale.z);
        //    rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        //}
        //if (Input.GetKeyUp(KeyCode.C))
        //{
        //    transform.localScale = new Vector3(transform.localScale.x, _startYSale, transform.localScale.z);

        //}
    }

    private void PlayerMoving()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        //animator.SetBool("run", grounded);

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
        if (Input.GetKey(KeyCode.LeftShift) && grounded)
        {
            _moveSpeed = Mathf.Lerp(_moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
        }
        else if (horizontalInput == 0 && verticalInput == 0)
        {
            _moveSpeed = 0;

        }
        else
        {
            _moveSpeed = Mathf.Lerp(_moveSpeed, walkSpeed, acceleration * Time.deltaTime);
        }
        animator.SetFloat("speed", _moveSpeed);
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
        if (Physics.Raycast(transform.position,Vector3.down, out _slopHit, height * 0.5f + 0.3f))
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

    private void CheckDist()
    {
        RaycastHit hit;
        if (Physics.Raycast(groundCheck.position, Vector3.down, out hit, groundMask))
        {
            currentGroundDist = hit.distance;
        }

        if (currentGroundDist > 0.5f)
        {
            animator.SetFloat("jump", currentGroundDist);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag=="Win")
        {
            Debug.Log("winner!");
        }
        if(other.transform.tag == "Death")
        {
            transform.position = lastPos;
            speedText.text = "You died";
        }
        if(other.transform.tag == "CheckPoint")
        {
            lastPos = other.transform.position;
        }
    }
}
