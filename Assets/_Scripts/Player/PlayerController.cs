using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;

    public float playerHP, regenSpeed;
    public Vector3 velocity;
    public float gravity = -9.81f;
    public WeaponHandler weaponHandler;
    public float walkSpeed, strafeSpeed;
    public float sprintMultiplier;
    public float jumpHeight;

    public float t, t_Goal, regenT, regenGoal; // variable 't' es el tiempo actual del timer, 't_Goal' es el objetivo del timer.

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public GameObject flashLight;
    private bool _isGrounded, flashOn;
    public bool hasKeyCard;
    private void Update()
    {
        if (regenT < regenGoal)
        {
            regenT += Time.deltaTime;
        }
        else
        {
            if (playerHP < 150)
            {
                RegenHP();
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Damage_Trigger"))
        {
            GameManager.Instance.DamageIndicator(playerHP);
            takeDamage(other.transform.root.GetComponent<ZM_AI>().zm_Hitpoints);
        }

        if (other.CompareTag("EntryCard"))
        {
            hasKeyCard = true;
            GameManager.Instance.interactText.gameObject.SetActive(true);
            GameManager.Instance.interactText.text = "Picked up a keycard";
            GameManager.Instance.interactText.GetComponent<Animator>().Play("interact_text_fadeout");
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Barrier_Trigger"))
        {
            if (other.transform.parent.GetComponent<BarrierLogic>().hitPoints < 9)
            {
                GameManager.Instance.interactText.gameObject.SetActive(true);
                GameManager.Instance.interactText.text = "Press 'F' to repair barrier";
                if (Input.GetKey(KeyCode.F))
                {
                    if (t < t_Goal)
                    {
                        t += Time.deltaTime;
                    }
                    else if (t >= t_Goal)
                    {
                        t = 0f;
                        other.transform.parent.GetComponent<BarrierLogic>().RepairBarrier();
                    }
                }

            }
            else if (other.transform.parent.GetComponent<BarrierLogic>().hitPoints >= 9)
            {
                GameManager.Instance.interactText.gameObject.SetActive(false);
            }
        }
    }

    public void takeDamage(float damage)
    {
        regenT = 0f;
        playerHP = playerHP - Mathf.RoundToInt(damage);
    }

    public void RegenHP()
    {
        playerHP += regenSpeed * Time.deltaTime;
    }
}
