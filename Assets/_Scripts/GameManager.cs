using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
    Este script controla las funciones y variables del juego que son usadas por otras partes del juego.
*/

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } //La instancia del GameManager, usada para acceder a los métodos y variables de esta script desde cualquier otra script.

    public bool isKeyActive = false, isKeyObtained = false, allowPickup, obtainedPickup, placeablePart;
    public GameObject truckKey, electricDoor;
    public int selectedPart = 4;
    public GameObject[] powerParts;
    public GameObject[] lights;
    public Material lightMaterial;
    public GameObject[] weaponPrefabs;
    public GameObject[] DamageIndicators;
    public GameObject[] powerUpArray;
    public GameObject[] radiationContainers;
    public GameObject[] mysteryBox;
    public ParticleSystem[] bloodFX;
    public ParticleSystem wallChipFX;
    public ParticleSystem groundFX;
    public Camera playerCam;
    public int powerUp_max, powerUp_current;
    public float powerUpChance;
    public ParticleSystem powerUp_fx;
    public bool[] radContainerFull;
    [SerializeField] GameObject options, graphics, controls, volume, credits;
    public Image loadingBar;
    public GameObject loadingScreen, maxAmmoUI, nukeUI, instaKillUI, doublePointsUI;
    public TMP_Text scoreText;
    public int score, kills, pointsOnHit, pointsOnKill, pointsOnHead, pointsOnNuke, killScore, playerScore;
    public GameObject scoreBoard, pauseCanvas, pointsInstance, damageIndicatorsContainer;
    public TMP_Text totalScore, totalKills, interactText;
    public bool isPaused = false, doublePoints, instaKill, gameOver, endGameTrigger;
    public float instaKillTimer, instaKillCountdown, doublePointsTimer, doublePointsCountdown;
    public Slider volumeSlider;
    public float slidervalue;


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

    [Header("Timer")]
    public int timer = 0;
    public int timerGoal = 25;
    public bool spawnZM = false;
    #endregion
    private void Awake() //Al iniciar el juego, se comprueba si ya existe una instancia del GameManager, si no la hay, este objeto se vuelve la instancia, si la hay, se destruye este objeto.
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        AssignVariables();

    }

    private void AssignVariables()
    {
        try
        {
        lightMaterial.SetColor("_EmissionColor", Color.black);
        }
        catch { }

        try
        {
        scoreText.text = score.ToString();
        }
        catch { }

        try
        {
        scoreBoard.SetActive(false);
        }
        catch { }

        try
        {
        IncreaseZombieHP(wave);
        }
        catch { }

        try
        {
        volumeSlider.value = PlayerPrefs.GetFloat("volumenAudio", 0.5f);
        }
        catch { }

        try
        {
        AudioListener.volume = slidervalue;
        }
        catch { }

        try
        {
        options.SetActive(false);
        }
        catch { }

        try
        {
        graphics.SetActive(false);
        }
        catch { }

        try
        {
        controls.SetActive(false);
        }
        catch { }

        try
        {
        volume.SetActive(false);
        }
        catch { }

        try
        {
        credits.SetActive(false);
        }
        catch { }

    }

    // Update is called once per frame
    void Update()
    {

        #region Pause&Resume

        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused) //Si el juego no está pausado y se presiona la tecla de Escape se activa el menu de pausa
        {
            PauseMenu();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isPaused) //Si el juego está pausado y se presiona la tecla de Escape se desactiva el menu de pausa
        {

            ResumeGame();
        }
        if (isPaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
        #endregion
        #region PowerupTimers
        if (instaKill && instaKillTimer < 30)
        {
            instaKillTimer += Time.deltaTime;
            instaKillCountdown -= Time.deltaTime;
            instaKillUI.SetActive(true);
            instaKillUI.GetComponent<TMP_Text>().text = "Instakill: " + Mathf.FloorToInt(instaKillCountdown);
        } //Timer de 30 segundos 
        else
        {
            instaKill = false;
            instaKillTimer = 0f;
            instaKillUI.SetActive(false);
            instaKillCountdown = 30f;
        }

        if (doublePoints && doublePointsTimer < 30)
        {
            doublePointsTimer += Time.deltaTime;
            doublePointsCountdown -= Time.deltaTime;
            doublePointsUI.SetActive(true);
            doublePointsUI.GetComponent<TMP_Text>().text = "Double Points: " + Mathf.FloorToInt(doublePointsCountdown);
        } //Timer de 30 segundos 
        else
        {
            doublePoints = false;
            doublePointsTimer = 0f;
            doublePointsUI.SetActive(false);
            doublePointsCountdown = 30f;
        }
        #endregion
        #region ScoreBoard
        UpdateScoreBoard();
        if (Input.GetKey(KeyCode.Tab)) //Al presionar tabulador se activar la pantalla de puntuacion hasta que la tecla se deje de pulsar
        {
            scoreBoard.SetActive(true);
        }
        else if(!gameOver)
        {
            scoreBoard.SetActive(false);
        }
        #endregion
    }

    #region PointManager
    public void AddPoints(int points)
    {
        if (!doublePoints) //Si el powerup de doble puntuacion no está activo se suma la cantidad base de puntos
        {
            score += points;
            playerScore += points;
            pointsInstance.GetComponent<TMP_Text>().color = new Color32(255, 174, 0, 255); //Se le da un color dorado a la instancia de puntos
            pointsInstance.GetComponent<TMP_Text>().text = points.ToString(); //Se le da el valor del coste al texto de la instancia de puntos
        }
        else //Si el powerup de doble puntuacion está activo se suma la cantidad base de puntos multiplicado por 2
        {
            score += points * 2;
            playerScore += points;
            pointsInstance.GetComponent<TMP_Text>().color = new Color32(255, 174, 0, 255); //Se le da un color dorado a la instancia de puntos
            pointsInstance.GetComponent<TMP_Text>().text = (points * 2).ToString(); //Se le da el valor del coste al texto de la instancia de puntos
        }

        Instantiate(pointsInstance, scoreText.transform); //Se instancia la animación de los puntos
        UpdateScoreText();
    }

    public void ReduceScore(int cost) //Se reduce la puntuación del jugador por el valor del coste dado
    {
        score -= cost;
        pointsInstance.GetComponent<TMP_Text>().color = new Color32(200, 0, 0, 255); //Se le da un color rojo a la instancia de puntos
        pointsInstance.GetComponent<TMP_Text>().text = "-" + cost.ToString(); //Se le da el valor del coste al texto de la instancia de puntos
        Instantiate(pointsInstance, scoreText.transform); //Se instancia la animación de los puntos
        UpdateScoreText();
    }
    #endregion

    public void NukePowerUp() //Funcion del powerup de bomba nuclear
    {
        nukeUI.GetComponent<Animator>().Play("RadDepletion");
        AddPoints(pointsOnNuke); //Añade los puntos que otorga el powerup
        UpdateScoreText(); //Actualiza la puntuacion
        UpdateScoreBoard();
        foreach (GameObject zombie in zombieList) //Recorre la lista de zombies y llama a su función de muerte
        {
            zombie.GetComponent<ZM_AI>().ZM_Nuke();
            zm_alive--;
            killScore++;
        }
        zombieList.Clear(); //Limpia la lista entera 

    }

    public float RandomNumberGenerator(float minIndex, float maxIndex) //Genera un número de tipo float atleatorio entre los dos índices dados y lo devuelve
    {
        float rnd = Random.Range(minIndex, maxIndex);
        return rnd;
    }

    public int RandomNumberGenerator(int minIndex, int maxIndex) //Genera un número de tipo int atleatorio entre los dos índices dados y lo devuelve
    {
        int rnd = Random.Range(minIndex, maxIndex);
        return rnd;
    }

    public void InstancePowerUp(int i, Vector3 pos) //Instancia  el powerup en la posición y rotación dada
    {
        Quaternion rot = powerUpArray[i].transform.rotation;
        Instantiate(powerUpArray[i], pos, rot);
    }

    public void ShowDamageIndicators(float hp) //Al recibir daño, activa el contenedor de las animaciones de daño y las ejecuta en base a la cantidad de vida del jugador
    {
        damageIndicatorsContainer.SetActive(true);
        if (hp > 100)
        {
            int rnd = RandomNumberGenerator(0, DamageIndicators.Length); //Ejecuta una de las 4 animaciones posibles
            DamageIndicators[rnd].GetComponent<Animator>().Play("Damage_Fadeout");
        }
        else
        {
            DamageIndicators[4].GetComponent<Animator>().SetTrigger("damage");
        }

        DamageIndicators[5].GetComponent<Animator>().Play("RedScreenDamage");

    }

    public void CheckContainerCompletion(int i) //Comprueba si los contenedores están llenos, y si lo están, activa el trigger que permite finalizar la partida
    {
        if (radiationContainers[i].GetComponent<SoulHarvest>().actualSouls >= 20)
        {
            radContainerFull[i] = true;
        }
        else
        {
        }

        if (radContainerFull[0].Equals(true) && radContainerFull[1].Equals(true) && radContainerFull[2].Equals(true))
        {
            endGameTrigger = true;
        }
    }
    #region UI
    public void UpdateScoreText() //Actualiza el texto de la puntuacion en pantalla
    {
        scoreText.text = score.ToString();
    }

    public void UpdateScoreBoard() //Actualiza los textos de puntuacion y bajas de la pantalla de puntuacion
    {
        totalScore.text = playerScore.ToString();
        totalKills.text = killScore.ToString();
    }
    public void Options() //Muestra la pantalla de opciones
    {
        options.SetActive(true);
    }

    public void BackOptions() //Vuelve al menu de pausa
    {
        options.SetActive(false);
    }
    public void BackGraphisAndVolumeAndControlsAndCredits() //Vuelve al menu de opciones
    {
        graphics.SetActive(false);
        volume.SetActive(false);
        controls.SetActive(false);
        credits.SetActive(false);
    }

    public void Credits() //Muestra la pantalla de creditos
    {
        credits.SetActive(true);
    }
    public void Graphics() //Muestra la pantalla de graficos
    {
        graphics.SetActive(true);
    }
    public void Volume() //Muestra la pantalla de volumen
    {
        volume.SetActive(true);
    }
    public void Controls() //Muestra la pantalla de controles
    {
        controls.SetActive(true);
    }
    public void MainMenu() //Vuelve al menu principal
    {
        SceneManager.LoadScene(0);
    }

    public void ChangeSlider(float valor) //Cambia el valor del slider del volumen
    {
        volumeSlider.value = valor;
        PlayerPrefs.SetFloat("volumenAudio", slidervalue);
        AudioListener.volume = slidervalue;
    }

    public void PauseMenu() //Pausa la partida
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        isPaused = true;
        pauseCanvas.SetActive(true);
    }

    public void ResumeGame() //Continua la partida
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = false; 
        pauseCanvas.SetActive(false);

    }
    #endregion

    #region SceneLoading
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
    public void Play(int sceneID)
    {
        StartCoroutine(LoadSceneAsync(sceneID));
        AssignVariables();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void EndGame() //Carga la escena con la cinemática final
    {
        SceneManager.LoadScene(2);
    }

    public void GameOver(GameObject player) //Acaba la partida, bloquea el movimiento del jugador y de la cámara, muestra la pantalla de puntuación y, tras 15 segundos, carga el menu principal
    {
        gameOver = true;
        player.GetComponent<CameraMovement>().enabled = false;
        player.GetComponent<PlayerController>().enabled = false;
        UpdateScoreBoard();
        UpdateScoreText();
        scoreBoard.SetActive(true);
        StartCoroutine(ReturnToMenu(15));
    }

    public IEnumerator ReturnToMenu(float seconds) //Vuelve al menu principal pasados los segundos indicados
    {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene(0);
        AssignVariables();
    }
    public void Exit() //Sale del juego
    {
        Application.Quit();
    }
    #endregion
    public float IncreaseZombieHP(int wave) //Aumenta la vida de los enemigos en un valor fijo hasta la ronda 9, y tras la ronda 10 lo multiplica for 1.1
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

}
