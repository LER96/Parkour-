using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("WallRunning")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallRunForce;
    public float maxWallRunTime;
    public float wallJumpUpForce;
    public float wallJumpSideForce;

    [Header("Inputs")]
    private float horizontalInput;
    private float verticalInput;

    [Header("WallDetection")]
    public float wallCheckDistance;
    public float minJumpHeight;

    private RaycastHit _leftWallHit;
    private RaycastHit _rightWallHit;
    private bool _wallLeft;
    private bool _wallRight;

    [Header("Exiting Wall")]
    public float exitingWallTime;
    private bool _exitingWall=true;
    private float _exitingWallTimer;

    [Header("GameReferences")]
    public Transform orientation;
    [SerializeField] Rigidbody _rb;
    private PlayerMovement _movementScript;

    private void Start()
    {
        _movementScript = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        CheckForWall();
        WallRunningState();
    }

    private void FixedUpdate()
    {
        if (_movementScript.wallRunning)
        {
            WallRunningMovement();
        }
    }

    public void CheckForWall()
    {
        //using raycast we check the distance between the player, and the walls
        _wallRight = Physics.Raycast(transform.position, orientation.right, out _rightWallHit, wallCheckDistance, whatIsWall);
        _wallLeft = Physics.Raycast(transform.position, -orientation.right, out _leftWallHit, wallCheckDistance, whatIsWall);
    }

    public bool AbovGround()
    {
        //returns true if raycast isnt hitting any wall, to check when player isnt grounded and can wall run
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void WallRunningState()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if ((_wallLeft || _wallRight) && verticalInput > 0 && AbovGround() && !_exitingWall)
        {
            if(_wallLeft)
            {
                _movementScript.animator.SetBool("isLeft", _wallLeft);
                _movementScript.animator.SetBool("isRight", false);
            }
            else if(_wallRight)
            {
                _movementScript.animator.SetBool("isLeft", false);
                _movementScript.animator.SetBool("isRight", _wallRight);
            }

            if (!_movementScript.wallRunning)
            {
                StartWallRun();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                WallJump();
            }
        }

        else if (_exitingWall)
        {
            if (_movementScript.wallRunning)
            {
                StopWallRun();
            }

            if (_exitingWallTimer > 0)
            {
                _exitingWallTimer -= Time.deltaTime;
            }
            
            if (_exitingWallTimer <= 0)
            {
                _exitingWall = false;
            }
        }
        else
        {
            if (_movementScript.wallRunning)
            {
                StopWallRun();
            }
            _movementScript.animator.SetBool("isLeft", false);
            _movementScript.animator.SetBool("isRight", false);
        }
        
    }

    private void StartWallRun()
    {
        _movementScript.wallRunning = true;
    }

    private void WallRunningMovement()
    {
        _rb.useGravity = false;
        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

        //if wall is on the right, use rightwall hit, if not then use leftwall hit
        Vector3 wallNormal = _wallRight ? _rightWallHit.normal : _leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        //checks where playr is facing and does the wallrun in the same direction instead of going backwards
        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        _rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        //checks if player is behind a curved wall
        if (!(_wallLeft && horizontalInput > 0) && !(_wallRight && horizontalInput < 0))
        {
            _rb.AddForce(-wallNormal * 100, ForceMode.Force);
        }
    }

    private void StopWallRun()
    {
        _movementScript.wallRunning = false;

    }

    private void WallJump()
    {
        _exitingWall = true;
        _exitingWallTimer = exitingWallTime;

        Vector3 wallNormal = _wallRight ? _rightWallHit.normal : _leftWallHit.normal;
        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        _rb.AddForce(forceToApply, ForceMode.Impulse);
    }
}
