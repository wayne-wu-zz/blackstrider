using UnityEngine;
using UnityEngine.Events;
using UnityEditor.Animations; 
using System.Collections.Generic;
using System.Collections;


public class ComboSystem : MonoBehaviour {

    [System.Serializable]
    public class Attack
    {
        private int AttId;
        public float attackPt = 0;
        public Animation animation;
        public UnityAction attackEvent;

        public void DoAttack()
        {
            attackEvent.Invoke(); 
        }
    }

    public List<Attack> attacks = new List<Attack>();
    public GameObject globalScript; 
    private AnimatorController animController; 
    private Animator animator;


	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        if(attacks.Count!=0)
            CreateController();
	}
	
	// Update is called once per frame
	void Update () {
	}

    // Create all the controllers 
    void CreateController()
    {
        foreach(Attack att in attacks)
        {
            
        }
    }

}

