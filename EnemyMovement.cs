using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour {

    public Transform target;
    public float speed = 3f;
    public float jumpForce = 800;
    public float distanceToTarget = 0.5f;
    public Transform groundCheck;
    public Transform barrierCheck;
    public Transform blockCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rigidBody2D;
    private Animator animator;
    private EnemyBehaviour behaviour;
    private float distance;
    private bool facingRight = true;
    private bool grounded = false;

    private float velocityx;
    private float previousX;

	// Use this for initialization
	void Start () {
        rigidBody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        behaviour = GetComponent<EnemyBehaviour>();
        target = GameObject.Find("EnemyTarget").transform;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (behaviour.isDead())
            return;

        grounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
        if(grounded)
            animator.SetBool("Grounded", grounded);

        distance = (target.position - transform.position).magnitude;

        if (behaviour.isFreezing())
            return;

        if (target.position.x > transform.position.x && !facingRight)
            Flip();
        if (target.position.x < transform.position.x && facingRight)
            Flip();

        if (distance > distanceToTarget)
        {
            animator.SetInteger("AnimState", 1);
            Move();
            if (HasBarrier()&&grounded)
                Jump();
            //if (isBlocked())
            //{
            //    int moveBack = (facingRight) ? -1 : 1;
            //    Vector3 pos = transform.position;
            //    pos.x += moveBack;
            //    transform.position = pos;

            //}
        }
        else
        {
            behaviour.Damage();
            //Idle();
        }

        //Fix Getting Stuck
        if(previousX == transform.position.x)
        {
            transform.position += new Vector3(0f, 0.01f, 0f);
        }
        if (Physics2D.OverlapPoint(transform.position, groundLayer))
            Destroy(gameObject);

        previousX = transform.position.x;
        velocityx = rigidBody2D.velocity.x;
	}

    void Move()
    {
        int direction = (facingRight) ? 1 : -1;
        rigidBody2D.velocity = new Vector2(direction * speed, rigidBody2D.velocity.y);
    }

    void Jump()
    {
        grounded = false;
        animator.SetBool("Grounded", false);
        rigidBody2D.AddForce(new Vector2(0, jumpForce));
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void Idle()
    {
        animator.SetInteger("AnimState", 0);
    }

    bool isBlocked()
    {
        int direction = (facingRight) ? 1 : -1;
        //Vector2 pos = new Vector2(groundCheck.position.x + direction * 1.5f, groundCheck.position.y + 0.5f);
        return Physics2D.OverlapPoint(blockCheck.position, groundLayer);
    }

    bool HasBarrier()
    {
        return Physics2D.OverlapPoint(barrierCheck.position, groundLayer);
    }

 




    
}
