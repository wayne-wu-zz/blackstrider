using UnityEngine;
using System.Collections;

public class Attack1StateMachine : AttackStateMachine {


    public override void Attack()
    {
        Debug.Log("Attack 1");
        attackControl.Attack1();
    }

}
