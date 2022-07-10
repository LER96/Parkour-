using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement1 : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;

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
    bool isJumping;
    bool isLanding;
    float saveLastHeight;
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

    //storing the states the player is at 
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
        //to stop the player from falling over 
        rb.freezeRotation = true;
        canJump = true;
        lastPos = transform.position;
        //_startYSale = transform.localScale.y;
    }
    private void Update()
    {
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
        //wallrun
        if (wallRunning)
        {
            state = MovementState.wallRunning;
            _desiredSpeed = wallRunSped;
        }
        //slide
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
        //sprint - grounded and holding shift
        else if (grounded && Input.GetKey(KeyCode.LeftShift))
        {
            state = MovementState.sprinting;
            _desiredSpeed = sprintSpeed;
        }
        //walk - grounded only
        else if (grounded)
        {
            state = MovementState.walking;
            _desiredSpeed = walkSpeed;
        }
        //if not grounded, then in the air
        else
        {
            state = MovementState.air;
        }

        //if speed changed by allot, start coroutine for smooth transition
        if (Mathf.Abs(_desiredSpeed - _lastDesiredSpeed) > 5f && _moveSpeed !=0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothMovementSpeed());
        }
        //if not, change the speed instantely 
        else
        {
            _moveSpeed = _desiredSpeed;
        }
        _lastDesiredSpeed = _desiredSpeed;
    }

    //changing the movement speed to desired movement speed over time
    //when speed increases, it will smoothly slowdown to the original speed
    private IEnumerator SmoothMovementSpeed()
    {
        float time = 0;
        //checks the difference between the speed we currently got and our movespeed
        float difference = Mathf.Abs(_desiredSpeed - _moveSpeed);
        float startValue = _moveSpeed;

        //when time is smaller than our difference float
        while (time < difference)
        {
            //change the movespeed
            _moveSpeed = Mathf.Lerp(startValue, _desiredSpeed, time / difference);
            if (OnSlop())
            {
                //when on slope, change the angle
                float slopeAngle = Vector3.Angle(Vector3.up, _slopHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);
                //increasing the speed with time while sliding on a slope
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
        //player movement
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        //checking when the player can jump
        if (Input.GetKey(KeyCode.Space) && canJump && grounded)
        {
            canJump = false;
            Jump();
            //lets you always jump as long as you have the space bar pressed
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
        //calculating the movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        //animator.SetBool("run", grounded);

        //if on slope, increase speed to fit with sprint
        if (OnSlop() && !_exitingSlop) 
        {
            rb.AddForce(SlopDirection(moveDirection) * _moveSpeed * 20f, ForceMode.Force);
            //if player is moving up, add force to not bounce
            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }
        else if (grounded == true) //if grounded, move normally
        {
            //increasing speed
            rb.AddForce(moveDirection.normalized * _moveSpeed * 10f, ForceMode.Force);
        }
        else if(grounded==false) //if in air, multiply with airMultiplayer so player can move while in the air
        {
            rb.AddForce(moveDirection.normalized * _moveSpeed * 10f * airMultiplayer, ForceMode.Force);
        }
        //turn gravity off if on slope to not slide backwards
        rb.useGravity = !OnSlop();
    }

    public void SpeedControl()
    {
        //limit the speed on slope. when going on slop, you travel further than on normal ground
        //we limit the speed so the player travels same amount
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
            //limit the velocity when needed
            if (flatVelocity.magnitude > _moveSpeed)
            {
                //if you go faster than the movement speed, you calculate the maxed velocity then apply it
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
        //lets you jump on a slope
        _exitingSlop = true;
        //resetting y velocity to always jump the same height
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        //jump pulse 
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void JumpReset()
    {
        //resetting when you can jump
        canJump = true;
        _exitingSlop = false;
    }

    public bool OnSlop()
    {
        //checks if you are on a slope
        if (Physics.Raycast(transform.position,Vector3.down, out _slopHit, height * 0.5f + 0.3f))
        {
            //calculating how steep the slope is
            float angle = Vector3.Angle(Vector3.up, _slopHit.normal);
            //checks if the angle is smaller than the max slope
            return angle < maxSlopAngle && angle!=0;
        }
        return false;
    }

    //finding correct direction of the slope
    public Vector3 SlopDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, _slopHit.normal).normalized;
    }

    private void CheckDist()
    {
        animator.SetBool("isLanding", isLanding);
        animator.SetBool("isJumping", isJumping);
        RaycastHit hit;
        if (Physics.Raycast(groundCheck.position, Vector3.down, out hit, groundMask))
        {
            currentGroundDist = hit.distance;
        }
        if (saveLastHeight < currentGroundDist && grounded==false)
        {
            saveLastHeight = currentGroundDist;
            isJumping = true;
            isLanding = false;
        }
        else if(saveLastHeight >= currentGroundDist && grounded==false)
        {
            currentGroundDist = saveLastHeight;
            isLanding=true;
            isJumping = false;
        }
        else if(grounded)
        {
            isLanding = false;
            isJumping = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag=="Win")
        {
            Debug.Log("winner!");
        }
        if(other.transform.tag == "Death" || other.transform.tag == "Bullet")
        {
            transform.position = lastPos;
        }
        if(other.transform.tag == "CheckPoint")
        {
            lastPos = other.transform.position;
        }

    }
}
