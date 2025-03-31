using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Movement Data")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private int maxJumpAmount = 2; // means player can do double jump


    [Header("Ground Collision Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Color groundedColor = Color.green;
    [SerializeField] private Color notGroundedColor = Color.red;

    private float horizontalDirection;
    private bool isFacingRight = true;
    private bool isWalking;
    private bool isGrounded;
    private bool canJump;
    private int currentJumpAmount;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        currentJumpAmount = maxJumpAmount;
    }


    private void Update()
    {
        CheckInput();
        CheckMovementDirection();
        CheckIfCanJump();
        AnimationController();
    }


    private void FixedUpdate()
    {
        HandleMovement();
        CheckSurroundings();
    }

    private void CheckInput()
    {
        horizontalDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
    }

    private void CheckIfCanJump()
    {
        if (isGrounded && rb.velocity.y < 0.01f)
        {
            currentJumpAmount = maxJumpAmount;
        }
        
        if(currentJumpAmount <= 0)
        {
            canJump = false;
        }
        else
        {
            canJump = true;
        }

    }

    private void AnimationController()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
    }

    private void HandleMovement()
    {
        rb.velocity = new Vector2(horizontalDirection * movementSpeed, rb.velocity.y);
    }

    private void CheckMovementDirection()
    {
        if (isFacingRight && horizontalDirection < 0)
        {
            Flip();
        }
        else if (!isFacingRight && horizontalDirection > 0)
        {
            Flip();
        }

        if (Mathf.Abs(rb.velocity.x) > 0)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }

    private void Jump()
    {
        if (canJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            currentJumpAmount--;  
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    private void OnDrawGizmos()
    {
        if (isGrounded)
        {
            Gizmos.color = groundedColor;
        }
        else
        {
            Gizmos.color = notGroundedColor;
        }

        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }
}
