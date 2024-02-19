using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxAmmo : MonoBehaviour
{

    public Vector3 rotDir;
    public float rotSpeed, lifeTime;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotDir * rotSpeed * Time.deltaTime);
    }
}
