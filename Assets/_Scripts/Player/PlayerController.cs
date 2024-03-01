using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Este script controla el movimiento, vida y varias interacciones del jugador con el juego y su entorno.
*/

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;

    public float playerHP, maxHP, regenSpeed;
    Vector3 velocity;
    public float gravity = -9.81f;
    public float walkSpeed, strafeSpeed;
    public float sprintMultiplier;
    public float jumpHeight;

    public float t_barrier, barrier_Cooldown, t_regen, regen_Cooldown;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public GameObject flashLight;
    private bool isGrounded, flashOn, end_Game;
    public bool hasKeyCard;
    private void Update()
    {
        PickUp();
        CheckGround();
        ApplyGravity();
        RegenHP();
        TurnOnFlashLight();
        if (end_Game.Equals(true))
        {
            if (Input.GetKeyDown(KeyCode.F) && GameManager.Instance.endGameTrigger)
            {
                GameManager.Instance.EndGame();
            }
        } //Si el jugador está en el trigger del boton, presiona F y la variable de endGameTrigger es true acaba la partida

        if (Input.GetKey(KeyCode.W))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                MovePlayer(walkSpeed * sprintMultiplier);
            }
            else
            {
                MovePlayer(walkSpeed);
            }

        } //Movimiento hacia delante
        else if (Input.GetKey(KeyCode.S))
        {

            MovePlayer(strafeSpeed);

        } //Movimiento hacia atras
        else if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {

            MovePlayer(strafeSpeed);

        } //Movimiento lateral

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        } //Salto

        controller.Move(velocity * Time.deltaTime);

    }

    private void TurnOnFlashLight()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            flashOn = !flashOn;
        }
        if (flashOn)
        {
            flashLight.SetActive(true);
        }
        else
        {
            flashLight.SetActive(false);
        }
    }

    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask); //Se comprueba si el jugador está en tierra usando un CheckSphere
    }
    private void ApplyGravity()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -5;
        }
        velocity.y += gravity * Time.deltaTime;
    }
    //Función de movimiento del jugador
    private void MovePlayer(float speed)
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(speed * Time.deltaTime * move.normalized);
    }

    private void PickUp()
    {
        //Gestiona la interaccion con la llave y las piezas

        if (GameManager.Instance.allowPickup == true && GameManager.Instance.obtainedPickup == false) 
        {
            if (Input.GetKeyDown(KeyCode.F)) //Al presionar F, si la parte seleccionada es la llave, da el valor true al booleano y desactivar el GO de la llave
            {
                if(GameManager.Instance.selectedPart == 3)
                {
                    GameManager.Instance.isKeyObtained = true;
                    GameManager.Instance.truckKey.SetActive(false);
                }
                else if(GameManager.Instance.selectedPart == 0 || GameManager.Instance.selectedPart == 1 || GameManager.Instance.selectedPart == 2) //Si selecciona una pieza de electricidad, obtiene el pickup y la desactiva
                {
                    GameManager.Instance.powerParts[GameManager.Instance.selectedPart].SetActive(false);
                    GameManager.Instance.obtainedPickup = true;
                }

            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Gestión de triggers para activar y desactivar eventos interactivos

        if (other.CompareTag("Damage_Trigger")) //Recibe daño al ser golpeado
        {
            GameManager.Instance.ShowDamageIndicators(playerHP);
            TakeDamage(GameManager.Instance.zm_Damage);
        }

        if (other.CompareTag("EntryCard")) //Obtención de keycard
        {
            hasKeyCard = true;
            GameManager.Instance.interactText.gameObject.SetActive(true);
            GameManager.Instance.interactText.text = "Picked up a keycard";
            GameManager.Instance.interactText.GetComponent<Animator>().Play("interact_text_fadeout");
            Destroy(other.gameObject);
        }

        if (other.CompareTag("EndGameTrigger")) //Comprobación de poder acabar la partida
        {
            GameManager.Instance.interactText.gameObject.SetActive(true);
            GameManager.Instance.interactText.GetComponent<Animator>().Play("interact_text_idle");
            if (GameManager.Instance.endGameTrigger)
            {
                GameManager.Instance.interactText.text = "Press \"F\" to turn on reactor";
                end_Game = true;
            }
            else
            {
                GameManager.Instance.interactText.text = "Fill up the containers to turn on the reactor";
            }


        }

        if (other.CompareTag("Llave")) //Comprueba que el jugador esté tocando la llave
        {
            GameManager.Instance.allowPickup = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Desactiva las bools al dejar de hace contacto con ellas

        if (other.CompareTag("EndGameTrigger"))
        {
            end_Game = false;
        }
        if (other.CompareTag("Llave"))
        {
            GameManager.Instance.allowPickup = false;
        }
    }

    // Gestión de triggers de tipo stay
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Barrier_Trigger"))
        {
            if (other.transform.parent.GetComponent<BarrierLogic>().hitPoints < 9)
            {
                GameManager.Instance.interactText.gameObject.SetActive(true);
                GameManager.Instance.interactText.GetComponent<Animator>().Play("interact_text_idle");
                GameManager.Instance.interactText.text = "Press \"F\" to repair barrier";
                if (Input.GetKey(KeyCode.F))
                {
                    if (t_barrier < barrier_Cooldown)
                    {
                        t_barrier += Time.deltaTime;
                    }
                    else if (t_barrier >= barrier_Cooldown)
                    {
                        t_barrier = 0f;
                        other.transform.parent.GetComponent<BarrierLogic>().RepairBarrier();
                    }
                }

            }
            else if (other.transform.parent.GetComponent<BarrierLogic>().hitPoints >= 9)
            {
                GameManager.Instance.interactText.gameObject.SetActive(false);
            }
        }

        if (other.CompareTag("Pieza1") && GameManager.Instance.obtainedPickup == false && GameManager.Instance.isKeyObtained == true) //Comprueba si esta tocando la pieza 1 y asigna los valores debidos
        {
            GameManager.Instance.allowPickup = true;
            GameManager.Instance.selectedPart = 0;
        }
        else if (other.CompareTag("Pieza2") && GameManager.Instance.obtainedPickup == false && GameManager.Instance.isKeyObtained == true) //Comprueba si esta tocando la pieza 2 y asigna los valores debidos
        {
            GameManager.Instance.allowPickup = true;
            GameManager.Instance.selectedPart = 1;
        }
        else if (other.CompareTag("Pieza3") && GameManager.Instance.obtainedPickup == false && GameManager.Instance.isKeyObtained == true) //Comprueba si esta tocando la pieza 3 y asigna los valores debidos
        {
            GameManager.Instance.allowPickup = true;
            GameManager.Instance.selectedPart = 2;

        }
        if (other.CompareTag("Llave") && GameManager.Instance.isKeyObtained == false) //Comprueba si esta tocando la llave y asigna los valores debidos
        {
            GameManager.Instance.allowPickup = true;
            GameManager.Instance.selectedPart = 3;
        }
    }

    public void TakeDamage(float damage) //Recibir daño
    {
        t_regen = 0f;
        playerHP -= Mathf.RoundToInt(damage);
        if(playerHP <= 0)
        {
            GameManager.Instance.GameOver(this.gameObject);
            GameManager.Instance.playerCam.GetComponent<Animator>().SetTrigger("onDeath");
        }
    }

    public void RegenHP() //Regeneración de vida
    {
        if (t_regen < regen_Cooldown)
        {
            t_regen += Time.deltaTime;
        }
        else
        {
            if (playerHP < maxHP) //Si el jugador está por debajo del valor máximo de vida, se regenera la vida()
            {
                playerHP += regenSpeed * Time.deltaTime;
            }

        }

    }
}
