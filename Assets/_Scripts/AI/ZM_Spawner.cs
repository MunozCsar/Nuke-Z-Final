using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class ZM_Spawner : MonoBehaviour
{   
    [Header("Spawners")]
    public Transform[] zmSpawners;

    public bool activeSpawner, barrierSpawner;
    // Start is called before the first frame update
    void Start()
    {
        NextWave();
        GameManager.Instance.waveText.text = GameManager.Instance.wave.ToString();
        GameManager.Instance.IncreaseHP(GameManager.Instance.wave);
        Timings();
    }

    private void Update()
    {
        GameManager.Instance.zombieCount.text = ("Zombies: " + GameManager.Instance.zm_alive.ToString());
        if (GameManager.Instance.timer >= GameManager.Instance.timerGoal && GameManager.Instance.roundEnd == false && activeSpawner)
        {
            zm_Spawn();
        }
        else if(GameManager.Instance.timer < GameManager.Instance.timerGoal)
        {
            GameManager.Instance.timer++;
        }

        if (GameManager.Instance.zm_alive.Equals(0) && GameManager.Instance.zm_spawned.Equals(GameManager.Instance.zm_Count))
        {
            GameManager.Instance.roundEnd = true;
            GameManager.Instance.waveText.GetComponent<Animator>().SetTrigger("onRoundChange");
            StartCoroutine(NewRound());
        }
    }

    private void NextWave()
    {
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
            case 3:
                GameManager.Instance.zm_Count = 13;
                break;
            case 4:
                GameManager.Instance.zm_Count = 15;
                break;
            case 5:
                GameManager.Instance.zm_Count = 18;
                break;
            case 6:
                GameManager.Instance.zm_Count = 21;
                break;
            case 7:
                GameManager.Instance.zm_Count = 24;
                break;
            case 8:
                GameManager.Instance.zm_Count = 27;
                break;
            case 9:
                GameManager.Instance.zm_Count = 30;
                break;
            case 10:
                GameManager.Instance.zm_Count = 32;
                break;
            case 11:
                GameManager.Instance.zm_Count = 35;
                break;
        }
        GameManager.Instance.powerUp_current = 0;
    }

    private void Timings()
    {
        switch (GameManager.Instance.wave)
        {
            case 1:
                GameManager.Instance.zm_Delay = 10f;
                GameManager.Instance.timerGoal = 500;
                break;
            case 5:
                GameManager.Instance.zm_Delay = 7.5f;
                GameManager.Instance.timerGoal = 400;
                break;
            case 10:
                GameManager.Instance.zm_Delay = 5f;
                GameManager.Instance.timerGoal = 325;
                break;
            case 20:
                GameManager.Instance.zm_Delay = 3.5f;
                GameManager.Instance.timerGoal = 200;
                break;
        }
        
    }
    
    private void zm_Spawn()
    {
        if (GameManager.Instance.zm_alive < GameManager.Instance.zm_maxHorde && GameManager.Instance.roundEnd == false && GameManager.Instance.isPaused == false) 
        {
            if (GameManager.Instance.zm_spawned < GameManager.Instance.zm_Count)
            {
                GameManager.Instance.zm_spawned++;
                GameManager.Instance.zm_alive++;
                int rnd = Random.Range(0, zmSpawners.Length);
                if (!barrierSpawner)
                {
                    Instantiate(GameManager.Instance.zombie[0], zmSpawners[rnd].transform.position, Quaternion.identity);
                }
                else
                {
                    Instantiate(GameManager.Instance.zombie[1], zmSpawners[rnd].transform.position, Quaternion.identity);
                }
                GameManager.Instance.timer = 0;
            }
        }
    }
 

    private IEnumerator NewRound()
    {
        GameManager.Instance.zm_spawned = 0;
        yield return new WaitForSeconds(GameManager.Instance.zm_Delay);
        GameManager.Instance.wave++;
        GameManager.Instance.waveText.GetComponent<Animator>().SetTrigger("onRoundStart");
        if(GameManager.Instance.wave < 55)
        {
            GameManager.Instance.zombie[0].GetComponent<ZM_HP>().hp = GameManager.Instance.IncreaseHP(GameManager.Instance.wave);
        }
        NextWave();
        Timings();
        GameManager.Instance.waveText.text = GameManager.Instance.wave.ToString();
        GameManager.Instance.timer = 0;
        GameManager.Instance.roundEnd = false;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activeSpawner = true;
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activeSpawner = false;
        }
    }
}
    

