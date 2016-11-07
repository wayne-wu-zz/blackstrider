using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class GameManager : MonoBehaviour {

    public GameObject hero;
    public GameObject enemyPrefab;

    public int maxEnemies = 10;
    public int experiencePerKill = 25;
    public int startingLives = 3;
    public int maxLives = 5;
    public float maxSpawnFrequency = 0.5f;

    private int _lives = 3;

    public int lives
    {
        get
        {
            return _lives;
        }
        set
        {
            _lives = value;
            string text = "";
            for (int i = 0; i < _lives; i++)
            {
                text += "❤";
            }
            GUILives.text = text;
        }
    }

    private int distanceTravelled = 0;
    private int killCount = 0;
    private int level = 1;
    private int enemyCount = 0;
    private int exp = 0;
    private int nextExp = 100;
    private int skillPoints = 0;

    private int lastCheckPoint = 0; 

    private float startPosition;
    private int spawnFrequency;
    private int spawnNumberMin = 1;

    private Transform enemiesLayer;

    private float screenWidth;

    private bool spawning = false;

    //Enemy Settings
    private float enemySpeed
    {
        get
        {
            return 5f + distanceTravelled / 20;
        }
    }
    private int enemyHealth
    {
        get
        {
            return 100 + (killCount / 2);
        }
    }


    //Hero stuff
    private StriderAttack heroAttack;
    private StriderControl heroControl;
    
    //GUI Stuff
    private Text GUIKillCount;
    private Text GUIDistance;
    private Slider GUIExpBar;
    private Text GUISkillPt;
    private Button GUIAddAttack;
    private Button GUIAddSpeed;
    private Text GUILives;


    // Use this for initialization
    void Start () {
        //Vector3 start = new Vector3(0, 0, Camera.main.nearClipPlane);
        //Vector3 end = new Vector3(Screen.width, 0, Camera.main.nearClipPlane);
        //screenWidth = (Camera.main.ScreenToWorldPoint(end) - Camera.main.ScreenToWorldPoint(start)).magnitude;
        print("Screen Width: " + screenWidth);
        enemiesLayer = GameObject.Find("Enemies").transform;
        startPosition = hero.transform.position.x;

        heroAttack = hero.GetComponent<StriderAttack>();
        heroControl = hero.GetComponent<StriderControl>();

        GUIKillCount = transform.Find("InGameUI/KillCount/KillNumber").gameObject.GetComponent<Text>();
        GUIKillCount.text = killCount.ToString();
        GUIDistance = transform.Find("InGameUI/DistanceCount/DistanceNumber").gameObject.GetComponent<Text>();
        GUIDistance.text = distanceTravelled.ToString();
        GUIExpBar = transform.Find("ExpCanvas/ExpUI/ExpBar").gameObject.GetComponent<Slider>();
        GUISkillPt = transform.Find("ExpCanvas/ExpUI/SkillsUI/SkillPoints").gameObject.GetComponent<Text>();
        GUIAddAttack = transform.Find("ExpCanvas/ExpUI/SkillsUI/AddAttack").gameObject.GetComponent<Button>();
        GUIAddSpeed = transform.Find("ExpCanvas/ExpUI/SkillsUI/AddSpeed").gameObject.GetComponent<Button>();
        GUILives = transform.Find("ExpCanvas/ExpUI/Lives").gameObject.GetComponent<Text>();

        GUISkillPt.text = "Skill Points: " + skillPoints;
        GUIExpBar.maxValue = nextExp;
        GUIExpBar.value = exp;

        lives = startingLives;
	}
	
    void FixedUpdate()
    {
        if (lives == 0)
        {
            GameOver();
        }

        distanceTravelled = Mathf.Max(distanceTravelled, (int)Mathf.Round(hero.transform.position.x - startPosition)/10);
        GUIDistance.text = distanceTravelled.ToString();

        //Gives an extra live every 50 meter 
        if (distanceTravelled / 50 != lastCheckPoint)
        {
            lastCheckPoint = distanceTravelled / 50;
            if (lives < maxLives)
                lives++;
        }

        if (skillPoints == 0)
        {
            GUIAddAttack.interactable = false;
            GUIAddSpeed.interactable = false;
        }
        else
        {
            GUIAddAttack.interactable = true;
            GUIAddSpeed.interactable = true;
        }

        if (!spawning)
        {
            spawning = true;
            StartCoroutine(Spawn(1/GetSpawnFrequency()));
        }
    }

    public void IncreaseKillCount()
    {
        exp += experiencePerKill;
        if (exp > nextExp)
        {
            LevelUp();
        }
        GUIExpBar.value = exp;
        enemyCount--;
        killCount++;

        GUIKillCount.text = killCount.ToString();
    }

    public void IncreaseAttack()
    {
        //Debug.Log("Increasing Attack");
        if (skillPoints > 0)
        {
            heroAttack.AttackMultiplier++;
            heroAttack.AttackPoints += 3;
            DecreaseSkillPoint();
        }
    }

    public void IncreaseSpeed()
    {
        if (skillPoints > 0)
        {
            heroControl.maxSpeed += 0.1f;
            DecreaseSkillPoint();
        }
    }

    //TODO: Change this to getter setter
    private void DecreaseSkillPoint()
    {
        skillPoints--;
        GUISkillPt.text = "Skill Points: " + skillPoints;
    }

    public void DecreaseEnemyCount()
    {
        enemyCount--;
    }

    void LevelUp()
    {
        nextExp += 100;
        exp = 0;
        level++;
        skillPoints++;
        GUISkillPt.text = "Skill Points: " + skillPoints;
        GUIExpBar.maxValue = nextExp;
    }

    IEnumerator Spawn(float period)
    {
        yield return new WaitForSeconds(period);
        yield return StartCoroutine(SpawnEnemies());
    }


    IEnumerator SpawnEnemies()
    {
        for(int i = 0; i < GetSpawnNumber(); i++)
        {
            if (enemyCount < maxEnemies)
            {
                GameObject enemy = Instantiate(enemyPrefab);
                enemy.transform.parent = enemiesLayer;
                enemy.transform.position = GetSpawnPoint();
                enemy.GetComponent<EnemyMovement>().speed = Random.Range(enemySpeed-3, enemySpeed);
                enemy.GetComponent<EnemyBehaviour>().maxHealth = enemyHealth;
                enemyCount++;
                yield return new WaitForSeconds(Random.Range(0.5f, 2f));
            }
        }

        spawning = false;
    }

    float GetSpawnFrequency()
    {
        //Get the spawn frequency
        float frq = 0.2f + distanceTravelled / 333f;
        if (frq > maxSpawnFrequency)
            frq = maxSpawnFrequency;
        return frq;
    }

    int GetSpawnNumber()
    {
        //Get the number of enemies to spawn
        int num = 5 + distanceTravelled / 20;
        if (num >= maxEnemies)
            num = maxEnemies;
        return num;
    }

    Vector3 GetSpawnPoint()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, 0, Camera.main.nearClipPlane));
        pos.x += 20f;
        pos.y += 10f;
        pos.z = hero.transform.position.z;
        return pos;
    }

    void GameOver()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
    }
}
