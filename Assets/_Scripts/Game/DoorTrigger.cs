using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
    Este script controla el comportamiento de un disparador de puerta que se activa cuando el jugador se acerca a él.
    Muestra un texto interactivo y permite al jugador abrir la puerta con una tecla específica si tiene una tarjeta clave.
*/

public class DoorTrigger : MonoBehaviour
{
    public bool triggerOn = false;
    public Animator doorAnim;

    private void Start()
    {
    }

    // Se llama cuando un objeto entra en el área de colisión del disparador
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(("Player")))
        {
            GameManager.Instance.interactText.GetComponent<Animator>().Play("interact_text_idle");
            if (other.GetComponent<PlayerController>().hasKeyCard.Equals(true))
            {
                triggerOn = true;
                GameManager.Instance.interactText.gameObject.SetActive(true);
                GameManager.Instance.interactText.gameObject.GetComponent<Animator>().Play("interact_text_idle");
                GameManager.Instance.interactText.text = ("Press \"F\" to insert Keycard");
            }
            else
            {
                GameManager.Instance.interactText.gameObject.SetActive(true);
                GameManager.Instance.interactText.gameObject.GetComponent<Animator>().Play("interact_text_idle");
                GameManager.Instance.interactText.text = ("You need a keycard to open this door");
            }
        }
    }

    // Se llama cuando un objeto sale del área de colisión del disparador
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(("Player")))
        {
            triggerOn = false;
            GameManager.Instance.interactText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // Verifica si se presiona la tecla correspondiente para abrir la puerta y si el disparador está activado
        if (Input.GetKeyDown(KeyCode.F) && triggerOn)
        {
            GameManager.Instance.interactText.gameObject.SetActive(false);
            doorAnim.SetTrigger("Fold");
            GetComponent<BoxCollider>().enabled = false;
        }
    }
}

