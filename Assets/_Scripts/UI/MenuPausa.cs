using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuPausa : MonoBehaviour
{
    public GameObject menuPausa;
    public GameObject canvas;
    private bool estaPausado = false;
    private void Start()
    {
        menuPausa.SetActive(false);
        canvas.SetActive(true);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (estaPausado)
            {
                ReanudarJuego();
            }
            else
            {
                PausarJuego();
            }
        }
    }

    public void PausarJuego()
    {
        Time.timeScale = 0f;
        menuPausa.SetActive(true);
        canvas.SetActive(false);
        estaPausado = true;
    }

    public void ReanudarJuego()
    {
        Time.timeScale = 1f;
        menuPausa.SetActive(false);
        canvas.SetActive(true);
        estaPausado = false;
    }
    public void SalirJuego()
    {
        SceneManager.LoadScene(0);
        Application.Quit();
    }
}
