using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class ZM_AI : MonoBehaviour
{
    public Animator zm_Animator;
    public GameObject nearestObject, bodyCollider, headCollider;
    private NavMeshAgent agent = null;
    private Transform target;
    public GameObject[] targetBarrier;
    public bool focusBarrier, isAlive;
    public float t_barrier, barrier_coolDown, d_Nearest, hp; // variable 't' es el tiempo actual del timer, 't_Goal' es el objetivo del timer.
    public Quaternion rot = new Quaternion(270, 0, 0, 0);
    // Start is called before the first frame update
    void Start()
    {
        hp = GameManager.Instance.zm_HP;
        targetBarrier = GameObject.FindGameObjectsWithTag("DestroyBarrier");
        nearestObject = targetBarrier[0];
        d_Nearest = Vector3.Distance(transform.position, nearestObject.transform.position);

        for(int i = 1; i < targetBarrier.Length; i++)
        {
            float distanceToCurrent = Vector3.Distance(transform.position, targetBarrier[i].transform.position);

            if(distanceToCurrent < d_Nearest)
            {
                nearestObject = targetBarrier[i];
                d_Nearest = distanceToCurrent;
            }
        }

        target = GameObject.Find("Player").transform;
        //targetBarrier = GameObject.FindGameObjectWithTag("Barrier_Trigger").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = 2f;

    }

    // Update is called once per frame
    void Update()
    {
        switch (GameManager.Instance.wave)
        {
            default:
                agent.speed = 2f; zm_Animator.SetBool("zombie_Walk", true); zm_Animator.SetBool("zombie_Run", false);
                GameManager.Instance.zm_Damage = 35;
                break;

            case 10:
                agent.speed = 3f; zm_Animator.SetBool("zombie_Walk", true); zm_Animator.SetBool("zombie_Run", false);
                GameManager.Instance.zm_Damage = 50;
                break;

            case 20:
                agent.speed = 4f; zm_Animator.SetBool("zombie_Walk", true); zm_Animator.SetBool("zombie_Run", true);
                GameManager.Instance.zm_Damage = 75;
                break;
        }

        if (!focusBarrier && isAlive)
        {
            MoveToTarget();
        }
        else if(isAlive)
        {
            DestroyBarrier();
        }
        
    }

    private void MoveToTarget()
    {
        agent.SetDestination(target.position);
    }

    private void DestroyBarrier()
    {
        agent.SetDestination(nearestObject.transform.position);
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
            GameManager.Instance.AddPoints(GameManager.Instance.pointsOnHit);
        }

    }

    public void zm_Death(bool headShot)
    {
        GameManager.Instance.killScore++;
        GameManager.Instance.zm_alive--;
        if (headShot)
        {
            GameManager.Instance.AddPoints(GameManager.Instance.pointsOnHead);
            GetComponent<Animator>().Play("Zombie_Headshot");
            agent.speed = 0f;
            agent.isStopped = true;
            isAlive = false;
            headCollider.SetActive(false);
            bodyCollider.SetActive(false);
            GetComponent<SphereCollider>().enabled = false;

        }
        else
        {
            GameManager.Instance.AddPoints(GameManager.Instance.pointsOnKill);
            GetComponent<Animator>().Play("Zombie_Death");
            agent.speed = 0f;
            agent.isStopped = true;
            isAlive = false;
            headCollider.SetActive(false);
            bodyCollider.SetActive(false);
            GetComponent<SphereCollider>().enabled = false;
        }

        if (GameManager.Instance.powerUp_current < GameManager.Instance.powerUp_max)
        {
            float rnd = GameManager.Instance.RandomNumberGenerator(0, 1);
            if (rnd > .45f && rnd < .50f)
            {
                Vector3 pos = new Vector3(this.transform.position.x, this.transform.position.y + 2, this.transform.position.z);
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

        Destroy(this.gameObject, 15f);
        GameManager.Instance.zombieList.Remove(this.gameObject);
    }

    public void zm_Nuke()
    {
        Instantiate(GameManager.Instance.bloodFX[4], transform.position, GameManager.Instance.bloodFX[4].transform.rotation);
        GameManager.Instance.killScore++;
        GameManager.Instance.zm_alive--;
        GetComponent<Animator>().Play("Zombie_Death");
        agent.speed = 0f;
        agent.isStopped = true;
        isAlive = false;
        headCollider.SetActive(false);
        bodyCollider.SetActive(false);
        Destroy(this.gameObject, 15f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            zm_Animator.SetBool("isAttacking", true);
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            agent.speed = 2f;
            zm_Animator.SetBool("isAttacking", false);
        }
        if (other.CompareTag("DestroyBarrier"))
        {
            focusBarrier = false;
            zm_Animator.SetBool("isAttacking", false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("DestroyBarrier"))
        {
            if (t_barrier < barrier_coolDown)
            {
                t_barrier += Time.deltaTime;
            }
            else if (t_barrier >= barrier_coolDown && other.transform.parent.GetComponent<BarrierLogic>().hitPoints > 0)
            {
                zm_Animator.SetBool("isAttacking", true);
                t_barrier = 0f;
                other.transform.parent.GetComponent<BarrierLogic>().ReduceHitPoints();
            }

            if(other.transform.parent.GetComponent<BarrierLogic>().hitPoints <= 0)
            {
                focusBarrier = false;
            }
        }
    }
}
