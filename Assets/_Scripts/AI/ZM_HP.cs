using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZM_HP : MonoBehaviour
{


    public GameObject player;
    public GameObject spawner;
    public float hp;
    public Quaternion rot = new Quaternion(270, 0, 0, 0);


    // Start is called before the first frame update
    void Start()
    {
        hp = GameManager.Instance.zm_HP;
        spawner = GameObject.Find("--SPAWN SYSTEM--");
        player = GameObject.Find("Player");
    }

    public void ReduceHP(float damage, bool headShot)
    {
        hp -= damage;
        if (hp <= 0)
        {
            zm_Death(headShot);
        }
        else
        {
            GameManager.Instance.AddPoints(10);
        }

    }
    public void zm_Death(bool headShot)
    {
        GameManager.Instance.killScore++;
        GameManager.Instance.zm_alive--;
        if (headShot)
        {
            GameManager.Instance.AddPoints(130);
        }
        else
        {
            GameManager.Instance.AddPoints(90);
        }

        if(GameManager.Instance.powerUp_current < GameManager.Instance.powerUp_max)
        {
            float rnd = GameManager.Instance.RandomNumberGenerator(0, 1);
            if (rnd > .45f && rnd < .50f)
            {
                Vector3 pos = new Vector3(this.transform.position.x, 2f, this.transform.position.z);
                GameManager.Instance.InstancePowerUp(0, pos, rot);
                GameManager.Instance.powerUp_current++;
            }
            if (rnd > .65f && rnd < .70f)
            {
                Debug.Log("MaxAmmo!");
                Vector3 pos = new Vector3(this.transform.position.x, 2f, this.transform.position.z);
                GameManager.Instance.InstancePowerUp(1, pos, rot);
                GameManager.Instance.powerUp_current++;
            }
            if (rnd > .10f && rnd < .175f)
            {
                Debug.Log("DoublePoints!");
                Vector3 pos = new Vector3(this.transform.position.x, 2f, this.transform.position.z);
                GameManager.Instance.InstancePowerUp(2, pos, rot);
                GameManager.Instance.powerUp_current++;
            }
        }

            Destroy(this.gameObject);
    }
}
