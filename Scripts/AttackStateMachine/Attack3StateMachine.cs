using UnityEngine;
using System.Collections;

public class Attack3StateMachine : AttackStateMachine {

    public override void Attack()
    {
        attackControl.Attack3(); 
    }
}
