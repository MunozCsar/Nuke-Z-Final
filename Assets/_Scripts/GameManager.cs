using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public bool isKeyActive = false, isKeyObtained = false, allowPickup, obtainedPickup, placeablePart;  //Booleano de llave activa, llave obtenida, permitir pickups y pickup obtenido
    public GameObject truckKey, electricDoor; //Gameobject de la llave y la puerta que se activa con electricidad
    public int selectedPart = 4; //Parte actualmente seleccionada
    public GameObject[] powerParts; //Array que almacena todas las piezas de la electricidad
    public GameObject[] lights; //Array que almacena todas las luces a activar
    public Material lightMaterial;
    public GameObject[] weaponPrefabs; //Array que almacena todas las armas del juego
    public GameObject[] DamageIndicators; //Array que almacena los  diferentes elementos que componen los indicadores de daño
    public GameObject[] powerUpArray; //Array que almacena los powerups
    public GameObject[] radiationContainers; //Array que almacena los contenedores de radiacion
    public GameObject[] mysteryBox; //Array que contiene las cajas misteriosas
    public ParticleSystem[] bloodFX; //Array que contiene los diferentes sistemas de particulas de sangre
    public ParticleSystem wallChipFX; //Sistema de particulas de impacto en pared
    public ParticleSystem groundFX; //Sistema de partículas de zombie saliendo de la tierra
    public Camera playerCam; //Cámara del jugador
    public int powerUp_max, powerUp_current; //Cantidad máxima de powerups y cantidad actual
    public ParticleSystem powerUp_fx; //Sistema de particulas de los powerups
    public bool[] radContainerFull; //Array de bools que almacena si los contenedores están llenos
    [SerializeField] GameObject options, graphics, controls, volume, credits; //Elementos de la UI
    public Image loadingBar; //Barra de carga
    public GameObject loadingScreen; //Pantalla de carga
    public TMP_Text scoreText; //Elemento de UI de puntuación
    public int score, kills, pointsOnHit, pointsOnKill, pointsOnHead, pointsOnNuke, killScore, playerScore; //Valores de puntuación, bajas, puntos por baja, cantidad de bajas y cantidad de puntuacion
    public GameObject scoreBoard, pauseCanvas, pointsInstance, damageIndicatorsContainer; //Contenedor de la pantalla de puntuacion y pantalla de pausa, objeto desde el que se instancia la animación de puntos y el contenedor de los indicadores de daño
    public TMP_Text totalScore, totalKills, interactText; //Elemento de UI de la puntuacion total, la cantidad total de bajas y el texto de interacción
    public bool isPaused = false, doublePoints, instaKill, gameOver, endGameTrigger; //Booleano de pausa, puntos dobles, baja instantanea, partida acabada, trigger de acabar partida y partes sueltas
    public float instaKillTimer, doublePointsTimer; //Timers de los powerups
    public Slider volumeSlider; //Slider del volumen
    public float slidervalue; //Valor del slider

    public static GameManager Instance { get; private set; } //La instancia del GameManager, usada para acceder a los métodos y variables de esta script desde cualquier otra script.
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
        lightMaterial.SetColor("_EmissionColor", Color.black);
        scoreText.text = score.ToString(); //Asigna el valor en string de la variable score al elemento de UI de la puntuación
        scoreBoard.SetActive(false); //Desactiva la pantalla de puntuación
        IncreaseHP(wave); //Llama a la funcion y le asigna la variable 
        Cursor.visible = false; //Desactiva la visibilidad del cursor
        Cursor.lockState = CursorLockMode.Locked; //Bloquea el cursor
        volumeSlider.value = PlayerPrefs.GetFloat("volumenAudio", 0.5f);
        AudioListener.volume = slidervalue;
        options.SetActive(false); //Desactiva el gameobject de opciones
        graphics.SetActive(false); //Desactiva el gameobject de graficos
        controls.SetActive(false); //Desactiva el gameobject de controles
        volume.SetActive(false); //Desactiva el gameobject de volumen
        credits.SetActive(false); //Desactiva el gameobject de creditos

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
        } //Timer de 30 segundos 
        else
        {
            instaKill = false;
            instaKillTimer = 0f;
        }

        if (doublePoints && doublePointsTimer < 30)
        {
            doublePointsTimer += Time.deltaTime;
        } //Timer de 30 segundos 
        else
        {
            doublePoints = false;
            doublePointsTimer = 0f;
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

    public void Nuke() //Funcion del powerup de bomba nuclear
    {
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

    public void DamageIndicator(float hp) //Al recibir daño, activa el contenedor de las animaciones de daño y las ejecuta en base a la cantidad de vida del jugador
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
    public void Exit() //Sale del juego
    {
        Application.Quit();
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
    }
    #endregion
    public float IncreaseHP(int wave) //Aumenta la vida de los enemigos en un valor fijo hasta la ronda 9, y tras la ronda 10 lo multiplica for 1.1
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
