using UnityEngine;
using System.Collections;

public class AddAttackButton : MonoBehaviour {

    public GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }


    void OnClick()
    {
        gameManager.IncreaseAttack(); 
    }
}
