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
    private float _WallRunTimer;


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
    private bool _exitingWall;
    public float exitingWallTime;
    private float _exitingWallTimer;


    [Header("GameReferences")]
    public Transform orientation;
    private PlayerMovement _movementScript;
    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
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
            wallRunningMovement();
        }
    }

    public void CheckForWall()
    {
        _wallRight = Physics.Raycast(transform.position, orientation.right, out _rightWallHit, wallCheckDistance, whatIsWall);
        _wallLeft = Physics.Raycast(transform.position, -orientation.right, out _leftWallHit, wallCheckDistance, whatIsWall);

        Debug.Log($"right:{_wallRight}, left{_wallLeft}");
    }

    public bool AbovGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void WallRunningState()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if ((_wallLeft || _wallRight) && verticalInput > 0 && AbovGround() && !_exitingWall)
        {
            if (!_movementScript.wallRunning)
            {
                startWallRun();
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
        }
        
    }

    private void startWallRun()
    {
        _movementScript.wallRunning = true;
    }

    private void wallRunningMovement()
    {
        _rb.useGravity = false;
        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

        Vector3 wallNormal = _wallRight ? _rightWallHit.normal : _leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        _rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        if (!(_wallLeft && horizontalInput > 0) && !(_wallRight && horizontalInput < 0))
        {

        }
        _rb.AddForce(-wallNormal * 100, ForceMode.Force);
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
