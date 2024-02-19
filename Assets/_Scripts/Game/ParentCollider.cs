using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentCollider : MonoBehaviour
{

    public GameObject bone;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.rotation = bone.transform.rotation;
        this.gameObject.transform.position = bone.transform.position;
    }
}
