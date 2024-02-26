using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Soul_AI : MonoBehaviour
{
    public NavMeshAgent agent = null;
    public GameObject[] targetContainer;
    float d_Nearest;
    GameObject nearestObject;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        targetContainer = GameObject.FindGameObjectsWithTag("SoulBox");
        nearestObject = targetContainer[0];
        d_Nearest = Vector3.Distance(transform.position, nearestObject.transform.position);
        for (int i = 1; i < targetContainer.Length; i++)
        {
            float distanceToCurrent = Vector3.Distance(transform.position, targetContainer[i].transform.position);

            if (distanceToCurrent < d_Nearest)
            {
                nearestObject = targetContainer[i];
                d_Nearest = distanceToCurrent;
            }
        }

        MoveToTarget();
    }
    private void MoveToTarget()
    {
        agent.SetDestination(nearestObject.transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SoulBox"))
        {
            other.GetComponentInParent<SoulHarvest>().actualSouls++;
            GameManager.Instance.CheckSoulCompletion(other.GetComponentInParent<SoulHarvest>().boxID);
            other.GetComponentInParent<SoulHarvest>().OpenContainer();
            Destroy(gameObject, 3f);
        }
    }
}
