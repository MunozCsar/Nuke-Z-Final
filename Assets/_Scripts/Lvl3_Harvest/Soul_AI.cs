using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Soul_AI : MonoBehaviour
{
    public NavMeshAgent agent = null;
    public Transform target;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("SoulBox").transform;
        agent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        MoveToTarget();
    }
    private void MoveToTarget()
    {
        agent.SetDestination(target.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SoulBox"))
        {
            Debug.Log("Enter");
            other.GetComponentInParent<SoulHarvest>().actualSouls++;
            Destroy(gameObject, 10f);
        }
    }
}
