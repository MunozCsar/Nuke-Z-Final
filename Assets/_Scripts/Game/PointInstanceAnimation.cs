using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointInstanceAnimation : MonoBehaviour
{
    float rndY, rndX;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, .65f);
        rndY = Random.Range(-.75f, .75f);
        rndX = Random.Range(-1.25f, -.75f);
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.Translate(rndX, rndY, 0 * Time.deltaTime);
    }
}
