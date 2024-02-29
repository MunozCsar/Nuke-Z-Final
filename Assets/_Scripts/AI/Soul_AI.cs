using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
    Este script controla el comportamiento de las almas de los zombies en el juego. Las almas se mueven hacia el contenedor más cercano.
*/

public class Soul_AI : MonoBehaviour
{
    public NavMeshAgent agent = null;
    public GameObject[] targetContainer;
    float d_Nearest;
    GameObject nearestObject;
    public ParticleSystem effectParticles;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // Inicializa el agente de navegación
    }

    private void Update()
    {
        // Encuentra todos los contenedores en el juego
        targetContainer = GameObject.FindGameObjectsWithTag("SoulBox");
        nearestObject = targetContainer[0]; // Establece el primer contenedor como el más cercano inicialmente
        d_Nearest = Vector3.Distance(transform.position, nearestObject.transform.position); // Calcula la distancia al contenedor más cercano

        // Encuentra el contenedor más cercano entre todos los contenedores
        for (int i = 1; i < targetContainer.Length; i++)
        {
            float distanceToCurrent = Vector3.Distance(transform.position, targetContainer[i].transform.position);

            if (distanceToCurrent < d_Nearest)
            {
                nearestObject = targetContainer[i];
                d_Nearest = distanceToCurrent;
            }
        }

        MoveToTarget(); // Mueve el alma hacia el contenedor más cercano
    }

    private void MoveToTarget()
    {
        agent.SetDestination(nearestObject.transform.position); // Establece el destino del agente de navegación hacia el contenedor más cercano
    }

    private void OnTriggerEnter(Collider other)
    {
        // Detecta la colisión con el contenedor y lo almacena
        if (other.CompareTag("SoulBox"))
        {
            other.GetComponentInParent<SoulHarvest>().actualSouls++;
            float fillUp = .9f / other.GetComponentInParent<SoulHarvest>().soulsRequired;
            other.GetComponentInParent<SoulHarvest>().fill.transform.Translate(0, fillUp, 0f);
            GameManager.Instance.CheckContainerCompletion(other.GetComponentInParent<SoulHarvest>().boxID);
            other.GetComponentInParent<SoulHarvest>().OpenContainer();
            StartCoroutine(StopParticles());
            Destroy(gameObject, 3f);
        }
    }

    IEnumerator StopParticles()
    {
        yield return new WaitForSeconds(2f);
        effectParticles.Stop();
    }
}

