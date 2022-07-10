using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning1 : MonoBehaviour
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
    private bool _exitingWall=true;
    public float exitingWallTime;
    private float _exitingWallTimer;


    [Header("GameReferences")]
    public Transform orientation;
    private PlayerMovement1 _movementScript;
    [SerializeField] Rigidbody _rb;

    private void Start()
    {
        //_rb = GetComponent<Rigidbody>();
        _movementScript = GetComponent<PlayerMovement1>();
    }

    private void Update()
    {
        CheckForWall();
        WallRunningState();
    }

    private void FixedUpdate()
    {
        //if player state is wallrunning, we call wallrun function
        if (_movementScript.wallRunning)
        {
            wallRunningMovement();
        }
    }

    public void CheckForWall()
    {
        //using raycast we check the distance between the player, and the walls
        _wallRight = Physics.Raycast(transform.position, orientation.right, out _rightWallHit, wallCheckDistance, whatIsWall);
        _wallLeft = Physics.Raycast(transform.position, -orientation.right, out _leftWallHit, wallCheckDistance, whatIsWall);

        //Debug.Log($"right:{_wallRight}, left{_wallLeft}");
    }

    public bool AbovGround()
    {
        //returns true if raycast isnt hitting any wall, to check when player isnt grounded and can wall run
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void WallRunningState()
    {
        //inputs
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        //when player can wall run: if wall left or wall right is true, w is pressed and is above ground
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

            //if you werent wallrunning before, you can wallrun
            if (!_movementScript.wallRunning)
            {
                startWallRun();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                WallJump();
            }
        }

        //if out of wall
        else if (_exitingWall)
        {
            //if you were wallrunning, stop wall run
            if (_movementScript.wallRunning)
            {
                StopWallRun();
            }

            //if timer is above 0, countdown
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

    private void startWallRun()
    {
        _movementScript.wallRunning = true;
    }

    private void wallRunningMovement()
    {
        //setting gravity off and velocity to 0
        _rb.useGravity = false;
        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

        //if wall is on the right, use rightwall hit, if not then use leftwall hit
        Vector3 wallNormal = _wallRight ? _rightWallHit.normal : _leftWallHit.normal;
        //moves upwards
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        //checks where playr is facing and does the wallrun in the same direction instead of going backwards
        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        //adding force forward for wallrun
        _rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        //checks if player is behind a curved wall
        if (!(_wallLeft && horizontalInput > 0) && !(_wallRight && horizontalInput < 0))
        {
            //pushes player to the wall so they can run on a curved wall from behind 
            _rb.AddForce(-wallNormal * 100, ForceMode.Force);
        }
    }

    private void StopWallRun()
    {
        _movementScript.wallRunning = false;

    }

    //
    private void WallJump()
    {
        _exitingWall = true;
        _exitingWallTimer = exitingWallTime;

        //if wall is on the right, use rightwall hit, if not then use leftwall hit
        Vector3 wallNormal = _wallRight ? _rightWallHit.normal : _leftWallHit.normal;
        //how much force you have while wall jumping
        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        // resetting velocity and walljump force
        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        _rb.AddForce(forceToApply, ForceMode.Impulse);
    }
}
