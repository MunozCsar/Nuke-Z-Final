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

    float xRotation;
    float yRotation;

    private void Awake()
    {
        // Establece la rotación inicial de la cámara y el objeto controlado por la cámara
        transform.rotation = Quaternion.Euler(0, 180, 0);
        playerCam.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    void LateUpdate()
    {
        // Calcula los movimientos del ratón en los ejes X e Y
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensitivityX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensitivityY;

        // Actualiza la rotación horizontal
        yRotation += mouseX;
        // Actualiza la rotación vertical
        xRotation += mouseY;

        // Aplica las rotaciones a los objetos controlados por la cámara, con restricciones en el eje vertical
        transform.rotation = Quaternion.Euler(0, yRotation * 15, 0);
        playerCam.transform.rotation = Quaternion.Euler(-xRotation * 15, yRotation * 15, 0);
        xRotation = Mathf.Clamp(xRotation, clampMin, clampMax); // Limita la rotación vertical dentro de los límites especificados
    }
}

