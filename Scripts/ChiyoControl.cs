using UnityEngine;
using System.Collections;

public class ChiyoControl : MonoBehaviour {

    public float maxSpeed = 10f;
    public float jumpForce = 700f;
    public LayerMask groundLayer;
    public Transform groundCheck;

    private bool facingRight = true;
    private bool isGrounded = false;

    private Rigidbody2D rigidBody2D;
    private Animator animator;

	// Use this for initialization
	void Start () {
        rigidBody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space)) 
        {
            Jump();
        }
    }

    void FixedUpdate()
    {

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
        Debug.Log(isGrounded);

        float move = Input.GetAxis("Horizontal");

        rigidBody2D.velocity = new Vector2(move * maxSpeed, rigidBody2D.velocity.y);

        print(rigidBody2D.velocity);

        if (move > 0 && !facingRight)
            Flip();
        else if (move < 0 && facingRight)
            Flip();

        if (move == 0 && isGrounded)
        {
            animator.SetInteger("AnimState", 0);
        }
        else if (move != 0 && isGrounded)
        {
            animator.SetInteger("AnimState", 1);
        }

    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void Jump()
    {
        isGrounded = false;
        animator.SetInteger("AnimState", -1); //start jumping animation
        rigidBody2D.AddForce(new Vector2(0, jumpForce));
        
    }
}
