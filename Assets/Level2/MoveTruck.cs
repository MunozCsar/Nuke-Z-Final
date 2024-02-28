using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Este script gestiona el movimiento del camión cuando se coloca la llave.
*/

public class MoveTruck : MonoBehaviour
{
    public PlayerController player; // Referencia al controlador del jugador
    public bool canPlaceKey = false; // Indica si el camión puede recibir la llave
    public Transform target; // Posición a la que se mueve el camión
    public float speed; // Velocidad de movimiento del camión

    // Se llama en cada frame
    private void Update()
    {
        UseKey(); // Llama a la función para utilizar la llave
    }

    // Utiliza la llave para mover el camión si se cumplen las condiciones
    private void UseKey()
    {
        if (GameManager.Instance.isKeyObtained == true && GameManager.Instance.allowPickup == true && GameManager.Instance.isKeyActive == false)
        {
            if (Input.GetKeyDown(KeyCode.F) && canPlaceKey == true)
            {
                GameManager.Instance.isKeyActive = true;
            }
        }
        if (GameManager.Instance.isKeyActive)
        {
            Move(); // Si la llave está activa, mueve el camión
        }
    }

    // Mueve el camión hacia la posición objetivo
    public void Move()
    {
        float d = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, d);
    }

    // Se llama cuando un objeto permanece en el área de colisión
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.allowPickup = true;

            if (GameManager.Instance.isKeyObtained == true)
            {
                canPlaceKey = true;
            }
        }
    }

    // Se llama cuando un objeto sale del área de colisión
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            canPlaceKey = false;
            GameManager.Instance.allowPickup = false;
        }
    }
}

