using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StriderAttack : MonoBehaviour {

    public int AttackMultiplier = 2;
    public int AttackPoints = 10;

    public Transform attackTracker;
    public LayerMask enemyLayer; 

    public float Attack1Radius = 1.0f;
    public float Attack2Radius = 1.0f;
    public float Attack3Radius = 1.5f;
    public float BigAttack1Radius = 3.0f;

    private Animator animator;
    private AttackStateMachine[] attackStates;
    private StriderControl control;
    private int currentComboState = 0;
    private int maxCombo = 4;
    private float centerX;
    private float centerY;
    private bool attacking = false;
    private float attackSpeed = 1.0f;
    private float maxAttackSpeed = 3.0f;

    private Queue<int> attackQueue; 

    private const string COMBO_STATE = "AttackCombo";

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        attackStates = new AttackStateMachine[maxCombo];
        attackStates[0] = animator.GetBehaviour<Attack1StateMachine>();
        attackStates[1] = animator.GetBehaviour<Attack2StateMachine>();
        attackStates[2] = animator.GetBehaviour<Attack4StateMachine>();
        attackStates[3] = animator.GetBehaviour<Attack3StateMachine>();

        maxCombo = attackStates.Length;

        control = GetComponent<StriderControl>();
        foreach(AttackStateMachine att in attackStates)
        {
            print(att.name);
            att.SetStriderControl(control);
            att.SetAttackControl(this);
        }

        centerX = GetComponent<Renderer>().bounds.size.x / 2.0f;
        centerY = GetComponent<Renderer>().bounds.size.y / 2.0f;

        attackQueue = new Queue<int>();
    }

    public void SetAttack(int combo)
    {
        animator.SetInteger(COMBO_STATE, combo);
    }

    public void NextAttack()
    {
        if (attackQueue.Count > 0)
            SetAttack(attackQueue.Dequeue());
        else
        {
            Reset();
        }
    }

    public void IncreaseAttackSpeed()
    {
        if(attackSpeed < maxAttackSpeed)
        {
            attackSpeed += 0.2f;
            animator.SetFloat("attackSpeed", attackSpeed);
        }
    }

    public void BasicAttack()
    {
        //Attacking Animation State
        animator.SetInteger("AnimState", 2);

        currentComboState++;

        if (currentComboState <= maxCombo)
        {
            if (currentComboState == 1)
                SetAttack(currentComboState);
            else
                attackQueue.Enqueue(currentComboState);
        }
    }

    public void Reset()
    {
        currentComboState = 0;
        animator.SetInteger(COMBO_STATE, currentComboState);
        control.Idle();
    }


    //Attack1
    public void Attack1()
    {
        StartCoroutine(DetectContacts(attackTracker.position, Attack1Radius));
    }

    //Attack2
    public void Attack2()
    {
        StartCoroutine(DetectContacts(attackTracker.position, Attack2Radius));
    }

    //Attack3
    public void Attack3()
    {
        StartCoroutine(DetectContacts(attackTracker.position, Attack3Radius));
    }

    public void Attack4()
    {
        StartCoroutine(DetectContacts(attackTracker.position, 1.5f));
    }

    public void BigAttack1()
    {
        animator.SetInteger("AnimState", 3);
        animator.SetInteger("BigAttack", 1);
        StartCoroutine(DetectContacts(attackTracker.position, BigAttack1Radius));
    }

    //Detect collisions
    private IEnumerator DetectContacts(Vector2 from, Vector2 to, int pt)
    {
        float scale_x = transform.localScale.x;
        Vector2 increment = new Vector2(scale_x*1f, 0f);
        while(from.x < to.x)
        {
            DetectContact(from, pt);
            from += increment;
            yield return null;
        }

    }

    private IEnumerator DetectContacts(Vector2 pos, float radius)
    {
        foreach (Collider2D enemyCollider in Physics2D.OverlapCircleAll(pos, radius, enemyLayer))
        {
            EnemyBehaviour enemy = enemyCollider.gameObject.GetComponent<EnemyBehaviour>();
            if (enemy)
                enemy.ReduceHp(RandomAttackPoint(AttackPoints));
            yield return null;
        }
    }

    private void DetectContact(Vector2 pos, int pt)
    {
        //Collider2D enemyCollider;
        foreach(Collider2D enemyCollider in Physics2D.OverlapCircleAll(pos, 1f, enemyLayer)){
            EnemyBehaviour enemy = enemyCollider.gameObject.GetComponent<EnemyBehaviour>();
            if (enemy)
                enemy.ReduceHp(pt);
        }
    }

    private int RandomAttackPoint(int attackPt)
    {
        //Generates a random attack point value based on some function
        return AttackMultiplier*Random.Range(attackPt - 10, attackPt + 10);
    }

    public bool isAttacking()
    {
        return attacking;
    }

    public void SetAttacking(bool flag)
    {
        attacking = flag;
    }

}

