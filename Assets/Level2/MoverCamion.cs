using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverCamion : MonoBehaviour
{
    public PlayerController player;
    public Animator animator;
    public bool puedeColocarLlave = false;
    public BoxCollider boxCollider;
    public Transform target;
    public float speed;

    private void Start()
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider>();
    }
    private void Update()
    {
        Colocar();
    }

    private void Colocar()
    {
        if (GameManager.Instance.llaveRecogida == true && GameManager.Instance.tecla == true)
        {
            if (Input.GetKeyDown(KeyCode.F) && puedeColocarLlave == true)
            {
                Debug.Log("Mover");
                GameManager.Instance.llaveIsActive = true;
                GameManager.Instance.llaveRecogida = false;
                boxCollider.enabled = false;
            }
        }
        if (GameManager.Instance.llaveIsActive)
        {
            MoveCamion();
        }
    }

    public void MoveCamion()
    {
        float d = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, d);
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Presiona F para colocar");
            GameManager.Instance.tecla = true;

            if (GameManager.Instance.llaveRecogida == true)
            {
                puedeColocarLlave = true;
            }
            else
            {
                Debug.Log("Necesitas una llave para mover el camion");
            }
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            puedeColocarLlave = false;
            GameManager.Instance.tecla = false;
        }
    }
}
