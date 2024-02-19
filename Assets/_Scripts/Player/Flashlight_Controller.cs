using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight_Controller : MonoBehaviour
{
    public GameObject flashLight;

    bool flashOn = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            flashOn = !flashOn;
        }

        if (flashOn)
        {
            flashLight.SetActive(true);
        }
        else
        {
            flashLight.SetActive(false);
        }
    }

}
