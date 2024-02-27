using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private bool _isGrounded, flashOn, end_Game; //Booleanos varios que almacenan si ele jugador está en el suelo, si la linterna está encendida y si el jugador puede acabar la partida
    public bool hasKeyCard; //Booleano que almacena si el jugador ha obtenido la keycard
    private void Update()
    {
        Agarrar();

        if (t_regen < regen_Cooldown) //Timer de la regeneración de vida: si el valor del cooldown es mayor que el valor del timer, se suma al timer Time.deltaTime
        {
            t_regen += Time.deltaTime;
        }
        else
        {
            if (playerHP < 150) //Si el jugador está por debajo de 150 de vida, se llama a la función RegenHP()
            {
                RegenHP();
            }

        }

        if (end_Game.Equals(true))
        {
            if (Input.GetKeyDown(KeyCode.F) && GameManager.Instance.endGameTrigger)
            {
                GameManager.Instance.EndGame();
            }
        }

        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (_isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
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

        }
        else if (Input.GetKey(KeyCode.S))
        {

            MovePlayer(strafeSpeed);

        }
        else if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {

            MovePlayer(strafeSpeed);

        }

        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

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
    private void MovePlayer(float speed)
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move.normalized * speed * Time.deltaTime);
    }

    private void Agarrar()
    {
        if (GameManager.Instance.tecla == true && GameManager.Instance.recogido == false)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if(GameManager.Instance.piezaSeleccionada == 3)
                {
                    GameManager.Instance.llaveRecogida = true;
                    GameManager.Instance.llaves.SetActive(false);
                }
                else if(GameManager.Instance.piezaSeleccionada == 0 || GameManager.Instance.piezaSeleccionada == 1 || GameManager.Instance.piezaSeleccionada == 2)
                {
                    GameManager.Instance.piezas[GameManager.Instance.piezaSeleccionada].SetActive(false);
                    GameManager.Instance.recogido = true;
                }

            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Damage_Trigger"))
        {
            GameManager.Instance.DamageIndicator(playerHP);
            takeDamage(GameManager.Instance.zm_Damage);
        }

        if (other.CompareTag("EntryCard"))
        {
            hasKeyCard = true;
            GameManager.Instance.interactText.gameObject.SetActive(true);
            GameManager.Instance.interactText.text = "Picked up a keycard";
            GameManager.Instance.interactText.GetComponent<Animator>().Play("interact_text_fadeout");
            Destroy(other.gameObject);
        }

        if (other.CompareTag("EndGameTrigger"))
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

        if (other.CompareTag("Llave"))
        {
            GameManager.Instance.tecla = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("EndGameTrigger"))
        {
            end_Game = false;
        }
        if (other.CompareTag("Llave"))
        {
            GameManager.Instance.tecla = false;
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

        if (other.CompareTag("Pieza1"))
        {
            Debug.Log("Presiona F para agarrar");

            GameManager.Instance.tecla = true;
            GameManager.Instance.piezaSeleccionada = 0;
            GameManager.Instance.colocar.puedeColocar = false;
        }
        else if (other.tag == "Pieza2" && GameManager.Instance.recogido == false && GameManager.Instance.llaveIsActive == true)
        {
            Debug.Log("Presiona F para agarrar");

            GameManager.Instance.tecla = true;
            GameManager.Instance.piezaSeleccionada = 1;
            GameManager.Instance.colocar.puedeColocar = false;
        }
        else if (other.tag == "Pieza3" && GameManager.Instance.recogido == false && GameManager.Instance.llaveIsActive == true)
        {
            Debug.Log("Presiona F para agarrar");

            GameManager.Instance.tecla = true;
            GameManager.Instance.piezaSeleccionada = 2;
            GameManager.Instance.colocar.puedeColocar = false;

        }
        else if (other.tag == "Llave" && GameManager.Instance.llaveRecogida == false)
        {
            Debug.Log("Presiona F para agarrar");

            GameManager.Instance.tecla = true;
            GameManager.Instance.piezaSeleccionada = 3;
        }
        if ((other.tag == "Pieza3" || other.tag == "Pieza2" || other.tag == "Pieza1" || other.tag == "Llave") && GameManager.Instance.recogido == true)
        {
            Debug.Log("Coloca la pieza antes de agarrar otra");
        }
    }

    public void takeDamage(float damage)
    {
        t_regen = 0f;
        playerHP = playerHP - Mathf.RoundToInt(damage);
        if(playerHP <= 0)
        {
            GameManager.Instance.GameOver(this.gameObject);
            GameManager.Instance.playerCam.GetComponent<Animator>().SetTrigger("onDeath");
        }
    }

    public void RegenHP()
    {
        playerHP += regenSpeed * Time.deltaTime;
    }
}
