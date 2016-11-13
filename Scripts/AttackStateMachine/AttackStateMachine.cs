using UnityEngine;
using System.Collections;


public class AttackStateMachine : StateMachineBehaviour {

    public static int RESET = 0;

    public int attackID;
    public float speed;

    protected StriderControl striderControl;
    protected StriderAttack attackControl;
    protected virtual int nextAttack
    {
        get
        {
            return attackID+1;
        }
    }
    private bool next = false;
     

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackControl.SetAttacking(true);
        Attack();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackControl.SetAttacking(false);
        attackControl.NextAttack();
    }

    public virtual void Attack()  { }

    public void SetStriderControl(StriderControl striderControl)
    {
        this.striderControl = striderControl;
    }

    public void SetAttackControl(StriderAttack striderAttack)
    {
        this.attackControl = striderAttack;
    }

    public void SetNextAttack(bool flag)
    {
        this.next = true;
    }
}
