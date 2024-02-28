using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
    Este script controla el comportamiento de un disparador de puerta que se activa cuando el jugador se acerca a �l.
    Muestra un texto interactivo y permite al jugador abrir la puerta con una tecla espec�fica si tiene una tarjeta clave.
*/

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private GameObject doorText; // Texto de la puerta para interacci�n
    public bool triggerOn = false; // Estado de activaci�n del disparador
    public Animator doorAnim; // Animator de la puerta

    // Se llama al inicio
    private void Start()
    {
        doorText.SetActive(false); // Desactiva el texto de la puerta al inicio
    }

    // Se llama cuando un objeto entra en el �rea de colisi�n del disparador
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(("Player")))
        {
            // Muestra el texto de interacci�n y la acci�n necesaria para abrir la puerta
            GameManager.Instance.interactText.GetComponent<Animator>().Play("interact_text_idle");
            if (other.GetComponent<PlayerController>().hasKeyCard.Equals(true))
            {
                triggerOn = true;
                doorText.SetActive(true);
                doorText.GetComponent<TMP_Text>().text = ("Presiona \"F\" para insertar la tarjeta clave");
            }
            else
            {
                // Muestra un mensaje indicando que se necesita una tarjeta clave para abrir la puerta
                doorText.SetActive(true);
                doorText.GetComponent<TMP_Text>().text = ("Necesitas una tarjeta clave para abrir esta puerta");
            }
        }
    }

    // Se llama cuando un objeto sale del �rea de colisi�n del disparador
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(("Player")))
        {
            // Desactiva el estado de activaci�n y oculta el texto de la puerta
            triggerOn = false;
            doorText.SetActive(false);
        }
    }

    // Se llama en cada frame
    private void Update()
    {
        // Verifica si se presiona la tecla correspondiente para abrir la puerta y si el disparador est� activado
        if (Input.GetKeyDown(KeyCode.F) && triggerOn)
        {
            doorText.SetActive(false); // Oculta el texto de la puerta
            doorAnim.SetTrigger("Fold"); // Activa la animaci�n de la puerta para abrirla
            GetComponent<BoxCollider>().enabled = false; // Desactiva el collider del disparador para evitar que se active nuevamente
        }
    }
}

