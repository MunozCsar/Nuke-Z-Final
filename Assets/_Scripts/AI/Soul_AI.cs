using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
    Este script controla el comportamiento de las almas de los zombies en el juego. Las almas se mueven hacia el contenedor m�s cercano.
*/

public class Soul_AI : MonoBehaviour
{
    public NavMeshAgent agent = null;
    public GameObject[] targetContainer;
    float d_Nearest;
    GameObject nearestObject;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // Inicializa el agente de navegaci�n
    }

    private void Update()
    {
        // Encuentra todos los contenedores en el juego
        targetContainer = GameObject.FindGameObjectsWithTag("SoulBox");
        nearestObject = targetContainer[0]; // Establece el primer contenedor como el m�s cercano inicialmente
        d_Nearest = Vector3.Distance(transform.position, nearestObject.transform.position); // Calcula la distancia al contenedor m�s cercano

        // Encuentra el contenedor m�s cercano entre todos los contenedores
        for (int i = 1; i < targetContainer.Length; i++)
        {
            float distanceToCurrent = Vector3.Distance(transform.position, targetContainer[i].transform.position);

            if (distanceToCurrent < d_Nearest)
            {
                nearestObject = targetContainer[i];
                d_Nearest = distanceToCurrent;
            }
        }

        MoveToTarget(); // Mueve el alma hacia el contenedor m�s cercano
    }

    private void MoveToTarget()
    {
        agent.SetDestination(nearestObject.transform.position); // Establece el destino del agente de navegaci�n hacia el contenedor m�s cercano
    }

    private void OnTriggerEnter(Collider other)
    {
        // Detecta la colisi�n con el contenedor y lo almacena
        if (other.CompareTag("SoulBox"))
        {
            other.GetComponentInParent<SoulHarvest>().actualSouls++;
            GameManager.Instance.CheckContainerCompletion(other.GetComponentInParent<SoulHarvest>().boxID);
            other.GetComponentInParent<SoulHarvest>().OpenContainer();
            Destroy(gameObject, 3f);
        }
    }
}

