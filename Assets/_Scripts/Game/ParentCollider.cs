using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentCollider : MonoBehaviour
{

    public GameObject bone;

    // Actualiza la posici�n del GO a la posici�n del hueso indicado
    void Update()
    {
        gameObject.transform.rotation = bone.transform.rotation;
        gameObject.transform.position = bone.transform.position;
    }
}
