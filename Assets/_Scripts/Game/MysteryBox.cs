using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Este script controla el comportamiento de una caja misteriosa que ofrece armas aleatorias al jugador a cambio de puntos.
*/

public class MysteryBox : MonoBehaviour
{
    WeaponHandler weapon; // Referencia al controlador de armas del jugador
    public int[] weapons; // Array de ID de armas disponibles en la caja
    public int maxUses = 1; // Número máximo de usos de la caja
    public int currentUses = 0, randomWeapon; // Número actual de usos y arma aleatoria seleccionada
    public bool boxActive; // Estado de activación de la caja

    // Se llama en cada frame
    private void Update()
    {
        if (boxActive)
        {
            // Verifica si se presiona la tecla correspondiente para usar la caja
            if (Input.GetKeyDown(KeyCode.F))
            {
                // Verifica si el jugador tiene suficientes puntos y no ha alcanzado el límite de usos
                if (GameManager.Instance.score >= 950 && currentUses < maxUses)
                {
                    currentUses++;
                    RollWeapon(); // Genera un arma aleatoria
                }
            }
        }
    }

    // Se llama cuando el jugador entra en el área de colisión de la caja
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Muestra un mensaje de interacción y activa la caja
            GameManager.Instance.interactText.gameObject.SetActive(true);
            GameManager.Instance.interactText.GetComponent<Animator>().Play("interact_text_idle");
            GameManager.Instance.interactText.text = "Presiona \"F\" para obtener un arma aleatoria (Costo: 950)";
            boxActive = true;
            weapon = other.GetComponent<WeaponHandler>(); // Obtiene el controlador de armas del jugador
        }
    }

    // Se llama cuando el jugador sale del área de colisión de la caja
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Oculta el mensaje de interacción y desactiva la caja
            GameManager.Instance.interactText.gameObject.SetActive(false);
            boxActive = false;
        }
    }

    // Genera un arma aleatoria y la agrega al jugador
    void RollWeapon()
    {
        // Verifica cuántas armas tiene el jugador equipadas
        if (weapon.playerWeapons.Count == 1)
        {
            // Si el jugador tiene una sola arma, asegura que la nueva arma no sea la misma
            while (weapon.playerWeapons[0].GetComponentInChildren<GunController>().id.Equals(randomWeapon))
            {
                randomWeapon = weapons[Random.Range(0, weapons.Length)];
            }
        }
        else if (weapon.playerWeapons.Count == 2)
        {
            // Si el jugador tiene dos armas, asegura que la nueva arma no sea ninguna de las dos
            while (weapon.playerWeapons[0].GetComponentInChildren<GunController>().id.Equals(randomWeapon) ||
                   weapon.playerWeapons[1].GetComponentInChildren<GunController>().id.Equals(randomWeapon))
            {
                randomWeapon = weapons[Random.Range(0, weapons.Length)];
            }
        }
        else
        {
            // Si el jugador no tiene armas o tiene más de dos, elige aleatoriamente una nueva arma
            randomWeapon = weapons[Random.Range(0, weapons.Length)];
        }

        // Reduce los puntos del jugador y actualiza el texto de los puntos
        GameManager.Instance.ReduceScore(950);
        GameManager.Instance.UpdateScoreText();

        // Agrega el arma seleccionada al jugador
        weapon.AddWeapon(randomWeapon);
    }
}

