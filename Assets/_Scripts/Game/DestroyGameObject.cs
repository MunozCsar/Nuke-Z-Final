using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Este script destruye el GameObject al que est� adjunto despu�s de un cierto per�odo de tiempo.
*/

public class DestroyGameObject : MonoBehaviour
{
    public int time;

    void Start()
    {
        // Destruye el GameObject despu�s del tiempo especificado
        Destroy(gameObject, time);
    }
}

