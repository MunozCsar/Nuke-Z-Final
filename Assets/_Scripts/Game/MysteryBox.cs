using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Este script controla el comportamiento de una caja misteriosa que ofrece armas aleatorias al jugador a cambio de puntos.
*/

public class MysteryBox : MonoBehaviour
{
    WeaponHandler weapon;
    public int[] weapons;
    public int maxUses = 1;
    public int currentUses = 0, randomWeapon;
    public bool boxActive;

    private void Update()
    {
        if (boxActive)
        {
            // Verifica si se presiona la tecla correspondiente para usar la caja
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (GameManager.Instance.score >= 950 && currentUses < maxUses)
                {
                    currentUses++;
                    RollWeapon(); // Genera un arma aleatoria
                }
            }
        }
    }

    // Se llama cuando el jugador entra en el �rea de colisi�n de la caja
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.interactText.gameObject.SetActive(true);
            GameManager.Instance.interactText.GetComponent<Animator>().Play("interact_text_idle");
            GameManager.Instance.interactText.text = "Press \"F\"to roll a random weapon (Cost: 950)";
            boxActive = true;
            weapon = other.GetComponent<WeaponHandler>();
        }
    }

    // Se llama cuando el jugador sale del �rea de colisi�n de la caja
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.interactText.gameObject.SetActive(false);
            boxActive = false;
        }
    }

    // Genera un arma aleatoria y la agrega al jugador
    void RollWeapon()
    {
        if (weapon.playerWeapons.Count == 1)
        {
            while (weapon.playerWeapons[0].GetComponentInChildren<GunController>().id.Equals(randomWeapon))
            {
                randomWeapon = weapons[Random.Range(0, weapons.Length)];
            }
        }
        else if (weapon.playerWeapons.Count == 2)
        {
            while (weapon.playerWeapons[0].GetComponentInChildren<GunController>().id.Equals(randomWeapon) ||
                   weapon.playerWeapons[1].GetComponentInChildren<GunController>().id.Equals(randomWeapon))
            {
                randomWeapon = weapons[Random.Range(0, weapons.Length)];
            }
        }
        else
        {
            randomWeapon = weapons[Random.Range(0, weapons.Length)];
        }

        GameManager.Instance.ReduceScore(950);
        GameManager.Instance.UpdateScoreText();
        weapon.AddWeapon(randomWeapon);
    }
}

