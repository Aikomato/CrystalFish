using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 7f;
    public float jumpForce = 12f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.4f;
    public LayerMask groundLayer;
    public Transform spriteHolder; // Child that holds the sprite

    private Rigidbody rb;
    private Animator anim;
    private bool isGrounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        rb.freezeRotation = true; // important for 3D+sprite

        if (groundCheck == null)
        {
            GameObject gc = new GameObject("GroundCheck");
            gc.transform.SetParent(transform);
            gc.transform.localPosition = new Vector3(0, -0.6f, 0);
            groundCheck = gc.transform;
        }

        if (spriteHolder == null)
            Debug.LogError("Assign the SpriteHolder child object!");
    }

    private void Update()
    {
        HandleMovement();
        HandleFlip();
        HandleJump();
        UpdateAnimator();
        Debug.Log("Grounded: " + isGrounded);
        Debug.Log("FacingRight = " + anim.GetBool("FacingRight"));
        Debug.Log("Current Speed = " + anim.GetFloat("Speed"));
    }

    void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");

        rb.linearVelocity = new Vector3(h * moveSpeed, rb.linearVelocity.y, 0);
    }

    void HandleFlip()
    {
        float h = Input.GetAxisRaw("Horizontal");

        if (h > 0) 
            spriteHolder.localScale = new Vector3(1, 1, 1);
        else if (h < 0) 
            spriteHolder.localScale = new Vector3(-1, 1, 1);
    }

    void HandleJump()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, 0);
        }
    }

    void UpdateAnimator()
    {
    float h = Input.GetAxisRaw("Horizontal");

    // Only change facing direction WHEN ACTUALLY MOVING
    if (h > 0)
        anim.SetBool("FacingRight", true);
    else if (h < 0)
        anim.SetBool("FacingRight", false);

    anim.SetFloat("Speed", Mathf.Abs(h));
    anim.SetBool("IsGrounded", isGrounded);

    }


    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
