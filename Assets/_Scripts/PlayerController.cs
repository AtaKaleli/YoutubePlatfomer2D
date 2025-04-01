using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Movement Data")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private int maxJumpAmount = 2; // means player can do double jump
    [SerializeField] private float wallSlideSpeedDivider = 0.1f;
    [SerializeField] private float variableJumpDivider = 0.5f;

    [Header("Ground Collision Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Color detectedColor = Color.green;
    [SerializeField] private Color notDetectedColor = Color.red;

    [Header("Wall Collision Check")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckRadius;


    private float horizontalDirection;
    private bool isFacingRight = true;
    private bool isWalking;
    private bool isGrounded;
    private bool canJump;
    private int currentJumpAmount;
    private bool isWallDetected;
    private bool isWallSliding;


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
        CheckIfWallSliding();
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
        if (Input.GetKeyUp(KeyCode.Space))
        {
            CancelJump();
        }
    }

    

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        isWallDetected = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckRadius, groundLayer);
    }


    private void AnimationController()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isWallSliding", isWallSliding);
        anim.SetFloat("yVelocity", rb.velocity.y);
    }

    #region Movement & Jump
    private void HandleMovement()
    {

        if (isWallSliding)
        {
            rb.velocity = new Vector2(0, rb.velocity.y * wallSlideSpeedDivider);
        }


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

        isWalking = Mathf.Abs(horizontalDirection) > 0;
    }

    private void Flip()
    {
        if (isWallSliding)
            return;

        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    private void CheckIfCanJump()
    {
        if (isGrounded && rb.velocity.y < 0.01f)
        {
            currentJumpAmount = maxJumpAmount;
        }

        canJump = currentJumpAmount > 0;

    }

    private void Jump()
    {
        if (canJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            currentJumpAmount--;
        }
    }

    private void CancelJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpDivider);
    }

    #endregion

    private void CheckIfWallSliding()
    {
        isWallSliding = isWallDetected && !isGrounded && rb.velocity.y < 0.01f;
    }

    #region Gizmos Drawings
    private void OnDrawGizmos()
    {
        //GroundCheckColorChange();
        //WallCheckColorChange();

        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + (horizontalDirection * wallCheckRadius), wallCheck.position.y));
    }

    private void GroundCheckColorChange()
    {
        if (isGrounded)
        {
            Gizmos.color = detectedColor;
        }
        else
        {
            Gizmos.color = notDetectedColor;
        }
    }

    private void WallCheckColorChange()
    {
        if (isWallDetected)
        {
            Gizmos.color = detectedColor;
        }
        else
        {
            Gizmos.color = notDetectedColor;
        }
    }
    #endregion
}
