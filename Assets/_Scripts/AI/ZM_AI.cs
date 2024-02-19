using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class ZM_AI : MonoBehaviour
{
    private Animator zm_Animator;
    public GameObject damageTrigger, nearestObject;
    private NavMeshAgent agent = null;
    private Transform target;
    public GameObject[] targetBarrier;
    public bool focusBarrier;
    public int zm_Hitpoints;
    public float t, t_Goal, d_Nearest; // variable 't' es el tiempo actual del timer, 't_Goal' es el objetivo del timer.
    // Start is called before the first frame update
    void Start()
    {
        targetBarrier = GameObject.FindGameObjectsWithTag("Barrier_Trigger");
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
        damageTrigger.SetActive(false);
        agent.speed = 2f;
        zm_Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {


        if (!focusBarrier)
        {
            MoveToTarget();
        }
        else
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


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Enter");
            damageTrigger.SetActive(true);
            zm_Animator.SetBool("isAttacking", true);
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Exit");
            agent.speed = 2f;
            damageTrigger.SetActive(false);
            zm_Animator.SetBool("isAttacking", false);
        }
        if (other.CompareTag("Barrier_Trigger"))
        {
            focusBarrier = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Barrier_Trigger"))
        {
            if (t < t_Goal)
            {
                t += Time.deltaTime;
            }
            else if (t >= t_Goal && other.GetComponent<BarrierLogic>().hitPoints > 0)
            {
                t = 0f;
                if(other.GetComponent<BarrierLogic>().hitPoints <= 1)
                {
                    focusBarrier = false;
                }
                other.GetComponent<BarrierLogic>().ReduceHitPoints();
            }
        }
    }
}
