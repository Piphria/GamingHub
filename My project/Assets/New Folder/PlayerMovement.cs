using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement")]

    public float playerHeight = 2f;
    public float moveSpeed = 6f;
    public float moveMultiplier = 10f;
    public float airMoveMultiplier = 0.5f;
    public float jumpForce = 5f;
    public float grappleJumpForce = 5f;
    public float maxYSpeed = 15f;
    float horizontalMovement;
    float verticalMovement;
    public float groundDrag = 5f;
    public float airDrag = 2f;
    public float gravity = 80f;
    public ForceMode forceMode;
    private Grappling gp;

    [Header("Ground Detection")]
    [SerializeField] LayerMask groundMask;
    public float groundDistance = 0.4f;
    public bool isGrounded;
    KeyCode jumpKey = KeyCode.Space;
    [SerializeField] Transform orientation;
    
    public bool canGPJump;
    bool canJump;
    public bool dashing;
    public bool freeze;
    public bool activeGrapple;
    private bool enableMovementOnNextTouch;
    Vector3 moveDirection;

    Rigidbody rb;
    

    RaycastHit slopeHit;
    Vector3 slopeMoveDirection;

    private bool onSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight/2 + 0.5f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;                   
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        gp = GetComponent<Grappling>();

    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, 1, 0), groundDistance, groundMask);

        MyInput();
        ControlDrag();

        

        if (Input.GetKeyDown(jumpKey) && !gp.IsGrappling())
        {
            if (canJump && !isGrounded)
            {
                Jump();
                canJump = false;
            }
            else if (isGrounded)
            {
                Jump();
                canJump = true;
            }
            if (!gp.canGrapple && canGPJump)
            {
                GrappleJump();
                canGPJump = false;

            }


        }
        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
        if (isGrounded)
        {
            canJump = true;
        }

        if (freeze)
        {
            rb.velocity = Vector3.zero;
        }
       
    }

    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;
        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);
        Invoke(nameof(ResetRestrictions), 2f);
    }

    private Vector3 velocityToSet;
    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        rb.velocity = velocityToSet;
    }

    void Jump()
    {
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    void GrappleJump()
    {
        rb.AddForce(transform.up * jumpForce * grappleJumpForce, ForceMode.Impulse);

    }
    void ControlDrag()
    {
        if(isGrounded && !activeGrapple)
        {
            rb.drag = groundDrag;
        }
        else if (!activeGrapple)
        {
            rb.drag = airDrag;
        }
        else
        {
            rb.drag = 0;
        }

    }

    public void ResetRestrictions()
    {
        activeGrapple = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestrictions();

            GetComponent<Grappling>().StopGrapple();
        }

    }
    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
    }

    private void FixedUpdate()
    {
        MovePlayer();
        rb.AddForce(Vector3.down * gravity, forceMode);
    }

    void MovePlayer()
    {
        if (!activeGrapple)
        {
            if (isGrounded && !onSlope())
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * moveMultiplier, ForceMode.Acceleration);
            }
            else if (isGrounded && onSlope())
            {
                rb.AddForce(slopeMoveDirection.normalized * moveSpeed * moveMultiplier, ForceMode.Acceleration);
            }
            else if (!isGrounded)
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * moveMultiplier * airMoveMultiplier, ForceMode.Acceleration);
            }
        }

        
    }
    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = -80f;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }
}
