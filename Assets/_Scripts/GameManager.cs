using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject[] weaponPrefabs;
    public GameObject[] DamageIndicators;
    public GameObject[] powerUpArray = new GameObject[3];
    public GameObject[] soulBoxes;
    public GameObject[] mysteryBox;
    public ParticleSystem[] bloodFX;
    public ParticleSystem wallChipFX;
    public ParticleSystem groundFX;
    public Camera playerCam;
    public int powerUp_max, powerUp_current;
    public ParticleSystem powerUp_fx;
    public bool[] soulCompletion;
    [SerializeField] GameObject options;
    [SerializeField] GameObject graphics;
    [SerializeField] GameObject controls;
    [SerializeField] GameObject volume;
    [SerializeField] GameObject credits;
    public Image loadingBar;
    public GameObject loadingScreen;

    public bool findzombies;

    public TMP_Text scoreText;
    public int score, kills, pointsOnHit, pointsOnKill, pointsOnHead, pointsOnNuke;
    public int playerScore, killScore;
    public GameObject scoreBoard, pauseCanvas, pointsInstance, damageIndicatorsContainer;
    public TMP_Text totalScore, totalKills, interactText;
    public bool isPaused = false, doublePoints, instaKill, gameOver, endGameTrigger;
    public float instaKillTimer, doublePointsTimer;
    public Slider slider;
    public float slidervalue;

    public static GameManager Instance { get; private set; }
    #region Zombie Spawn Variables

    [Header("Zombie prefabs")]
    public GameObject[] zombie;
    public float zm_HP;
    public int zm_Damage;
    public List<GameObject> zombieList;

    [Header("Waves")]
    public int wave = 1, powerUps;
    public bool roundEnd;
    public TMP_Text waveText;
    public float zm_Delay;

    [Header("Zombie count")]
    public int zm_Count;
    public int zm_maxHorde = 24;
    public int zm_spawned = 0;
    public int zm_alive;
    public TMP_Text zombieCount;

    [Header("Timer")]
    public int timer = 0;
    public int timerGoal = 25;
    public bool spawnZM = false;
    #endregion
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        //DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = score.ToString();
        scoreBoard.SetActive(false);
        IncreaseHP(wave);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        options.gameObject.SetActive(false);
        graphics.gameObject.SetActive(false);
        controls.gameObject.SetActive(false);
        volume.gameObject.SetActive(false);
        credits.gameObject.SetActive(false);
        slider.value = PlayerPrefs.GetFloat("volumenAudio", 0.5f);
        AudioListener.volume = slidervalue;

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused)
        {

            PauseMenu();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isPaused)
        {

            ResumeGame();
        }

        if (instaKill && instaKillTimer < 30) //Timer de 30 segundos 
        {
            instaKillTimer += Time.deltaTime;
        }
        else
        {
            instaKill = false;
            instaKillTimer = 0f;
        }

        if (doublePoints && doublePointsTimer < 30) //Timer de 30 segundos 
        {
            doublePointsTimer += Time.deltaTime;
        }
        else
        {
            doublePoints = false;
            doublePointsTimer = 0f;
        }


        //DebugMouse();
        UpdateScoreBoard();
        if (Input.GetKey(KeyCode.Tab))
        {
            scoreBoard.SetActive(true);
        }
        else if(!gameOver)
        {
            scoreBoard.SetActive(false);
        }
        if (isPaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    #region PointManager
    public void AddPoints(int points)
    {
        if (!doublePoints)
        {
            score += points;
            playerScore += points;
            pointsInstance.GetComponent<TMP_Text>().color = new Color32(255, 174, 0, 255);
            pointsInstance.GetComponent<TMP_Text>().text = points.ToString();
        }
        else
        {
            score += points * 2;
            playerScore += points;
            pointsInstance.GetComponent<TMP_Text>().color = new Color32(255, 174, 0, 255);
            pointsInstance.GetComponent<TMP_Text>().text = (points * 2).ToString();
        }

        Instantiate(pointsInstance, scoreText.transform);
        UpdateScoreText();
    }

    public void ReduceScore(int cost)
    {
        score -= cost;
        pointsInstance.GetComponent<TMP_Text>().color = new Color32(200, 0, 0, 255);
        pointsInstance.GetComponent<TMP_Text>().text = "-" + cost.ToString();
        Instantiate(pointsInstance, scoreText.transform);
        UpdateScoreText();
    }
    #endregion

    public void Nuke()
    {
        score += pointsOnNuke;
        UpdateScoreText();
        UpdateScoreBoard();
        foreach (GameObject zombie in zombieList)
        {
            zombie.GetComponent<ZM_AI>().zm_Nuke();
        }
        zombieList.Clear();

    }

    public float RandomNumberGenerator(float minIndex, float maxIndex)
    {
        float rnd = Random.Range(minIndex, maxIndex);
        Debug.Log(rnd);
        return rnd;
    }

    public void InstancePowerUp(int i, Vector3 pos, Quaternion rot)
    {
        Quaternion rota = powerUpArray[i].gameObject.transform.rotation;
        Instantiate(powerUpArray[i], pos, rota);
        Debug.Log("PowerUp!");
    }

    public void DamageIndicator(float hp)
    {
        damageIndicatorsContainer.SetActive(true);
        if (hp > 100)
        {
            int rnd = Random.Range(0, 4);
            DamageIndicators[rnd].GetComponent<Animator>().Play("Damage_Fadeout");
        }
        else
        {
            DamageIndicators[4].GetComponent<Animator>().SetTrigger("damage");
        }

        DamageIndicators[5].GetComponent<Animator>().Play("RedScreenDamage");

    }

    public void CheckSoulCompletion(int i)
    {
        if (soulBoxes[i].GetComponent<SoulHarvest>().actualSouls >= 20)
        {
            soulCompletion[i] = true;
        }
        else
        {
        }

        if (soulCompletion[0].Equals(true) && soulCompletion[1].Equals(true) && soulCompletion[2].Equals(true))
        {
            endGameTrigger = true;
        }
    }

    public void UpdateScoreText()
    {
        scoreText.text = score.ToString();
    }

    public void UpdateScoreBoard()
    {
        totalScore.text = playerScore.ToString();
        totalKills.text = killScore.ToString();
    }

    public void Play(int sceneID)
    {
        StartCoroutine(LoadSceneAsync(sceneID));
    }

    IEnumerator LoadSceneAsync(int sceneID)
    {
        loadingScreen.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneID);
        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);

            loadingBar.fillAmount = progressValue;

            yield return null;
        }
    }
    public void Options()
    {
        options.gameObject.SetActive(true);
    }
    public void Exit()
    {
        Application.Quit();
    }
    public void BackOptions()
    {
        options.gameObject.SetActive(false);
    }
    public void BackGraphisAndVolumeAndControlsAndCredits()
    {
        graphics.gameObject.SetActive(false);
        volume.gameObject.SetActive(false);
        controls.gameObject.SetActive(false);
        credits.gameObject.SetActive(false);
    }

    public void Credits()
    {
        credits.gameObject.SetActive(true);
    }
    public void Graphics()
    {
        graphics.gameObject.SetActive(true);
    }
    public void Volume()
    {
        volume.gameObject.SetActive(true);
    }
    public void Controls()
    {
        controls.gameObject.SetActive(true);
    }
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ChangeSlider(float valor)
    {
        slider.value = valor;
        PlayerPrefs.SetFloat("volumenAudio", slidervalue);
        AudioListener.volume = slidervalue;
    }

    public void PauseMenu()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        isPaused = true;
        pauseCanvas.SetActive(true);
    }

    public void ResumeGame()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = false;
        pauseCanvas.SetActive(false);

    }

    public void EndGame()
    {
        SceneManager.LoadScene(2);
    }

    public void GameOver(GameObject player)
    {
        gameOver = true;
        player.GetComponent<CameraMovement>().enabled = false;
        player.GetComponent<PlayerController>().enabled = false;
        UpdateScoreBoard();
        UpdateScoreText();
        scoreBoard.SetActive(true);
        StartCoroutine(ReturnToMenu(15));
    }

    public IEnumerator ReturnToMenu(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene(0);
    }

    public float IncreaseHP(int wave)
    {
        switch (wave)
        {
            case 1:
                zm_HP = 150;
                break;
            case 2:
                zm_HP = 250;
                break;
            case 3:
                zm_HP = 350;
                break;
            case 4:
                zm_HP = 450;
                break;
            case 5:
                zm_HP = 550;
                break;
            case 6:
                zm_HP = 650;
                break;
            case 7:
                zm_HP = 750;
                break;
            case 8:
                zm_HP = 850;
                break;
            case 9:
                zm_HP = 950;
                break;
            default:
                zm_HP *= 1.1f;
                break;
        }
        return zm_HP;
    }

    #region DebugFeatures
    public GameObject spawner;
    private bool toggleSpawn = true;
    public void ResetRounds()
    {
        wave = 1;
    }

    public void ToggleSpawning()
    {
        if (toggleSpawn)
        {
            spawner.SetActive(true);
        }
        else
        {
            spawner.SetActive(false);
        }
        toggleSpawn = !toggleSpawn;
    }

    public void IncreaseScore()
    {
        score += 10000;
        UpdateScoreText();
    }

    //public void DebugMouse()
    //{
    //    if (Input.GetKeyDown(KeyCode.Backspace))
    //    {
    //        Cursor.visible = true;
    //        Cursor.lockState = CursorLockMode.None;
    //    }
    //    else if (Input.GetKeyDown(KeyCode.Return))
    //    {
    //        Cursor.visible = false;
    //        Cursor.lockState = CursorLockMode.Locked;
    //    }
    //}

    
    #endregion

}
