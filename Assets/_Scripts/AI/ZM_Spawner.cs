using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

/*
    Este script gestiona la aparición de zombies durante el juego y controla varios parámetros del juego
    como el recuento de oleadas, el recuento de zombies, los tiempos de aparición y las transiciones de rondas.
*/

public class ZM_Spawner : MonoBehaviour
{
    [Header("Generadores")]
    public Transform[] zmSpawners; // Array de transformaciones de generadores de zombies
    public bool activeSpawner; // Indica si el generador está activo

    // Start se llama antes del primer frame
    void Start()
    {
        // Inicializa la primera oleada
        NextWave();

        // Actualiza los elementos de la interfaz de usuario con la información de la oleada actual
        GameManager.Instance.waveText.text = GameManager.Instance.wave.ToString();

        // Aumenta la salud del jugador según la oleada actual
        GameManager.Instance.IncreaseZombieHP(GameManager.Instance.wave);

        // Configura los tiempos para la aparición de zombies
        Timings();
    }

    // Update se llama una vez por frame
    private void Update()
    {
        // Comprueba si se cumplen las condiciones para aparecer zombies
        if (GameManager.Instance.timer >= GameManager.Instance.timerGoal && GameManager.Instance.roundEnd == false && activeSpawner)
        {
            // Aparece zombies
            ZM_Spawn();
        }
        // Si el temporizador aún no ha alcanzado la meta, continúa contando
        else if (GameManager.Instance.timer < GameManager.Instance.timerGoal)
        {
            GameManager.Instance.timer++;
        }

        // Comprueba si todos los zombies han muerto y se han aparecido todos los zombies para terminar la ronda
        if (GameManager.Instance.zm_alive.Equals(0) && GameManager.Instance.zm_spawned.Equals(GameManager.Instance.zm_Count))
        {
            GameManager.Instance.roundEnd = true;
            GameManager.Instance.waveText.GetComponent<Animator>().SetTrigger("onRoundChange");
            StartCoroutine(NewRound()); // Inicia una nueva ronda
        }
    }

    // Determina el recuento de zombies para la próxima oleada
    private void NextWave()
    {
        // Establece el recuento de zombies según la oleada actual
        switch (GameManager.Instance.wave)
        {
            default:
                GameManager.Instance.zm_Count = Mathf.RoundToInt(0.0842f * Mathf.Pow(GameManager.Instance.wave, 2) + 0.1954f * GameManager.Instance.wave + 22.05f);
                break;
            case 1:
                GameManager.Instance.zm_Count = 6;
                break;
            case 2:
                GameManager.Instance.zm_Count = 9;
                break;
                // Otros casos para recuentos específicos de oleadas
        }

        // Restablece el recuento actual de potenciadores
        GameManager.Instance.powerUp_current = 0;
    }

    // Configura los tiempos para la aparición de zombies según la oleada actual
    private void Timings()
    {
        switch (GameManager.Instance.wave)
        {
            case 1:
                GameManager.Instance.zm_Delay = 10f;
                GameManager.Instance.timerGoal = 500;
                break;
                // Otros casos para tiempos específicos basados en el número de oleada
        }
    }

    // Aparece zombies
    private void ZM_Spawn()
    {
        // Comprueba las condiciones para aparecer zombies
        if (GameManager.Instance.zm_alive < GameManager.Instance.zm_maxHorde && GameManager.Instance.roundEnd == false && GameManager.Instance.isPaused == false)
        {
            if (GameManager.Instance.zm_spawned < GameManager.Instance.zm_Count)
            {
                // Incrementa el recuento de apariciones y vivos
                GameManager.Instance.zm_spawned++;
                GameManager.Instance.zm_alive++;
                // Selecciona aleatoriamente un generador
                int rnd = Random.Range(0, zmSpawners.Length);
                // Instancia zombies según la etiqueta del generador
                if (zmSpawners[rnd].CompareTag("BarrierSpawner"))
                {
                    GameManager.Instance.zombieList.Add(Instantiate(GameManager.Instance.zombie[0], zmSpawners[rnd].transform.position, Quaternion.identity));
                    Instantiate(GameManager.Instance.groundFX, zmSpawners[rnd].transform.position, Quaternion.identity);
                }
                else if (zmSpawners[rnd].CompareTag("NormalSpawner"))
                {
                    GameManager.Instance.zombieList.Add(Instantiate(GameManager.Instance.zombie[1], zmSpawners[rnd].transform.position, Quaternion.identity));
                    Instantiate(GameManager.Instance.groundFX, zmSpawners[rnd].transform.position, Quaternion.identity);
                }
                // Reinicia el temporizador
                GameManager.Instance.timer = 0;
            }
        }
    }

    // Corrutina para iniciar una nueva ronda
    private IEnumerator NewRound()
    {
        // Restablece el recuento de apariciones y espera un tiempo antes de iniciar la próxima ronda
        GameManager.Instance.zm_spawned = 0;
        yield return new WaitForSeconds(GameManager.Instance.zm_Delay);
        // Incrementa el recuento de oleadas y activa la animación de inicio de ronda en la interfaz de usuario
        GameManager.Instance.wave++;
        GameManager.Instance.waveText.GetComponent<Animator>().SetTrigger("onRoundStart");
        // Aumenta la salud de los zombies para oleadas hasta 55
        if (GameManager.Instance.wave < 55)
        {
            GameManager.Instance.zombie[0].GetComponent<ZM_AI>().hp = GameManager.Instance.IncreaseZombieHP(GameManager.Instance.wave);
        }
        // Configura parámetros para la próxima oleada
        NextWave();
        Timings();
        // Actualiza elementos de la interfaz de usuario y reinicia temporizador y bandera de fin de ronda
        GameManager.Instance.waveText.text = GameManager.Instance.wave.ToString();
        GameManager.Instance.timer = 0;
        GameManager.Instance.roundEnd = false;
        // Reinicia usos para cajas misteriosas
        for (int i = 0; i < GameManager.Instance.mysteryBox.Length; i++)
        {
            GameManager.Instance.mysteryBox[i].GetComponent<MysteryBox>().currentUses = 0;
        }
    }

    // Activa el generador cuando el jugador entra en la zona de activación
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activeSpawner = true;
        }
    }

    // Desactiva el generador cuando el jugador sale de la zona de activación
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activeSpawner = false;
        }
    }
}



