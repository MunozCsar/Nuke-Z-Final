using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Este script controla el movimiento de la c�mara en el juego, permitiendo al jugador mirar alrededor con el rat�n.
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
        // Establece la rotaci�n inicial de la c�mara y el objeto controlado por la c�mara
        transform.rotation = Quaternion.Euler(0, 180, 0);
        playerCam.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    void LateUpdate()
    {
        // Calcula los movimientos del rat�n en los ejes X e Y
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensitivityX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensitivityY;

        // Actualiza la rotaci�n horizontal
        yRotation += mouseX;
        // Actualiza la rotaci�n vertical
        xRotation += mouseY;

        // Aplica las rotaciones a los objetos controlados por la c�mara, con restricciones en el eje vertical
        transform.rotation = Quaternion.Euler(0, yRotation * 15, 0);
        playerCam.transform.rotation = Quaternion.Euler(-xRotation * 15, yRotation * 15, 0);
        xRotation = Mathf.Clamp(xRotation, clampMin, clampMax); // Limita la rotaci�n vertical dentro de los l�mites especificados
    }
}

