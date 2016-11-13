using UnityEngine;
using System.Collections;

public class Attack2StateMachine : AttackStateMachine {

    public override void Attack()
    {
        Debug.Log("Attack 2");
        attackControl.Attack2();
    }
}
