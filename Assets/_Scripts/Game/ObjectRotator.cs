using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotator : MonoBehaviour
{

    public Vector3 rotDir;
    public float rotSpeed, lifeTime;
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Rotaci�n del objeto en la direcci�n y a la velocidad especificadas
    void Update()
    {
        transform.Rotate(rotDir * rotSpeed * Time.deltaTime);
    }
}
