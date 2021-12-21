using UnityEngine;

public class PlayerMov : MonoBehaviour 
{
    private float movementInputDirection;

    private int amountOfJumpLeft;
    private int facingDirection = 1;

    private bool isFacingRight = true;
    private bool isWalking;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool canJump;

    private Rigidbody2D body;
    private Animator anim;

    public int amountOfJumps = 2;
    
    public float mov_speed = 5.0f;
    public float jumpForce = 10.0f;
    public float groundCheckRadius;
    public float wallCheckDistance;
    public float wallSlideSpeed;
    public float movementForceInAir;
    public float airDragMultipier = 0.95f;
    public float variableJumpHeightMultipier = 0.1f;

    public Transform groundCheck;
    public Transform wallCheck;

    public LayerMask whatIsGround;
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        amountOfJumpLeft = amountOfJumps;
    }

    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        UpdateAnimations();
        CheckIfCanJump();
    }
    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurroundings();
    }

    private void CheckIfWallSliding()
    {
        if (isTouchingWall && !isGrounded && body.velocity.y < 0)
        {
            isWallSliding = true;
        } else
        {
            isWallSliding = false;
        }
    }
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
    }

    private void CheckIfCanJump()
    {
        if(isGrounded && body.velocity.y <= 0)
        {
            amountOfJumpLeft = amountOfJumps;
        }
            if (amountOfJumpLeft <= 0)
            {
                canJump = false;
            } else
            {
                canJump = true;
            }
        }
    private void CheckMovementDirection()
    {
        if(isFacingRight && movementInputDirection < 0)
        {
            Flip();
        } else if (!isFacingRight && movementInputDirection > 0)
        {
            Flip();
        }
        if(Mathf.Abs(body.velocity.x) >=0.01f)
        {
            isWalking = true;
        } else
        {
            isWalking = false;
        }
    }

    private void UpdateAnimations()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", body.velocity.y);
    }

    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump")) {
            Jump();
        }
        if (Input.GetButtonUp("Jump"))
        {
            body.velocity = new Vector2(body.velocity.x, body.velocity.y * variableJumpHeightMultipier);
        }
    }
    
    private void Jump()
    {
        if (canJump)
        {
            body.velocity = new Vector2(body.velocity.x, jumpForce);
            amountOfJumpLeft--;
        }
    }

    private void ApplyMovement()
    {
        if(isGrounded)
        {
            body.velocity = new Vector2(mov_speed * movementInputDirection, body.velocity.y);
        }
        else if (!isGrounded && !isWallSliding && movementForceInAir !=0)
        {
            Vector2 forceToAdd = new Vector2(movementForceInAir * movementInputDirection, 0);
            body.AddForce(forceToAdd);

            if(Mathf.Abs(body.velocity.x) > mov_speed)
            {
                body.velocity = new Vector2(mov_speed * movementInputDirection, body.velocity.y);
            }
        } else if (!isGrounded && !isWallSliding && movementInputDirection == 0 )
        {
            body.velocity = new Vector2(body.velocity.x * airDragMultipier, body.velocity.y);
        }

        if (isWallSliding)
        {
            if(body.velocity.y < -wallSlideSpeed)
            {
                body.velocity = new Vector2(body.velocity.x, -wallSlideSpeed);
            }
        }
    }

    /*
    public void DisableFlip()
    {
        canFlip = false;
    }

    public void EnableFlip()
    {
        canFlip = true;
    }
    
    */

    public int GetFacingDirection()
    {
        return facingDirection;
    }

    private void Flip()
    {
        if (!isWallSliding)
        {
            facingDirection *= -1;
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }
}
