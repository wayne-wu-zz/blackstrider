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
    private AttackStateMachine attackState;
    private AttackStateMachine[] attackStates;
    private StriderControl control;
    private int currentComboState = 0;
    private int maxCombo = 4;
    private float centerX;
    private float centerY;
    private bool attacking = false;

    private const string COMBO_STATE = "AttackCombo";

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        attackState = animator.GetBehaviour<AttackStateMachine>();
        attackStates = new AttackStateMachine[maxCombo];
        attackStates[0] = animator.GetBehaviour<Attack1StateMachine>();
        attackStates[1] = animator.GetBehaviour<Attack2StateMachine>();
        attackStates[2] = animator.GetBehaviour<Attack4StateMachine>();
        attackStates[3] = animator.GetBehaviour<Attack3StateMachine>();
        control = GetComponent<StriderControl>();
        foreach(AttackStateMachine att in attackStates)
        {
            print(att.name);
            att.SetStriderControl(control);
            att.SetAttackControl(this);
        }

        centerX = GetComponent<Renderer>().bounds.size.x / 2.0f;
        centerY = GetComponent<Renderer>().bounds.size.y / 2.0f;
    }

    void Update()
    {
        print(transform.position);
        //print(isAttacking());
    }

    public void BasicAttack()
    {
        //Attacking Animation State
        animator.SetInteger("AnimState", 2);

        currentComboState++;
        if (currentComboState <= maxCombo)
        {
           // print("ComboState:" + currentComboState);
            if (isAttacking()&&currentComboState-2>=0)
                attackStates[currentComboState-2].SetNextAttack(true);
            SetComboState(currentComboState);
        }
        else
        {
            Reset();
        }
    }

    public void SetComboState(int num)
    {
        currentComboState = num;
        animator.SetInteger(COMBO_STATE, num);
    }

    public void Reset()
    {
        currentComboState = 0;
        animator.SetInteger(COMBO_STATE, currentComboState);
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


    public class Attack
    {
        public int attack_id;
        public AttackStateMachine state;

        public Attack(int id, AttackStateMachine state)
        {
            this.attack_id = id;
            this.state = state;
        }

        public void DoAttack()
        {

        }

    }

}

