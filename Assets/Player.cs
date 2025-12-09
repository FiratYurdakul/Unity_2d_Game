using System;
using System.Collections;
using UnityEngine;
using UnityEngine.WSA;

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

    [Header("Buffer & Coyote Jump")]
    [SerializeField] private float bufferJumpWindow = .25f;
    private float bufferJumpPressed = -1;
    [SerializeField] private float coyoteJumpWindow = .5f;
    private float coyoteJumpActivated = -1;

    [Header("Wall interactions")]
    [SerializeField] private float wallJumpDuration = .6f;
    [SerializeField] private Vector2 wallJumpForce;
    private bool isWallJumping;


    [Header("KnockedBack")]
    [SerializeField] private float knockbackDuration = 1;
    [SerializeField] private Vector2 knockbackPower;
    private bool isknocked;
    private bool canBeKnocked;


    [Header("Collision Info")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    private bool isGrounded;
    private bool isAirborne;
    private bool isWallDetected;

    private float xInput;
    private float yInput;
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


        if (isknocked)
            return;

        HandleInput();
        HandleWallSlide();
        HandleMovement();
        HandleFlip();
        HandleCollision();
        HandleAnimations();

    }


    public void Knockback()
    {
        if (isknocked)
        {
            return;
        }

        StartCoroutine(KnockBackRoutine());

        isknocked = true;
        anim.SetTrigger("knockedback");

        rb.linearVelocity = new Vector2(knockbackPower.x * -facingDir, knockbackPower.y);

    }

    private void  HandleWallSlide()
    {
        bool canWallSlide = isWallDetected && rb.linearVelocity.y < 0;
        float yModifier = .05f;

        if (canWallSlide == false)
        {
            return;
        }
        if (yInput<0)
            yModifier = 1;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * yModifier);
      

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
        if (rb.linearVelocity.y < 0)
              ActivateCoyoteJump();
    }

    private void HandleLanding()
    {
        isAirborne = false;
        canDoublejump = true;
        AttemptBufferJump();
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
        anim.SetBool("isWallDetected", isWallDetected);
       //nim.SetBool("IsRunning", IsRunning);
        //Running = rb.linearVelocity.x != 0;
    }

    private void HandleMovement()
    {
        if (isWallDetected)
        {
            return;
        }

        if (isWallJumping) 
            return;

        rb.linearVelocity = new Vector2(xInput * moveSpeed, rb.linearVelocity.y);
    }
    private void HandleInput()
    {

        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");
        //rb.linearVelocity = new Vector2(xInput * moveSpeed, rb.linearVelocity.y);



        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpButton();
            RequestBufferJump();
        }
    }

    private void RequestBufferJump()
    {
        if (isAirborne)
            bufferJumpPressed = Time.time;
    }

    private void AttemptBufferJump()
    {
        if (Time.time < bufferJumpPressed + bufferJumpWindow)
        {
            bufferJumpPressed = 0;
            Jump();
        }
    }

    private void ActivateCoyoteJump() => coyoteJumpActivated = Time.time;
    private void CancelCoyoteJump() => coyoteJumpActivated = Time.time -1;


    private void JumpButton()
    {

        bool coyoteJumpAvalibel = Time.time < coyoteJumpActivated + coyoteJumpWindow;
        if (isGrounded || coyoteJumpAvalibel)
        {
            Jump();
        }
        else if (isWallDetected && !isGrounded)
        {
            WallJump(); 
        }
        else if (canDoublejump)
        {
            DoubleJump();

            CancelCoyoteJump(); 
        }
    }

    private void Jump() => rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

    private void DoubleJump()
    {
        isWallJumping = false;
        canDoublejump = false;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, doubleJumpForce);
    }

    private void WallJump()
    {
        canDoublejump = true;
        rb.linearVelocity = new Vector2(wallJumpForce.x * -facingDir, wallJumpForce.y);
        Flip();

        //StopAllCoroutines();    
        StartCoroutine(WallJumpRoutine());
    }

    private IEnumerator WallJumpRoutine()
    {
        isWallJumping = true;

        yield return new WaitForSeconds(wallJumpDuration);
        isWallJumping = false;
    }


    private IEnumerator KnockBackRoutine()
    {
        canBeKnocked = false;   
        isknocked = true;

        yield return new WaitForSeconds(knockbackDuration);
        canBeKnocked = true;
        isknocked = false;  
    }

    private void HandleFlip()
    {
        if(xInput < 0 && facingRight || xInput > 0 && !facingRight)
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
