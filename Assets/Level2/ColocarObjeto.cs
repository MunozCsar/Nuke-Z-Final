using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColocarObjeto : MonoBehaviour
{
    [SerializeField] GameObject[] piezas;
    public int piezasActivas = 0;
    public bool puedeColocar = false;
    public bool activarElectricidad;
    private void Update()
    {
        Colocar();
        ActivarElectricidad();
    }

    private void Colocar()
    {
        if (GameManager.Instance.tecla == true && puedeColocar == true && GameManager.Instance.recogido == true)
        {
            if (Input.GetKeyDown(KeyCode.F) && (GameManager.Instance.piezaSeleccionada == 0 || GameManager.Instance.piezaSeleccionada == 1 || GameManager.Instance.piezaSeleccionada == 2))
            {
                piezas[GameManager.Instance.piezaSeleccionada].SetActive(true);
                piezasActivas++;
                GameManager.Instance.piezaSeleccionada = 4;
                GameManager.Instance.recogido = false;
            }
        }
    }

    public void ActivarElectricidad()
    {
        if (activarElectricidad && Input.GetKeyDown(KeyCode.F))
        {
            foreach (GameObject light in GameManager.Instance.lights)
            {
                light.SetActive(true);
                GameManager.Instance.interactText.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.Instance.recogido == true)
        {
            Debug.Log("Presiona F para colocar");

            puedeColocar = true;
            GameManager.Instance.tecla = true;
        }
        else
        {
            Debug.Log("Faltan piezas para poder activar el generador");
        }

        if (piezasActivas == 3)
        {
            GameManager.Instance.interactText.gameObject.SetActive(true);
            GameManager.Instance.interactText.GetComponent<Animator>().Play("interact_text_idle");
            GameManager.Instance.interactText.text = "Press \"F\" to turn on electricity";
            activarElectricidad = true;


        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.Instance.recogido == true)
        {
            puedeColocar = false;
            GameManager.Instance.tecla = false;
            GameManager.Instance.interactText.gameObject.SetActive(false);
        }
    }
}