using UnityEngine;
using System.Collections;

public class StriderControl : MonoBehaviour {

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
    private float xBoundary;

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

        //Jumping
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        //Basic Attack
        if (Input.GetMouseButtonDown(0))
        {
            //Readjust facing direction based on mouse position
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            if ((mousePos.x < transform.position.x && facingRight) || (mousePos.x > transform.position.x && !facingRight))
                Flip();

            unInterruptable = true;
            attack.BasicAttack();
        }

        //if (Input.GetMouseButtonDown(1))
        //{
        //    unInterruptable = true;
        //    attack.BigAttack1();
        //}
    }

    void FixedUpdate()
    {
        xBoundary = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane)).x;
        print("x: " + xBoundary);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
        animator.SetBool("Grounded", isGrounded);

        float move = Input.GetAxis("Horizontal");

        if(transform.position.x < xBoundary && move < 0)
        {
            rigidBody2D.velocity = new Vector2(0, rigidBody2D.velocity.y);
        }
        else
        {
            rigidBody2D.velocity = new Vector2(move * maxSpeed, rigidBody2D.velocity.y);
        }

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
