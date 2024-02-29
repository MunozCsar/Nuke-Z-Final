using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Este script controla el movimiento de la cámara en el juego, permitiendo al jugador mirar alrededor con el ratón.
*/

public class CameraMovement : MonoBehaviour
{
    public Camera playerCam;

    public float sensitivityX, clampMin, clampMax;
    public float sensitivityY;
    public int fps;
    float xRotation;
    float yRotation;

    private void Awake()
    {
        Application.targetFrameRate = 0;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // Establece la rotación inicial
        xRotation = transform.eulerAngles.x;
        yRotation = transform.eulerAngles.y;
    }

    void LateUpdate()
    {

        // Calcula los movimientos del ratón en los ejes X e Y
        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivityY;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivityX;

        // Actualiza la rotación horizontal
        yRotation += mouseX;
        // Actualiza la rotación vertical
        xRotation += mouseY;

        // Aplica las rotaciones a los objetos controlados por la cámara, con restricciones en el eje vertical
        transform.rotation = Quaternion.Euler(0, yRotation * sensitivityY, 0);
        playerCam.transform.rotation = Quaternion.Euler(-xRotation * sensitivityX, yRotation * sensitivityY, 0);
        xRotation = Mathf.Clamp(xRotation, clampMin, clampMax); // Limita la rotación vertical dentro de los límites especificados
    }
}

