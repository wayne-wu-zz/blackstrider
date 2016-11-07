using UnityEngine;
using System.Collections;

public class CharacterControl : MonoBehaviour {

    public float maxSpeed = 10f;
    public float jumpForce = 700f;
    public LayerMask groundLayer;
    public Transform groundCheck;

    public float maxHealth;

    public bool unInterruptable = false;

    private bool facingRight = true;
    private bool isGrounded = false;


    private Rigidbody2D rigidBody2D;
    private Animator animator;
    private Vector2 velocity;
    private StriderAttack attack;


    // Use this for initialization
    void Start()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        attack = GetComponent<StriderAttack>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetMouseButtonDown(0))
        {
            unInterruptable = true;
            attack.BasicAttack();
        }
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
        animator.SetBool("Grounded", isGrounded);

        float move = Input.GetAxis("Horizontal");
        rigidBody2D.velocity = new Vector2(move * maxSpeed, rigidBody2D.velocity.y);
        animator.SetFloat("vSpeed", rigidBody2D.velocity.y);

        if (unInterruptable)
            return;

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
        animator.SetBool("Grounded", false);
        rigidBody2D.AddForce(new Vector2(0, jumpForce));
    }

    bool isMoving()
    {
        return rigidBody2D.velocity.x != 0;
    }


    public void Idle()
    {
        animator.SetInteger("AnimState", 0);
        animator.SetInteger("AttackCombo", 0);
        unInterruptable = false;
    }
}
