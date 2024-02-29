using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Este script controla el movimiento, vida y varias interacciones del jugador con el juego y su entorno.
*/

public class PlayerController : MonoBehaviour
{
    public CharacterController controller; //Toma referencia del componente "CharacterController"

    public float playerHP, regenSpeed; //Variables de vida y regeneración de vida
    Vector3 velocity; //Control de la dirección de la gravedad
    public float gravity = -9.81f; //Valor de la gravedad
    public float walkSpeed, strafeSpeed; //Velocidad de movimiento hacia delante y hacia los lados
    public float sprintMultiplier; //Multiplicador de sprint
    public float jumpHeight; //Fuerza de salto

    public float t_barrier, barrier_Cooldown, t_regen, regen_Cooldown; // Timers para la reparación de la barrera y la regeneración de vida

    public Transform groundCheck; //Transform usado para la detección de suelo
    public float groundDistance = 0.4f; //Radio de la esfera de detección
    public LayerMask groundMask; //Máscara del suelo
    public GameObject flashLight; //Gameobject linterna
    private bool isGrounded, flashOn, end_Game; //Booleanos varios que almacenan si ele jugador está en el suelo, si la linterna está encendida y si el jugador puede acabar la partida
    public bool hasKeyCard; //Booleano que almacena si el jugador ha obtenido la keycard
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

        controller.Move(velocity * Time.deltaTime); //Se usa el componente CharacterController para hacer saltar al personaje 

    }

    private void TurnOnFlashLight()
    {
        if (Input.GetKeyDown(KeyCode.Q)) //Al presionar la tecla Q se invierte el bool de la linterna
        {
            flashOn = !flashOn;
        }
        if (flashOn) //Si el bool es true se activa la linterna
        {
            flashLight.SetActive(true);
        }
        else //Si el bool es false se desactiva la linterna
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
        if (isGrounded && velocity.y < 0) //Si el jugador está en tierra y velocity.y es menor que 0, se restea velocity.y a 0
        {
            velocity.y = -2;
        }
        velocity.y += gravity * Time.deltaTime; //Se le da a velocity.y el valor de la gravedad por Time.deltaTime
    }
    private void MovePlayer(float speed)
    {
        float x = Input.GetAxisRaw("Horizontal"); //Se toma la dirección horizontal
        float z = Input.GetAxisRaw("Vertical"); //Se toma la dirección vertical

        Vector3 move = transform.right * x + transform.forward * z; //Se crea la variable local move y multiplicamos los valores anteriores por los ejes x y z
        controller.Move(speed * Time.deltaTime * move.normalized); //Se llama a la funcion Move del CharacterController y se asigna la velocidad.
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
        t_regen = 0f; //Resetea el timer de regeneración a 0
        playerHP -= Mathf.RoundToInt(damage); //Reduce la vida del jugador
        if(playerHP <= 0)
        {
            GameManager.Instance.GameOver(this.gameObject);
            GameManager.Instance.playerCam.GetComponent<Animator>().SetTrigger("onDeath");
        } //Si la vida del jugador es igual o menor a 0, llama a la función de muerte y activa la animación de muerte
    }

    public void RegenHP()
    {
        if (t_regen < regen_Cooldown) //Timer de la regeneración de vida: si el valor del cooldown es mayor que el valor del timer, se suma al timer Time.deltaTime
        {
            t_regen += Time.deltaTime;
        }
        else
        {
            if (playerHP < 150) //Si el jugador está por debajo de 150 de vida, se regenera la vida()
            {
                playerHP += regenSpeed * Time.deltaTime;
            }

        }

    }
}
