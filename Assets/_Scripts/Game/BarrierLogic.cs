using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Este script gestiona la l�gica de las barreras defensivas en el juego.
    Controla el estado de salud de la barrera, la destrucci�n y reparaci�n de partes de la misma.
*/

public class BarrierLogic : MonoBehaviour
{
    public int hitPoints; // Puntos de resistencia de la barrera
    public GameObject[] planks; // Array de objetos de las partes de la barrera

    // Reduce los puntos de resistencia de la barrera y la destruye si alcanza cero puntos
    public void ReduceHitPoints()
    {
        hitPoints--;
        DestroyBarrier();
    }

    // Destruye la parte de la barrera correspondiente a los puntos de resistencia actuales
    public void DestroyBarrier()
    {
        planks[hitPoints].SetActive(false);
        if (hitPoints <= 0)
        {
            transform.GetChild(0).gameObject.SetActive(false); // Desactiva el objeto padre si la barrera est� completamente destruida
        }
    }

    // Repara la barrera, activando la parte destruida y aumentando los puntos de resistencia
    public void RepairBarrier()
    {
        if (hitPoints <= 0)
        {
            transform.GetChild(0).gameObject.SetActive(true); // Activa la barrera completa si estaba completamente destruida
        }
        planks[hitPoints].SetActive(true); // Activa la parte de la barrera correspondiente a los puntos de resistencia actuales
        hitPoints++; // Aumenta los puntos de resistencia
        GameManager.Instance.AddPoints(10); // A�ade puntos al jugador por reparar la barrera
    }
}

