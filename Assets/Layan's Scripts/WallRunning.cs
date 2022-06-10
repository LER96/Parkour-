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
    private float _wallRunTimer;

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

    [Header("GameReferences")]
    public Transform orientation;
    private PlayerMovement _movementScript;
    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _movementScript = GetComponent<PlayerMovement>();
    }

    public void CheckForWall()
    {
        _wallRight = Physics.Raycast(transform.position, orientation.right, out _rightWallHit, wallCheckDistance, whatIsWall);
        _wallLeft = Physics.Raycast(transform.position, -orientation.right, out _leftWallHit, wallCheckDistance, whatIsWall);
    }

    public bool AbovGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void WallRunningState()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if ((_wallLeft || _wallRight) && verticalInput > 0 && AbovGround())
        {

        }
    }

    private void startWallRun()
    {

    }

    private void wallRunningMovement()
    {

    }

    private void StopWallRun()
    {

    }
}
