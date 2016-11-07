using UnityEngine;
using System.Collections;


public class AttackStateMachine : StateMachineBehaviour {

    public static int RESET = 0;

    public int stateId;

    protected StriderControl striderControl;
    protected StriderAttack attackControl;
    private int nextAttack = AttackStateMachine.RESET;
    private bool next = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackControl.SetAttacking(true);
        Attack();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackControl.SetAttacking(false);
        if (!next)
            Exit();
        next = false;
    }

    public virtual void Attack()
    {

    }

    protected void Exit()
    {
        if (striderControl)
        {
            striderControl.Idle();
            attackControl.Reset();
        }
    }

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
