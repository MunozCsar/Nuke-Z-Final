using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColocarObjeto : MonoBehaviour
{
    [SerializeField] GameObject[] parts;
    public int activeParts = 0;
    public bool turnOnPower, powerOn;
    private void Update()
    {
        Colocar();
        ActivarElectricidad();
    }

    private void Colocar()
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

    public void ActivarElectricidad()
    {
        if (turnOnPower && Input.GetKeyDown(KeyCode.F))
        {
            foreach (GameObject light in GameManager.Instance.lights)
            {
                light.SetActive(true);
                GameManager.Instance.interactText.gameObject.SetActive(false);
            }
            GameManager.Instance.lightMaterial.SetColor("_EmissionColor", Color.white); //Al activar la electricidad, activa la propiedad de emission del material de las luces
            GameManager.Instance.electricDoor.GetComponent<Animator>().SetTrigger("Fold");
            turnOnPower = false;
            powerOn = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.Instance.obtainedPickup == true)
        {
            GameManager.Instance.placeablePart = true;
            GameManager.Instance.allowPickup = true;
        }
        if (activeParts == 3 && powerOn == false)
        {
            GameManager.Instance.interactText.gameObject.SetActive(true);
            GameManager.Instance.interactText.GetComponent<Animator>().Play("interact_text_idle");
            GameManager.Instance.interactText.text = "Press \"F\" to turn on electricity";
            turnOnPower = true;


        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.Instance.obtainedPickup == true)
        {
            GameManager.Instance.placeablePart = false;
            GameManager.Instance.allowPickup = false;
            GameManager.Instance.interactText.gameObject.SetActive(false);
        }
    }
}