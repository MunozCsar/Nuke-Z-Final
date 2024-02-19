using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light_Flicker : MonoBehaviour
{
    public float minIntensity;
    public float maxIntensity;

    float changeGoal;
    float changeAdd;

    public float changeTime = 0f;
    public Light lightSource;
    // Start is called before the first frame update
    void Start()
    {
        lightSource = GetComponent<Light>();
        changeGoal = Random.Range(2f, 5f);
        changeAdd = Random.Range(0.25f, 0.75f);
    }

    // Update is called once per frame
    void Update()
    {
        if (changeTime > changeGoal && GameManager.Instance.isPaused == false)
        {
            float value = Random.Range(minIntensity, maxIntensity);
            changeTime = 0f;
            lightSource.intensity = value;
        }

        changeTime += changeAdd;
    }
}
