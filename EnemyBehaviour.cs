using UnityEngine;
using System.Collections;

public class EnemyBehaviour : MonoBehaviour {

    public int maxHealth = 100;
    public int defense = 0;
    public GameObject damageText;

    public float disappearTime = 5f;
    public float temporaryImmortalTime = 1f;
    public float temporaryFreezeTime = 1f;

    private int Hp;
    private bool immortal = false;
    private bool dead = false;
    private bool tempFreeze = false;
    private Animator animator;


    private GameManager gameManager;

    // Use this for initialization
    void Start() {
        Hp = maxHealth;
        animator = GetComponent<Animator>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (transform.position.y < -10)
        {
            Destroy(gameObject);
            gameManager.DecreaseEnemyCount();
        }

    }

    public void ReduceHp(int attackPt)
    {
        tempFreeze = true;
        if (!immortal)
        {
            Hp -= (attackPt - defense);
            DrawDamage(attackPt);
            if (Hp <= 0)
            {
                gameManager.IncreaseKillCount();
                Dead();
            }
            immortal = true;
            StartCoroutine(reset());
            StartCoroutine(resetFreeze());
        }
    }

    private void DrawDamage(int attPt)
    {
        //GameObject text = Instantiate(damageText, transform.position, transform.rotation) as GameObject;
        GameObject text = Instantiate(damageText) as GameObject;
        text.GetComponent<UnityEngine.UI.Text>().text = attPt.ToString();
        text.transform.SetParent(GetComponentInChildren<Canvas>().transform, false);
    }

    IEnumerator reset()
    {
        yield return new WaitForSeconds(temporaryImmortalTime);
        immortal = false;
    }

    IEnumerator resetFreeze()
    {
        yield return new WaitForSeconds(temporaryFreezeTime);
        tempFreeze = false;
    }

    public bool isDead()
    {
        return dead;
    }

    public void Damage()
    {
        //doDamage
        if (!dead)
        {
            gameManager.lives--;
            Destroy(gameObject);
            gameManager.DecreaseEnemyCount();
        }
    }


    private IEnumerator SelfDestroy()
    {
        yield return new WaitForSeconds(disappearTime);
        Destroy(gameObject);
    }

    private void Dead()
    {
        dead = true;
        animator.SetBool("Dead", true);
        Destroy(GetComponent<Rigidbody2D>());
        Destroy(GetComponent<Collider2D>());
        StartCoroutine(SelfDestroy());
    }

    public bool isFreezing()
    {
        return tempFreeze;
    }

}
