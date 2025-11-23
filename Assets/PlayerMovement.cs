using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 7f;
    public float jumpForce = 12f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.25f;
    public LayerMask groundLayer;

    private Rigidbody rb;
    private Animator anim;
    private bool facingRight = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        if (groundCheck == null)
        {
            GameObject gc = new GameObject("GroundCheck");
            gc.transform.SetParent(transform);
            gc.transform.localPosition = new Vector3(0, -0.5f, 0);
            groundCheck = gc.transform;
        }
    }

    private void Update()
    {
        // Horizontal input
        float h = Input.GetAxisRaw("Horizontal");

        // Move player
        rb.linearVelocity = new Vector3(h * moveSpeed, rb.linearVelocity.y, 0);

        // Ground check
        bool isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        // Update facing direction
        if (h > 0) facingRight = true;
        else if (h < 0) facingRight = false;

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, 0);
        }

        // Set Animator parameters
        anim.SetFloat("Horizontal", h);
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetBool("FacingRight", facingRight);
    }
}
