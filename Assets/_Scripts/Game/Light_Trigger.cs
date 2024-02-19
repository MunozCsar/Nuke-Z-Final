using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light_Trigger : MonoBehaviour
{
    //Variable declaration
    public GameObject[] pointLights;

    public bool isActive;

    private void Start()
    {
        LightPower();
    }

    //Code block
    private void OnTriggerEnter(Collider other)
    {
        LightPower();
    }

    public void LightPower()
    {
        if (isActive)
        {
            foreach (var t in pointLights)
            {
                t.SetActive(false);
            }
        }
        else
        {
            foreach (var t in pointLights)
            {
                t.SetActive(true);
            }
        }
        isActive = !isActive;
    }
}
