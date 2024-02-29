using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Este script gestiona las partes de la electricidad y la activaci�n de la energ�a el�ctrica en el nivel.
*/

public class PartManager : MonoBehaviour
{
    [SerializeField] GameObject[] parts;
    public int activeParts = 0;
    public bool turnOnPower, powerOn;

    // Se llama en cada frame
    private void Update()
    {
        PlaceObject();
        ActivatePower();
    }

    // Coloca la parte seleccionada si las condiciones son correctas
    private void PlaceObject()
    {
        if (GameManager.Instance.allowPickup == true && GameManager.Instance.placeablePart == true && GameManager.Instance.obtainedPickup == true)
        {
            if (Input.GetKeyDown(KeyCode.F) && (GameManager.Instance.selectedPart == 0 || GameManager.Instance.selectedPart == 1 || GameManager.Instance.selectedPart == 2))
            {
                parts[GameManager.Instance.selectedPart].SetActive(true);
                activeParts++;
                GameManager.Instance.selectedPart = 4;
                GameManager.Instance.obtainedPickup = false;
                GameManager.Instance.placeablePart = false;
            }
        }
    }

    // Activa la energ�a el�ctrica si se cumple la condici�n y se presiona la tecla correspondiente
    public void ActivatePower()
    {
        if (turnOnPower && Input.GetKeyDown(KeyCode.F))
        {
            foreach (GameObject light in GameManager.Instance.lights)
            {
                light.SetActive(true);
                GameManager.Instance.interactText.gameObject.SetActive(false);
            }
            Color32 emissionColor = new Color(1f, .875f, .65f);
            GameManager.Instance.lightMaterial.SetColor("_EmissionColor", emissionColor) ; // Al activar la electricidad, activa la propiedad de emission del material de las luces
            GameManager.Instance.electricDoor.GetComponent<Animator>().SetTrigger("Fold");
            turnOnPower = false;
            powerOn = true;
        }
    }

    // Se llama cuando un objeto permanece en el �rea de colisi�n
    private void OnTriggerStay(Collider other)
    {
        // Si detecta al jugador y tiene un pickup le permite colocar la parte en su lugar
        if (other.CompareTag("Player") && GameManager.Instance.obtainedPickup == true)
        {
            GameManager.Instance.placeablePart = true;
            GameManager.Instance.allowPickup = true;
        }
        // Si todas las partes est�n colocadas y la energ�a no est� activa, permite activar la energ�a
        if (activeParts == 3 && powerOn == false)
        {
            GameManager.Instance.interactText.gameObject.SetActive(true);
            GameManager.Instance.interactText.GetComponent<Animator>().Play("interact_text_idle");
            GameManager.Instance.interactText.text = "Presiona \"F\" para encender la electricidad";
            turnOnPower = true;
        }
    }

    // Se llama cuando un objeto sale del �rea de colisi�n
    private void OnTriggerExit(Collider other)
    {
        // Si el jugador sale del �rea no le permite colocar la parte y desactiva el texto de interacci�n
        if (other.CompareTag("Player") && GameManager.Instance.obtainedPickup == true)
        {
            GameManager.Instance.placeablePart = false;
            GameManager.Instance.allowPickup = false;
            GameManager.Instance.interactText.gameObject.SetActive(false);
        }
    }
}
