using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Este script gestiona el movimiento del cami�n cuando se coloca la llave.
*/

public class MoveTruck : MonoBehaviour
{
    public PlayerController player; // Referencia al controlador del jugador
    public bool canPlaceKey = false; // Indica si el cami�n puede recibir la llave
    public Transform target; // Posici�n a la que se mueve el cami�n
    public float speed; // Velocidad de movimiento del cami�n

    // Se llama en cada frame
    private void Update()
    {
        UseKey(); // Llama a la funci�n para utilizar la llave
    }

    // Utiliza la llave para mover el cami�n si se cumplen las condiciones
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
            Move(); // Si la llave est� activa, mueve el cami�n
        }
    }

    // Mueve el cami�n hacia la posici�n objetivo
    public void Move()
    {
        float d = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, d);
    }

    // Se llama cuando un objeto permanece en el �rea de colisi�n
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

    // Se llama cuando un objeto sale del �rea de colisi�n
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            canPlaceKey = false;
            GameManager.Instance.allowPickup = false;
        }
    }
}

