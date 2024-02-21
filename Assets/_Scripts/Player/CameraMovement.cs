using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Camera cam;

    public float sensitivityX, clampMin, clampMax;
    public float sensitivityY;

    float xRotation;
    float yRotation;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensitivityX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensitivityY;

        yRotation += mouseX;

        xRotation += mouseY;


        transform.rotation = Quaternion.Euler(0, yRotation * 15, 0);
        cam.transform.rotation = Quaternion.Euler(-xRotation * 15, yRotation * 15, 0);
        //orientation.rotation = Quaternion.Euler(xRotation, 0, 0);
        xRotation = Mathf.Clamp(xRotation, clampMin, clampMax);

    }
}
