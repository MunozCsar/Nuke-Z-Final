using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToMenu : MonoBehaviour
{
    // Vuelve al men� principal pasados los segundos indicados
    void Start()
    {
        StartCoroutine(GameManager.Instance.ReturnToMenu(45f));
    }

}
