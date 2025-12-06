using System;
using UnityEngine;

public class PLayer1 : MonoBehaviour

{

    private Rigidbody2D rb;

    private Animator anim;
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;

    [Header("DoubleJump")]
    [SerializeField] private float doubleJumpForce;
    private bool canDoublejump;


    [Header("Collision Info")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    private bool isGrounded;
    private bool isAirborne;
    private bool isWallDetected;

    private float xInput;
    private bool facingRight = true;
    private int facingDir = 1;

    public bool IsRunning;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        UpdateAirborneStatus();

        HandleCollision();
        HandleInput();
        HandleWallSlide();
        HandleMovement();
        HandleFlip();
        HandleAnimations();

    }

    private void HandleWallSlide()
    {
        if(isWallDetected && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }

    }

    private void UpdateAirborneStatus()
    {
        if (isAirborne && isGrounded)
            HandleLanding();

        if (!isAirborne && !isGrounded)
            BecomeAirborne();
    }

    private void BecomeAirborne()
    {
        isAirborne = true;
    }

    private void HandleLanding()
    {
        isAirborne = false;
        canDoublejump = true;
    }

    private void HandleCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
    }

    private void HandleAnimations()
    {
        anim.SetFloat("xVelocity", rb.linearVelocity.x);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        anim.SetBool("isGrounded", isGrounded);
       //nim.SetBool("IsRunning", IsRunning);
        //Running = rb.linearVelocity.x != 0;
    }

    private void HandleMovement()
    {
        rb.linearVelocity = new Vector2(xInput * moveSpeed, rb.linearVelocity.y);
    }
    private void HandleInput()
    {

        xInput = Input.GetAxisRaw("Horizontal");
        //rb.linearVelocity = new Vector2(xInput * moveSpeed, rb.linearVelocity.y);



        if (Input.GetKeyDown(KeyCode.Space))
            JumpButton();
    }

    private void JumpButton()
    {
        if (isGrounded)
        {
            Jump();
        }
            
        else if (canDoublejump)
        {
            DoubleJump();
        }
    }

    private void Jump() => rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

    private void DoubleJump()
    {
        canDoublejump = false;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, doubleJumpForce);
    }

    private void HandleFlip()
    {
        if(rb.linearVelocity.x < 0 && facingRight || rb.linearVelocity.x > 0 && !facingRight)
        {
            Flip();
           
        }

    }

    private void Flip()
    {
        facingDir = facingDir * -1;
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (wallCheckDistance * facingDir), transform.position.y));
    }
}
