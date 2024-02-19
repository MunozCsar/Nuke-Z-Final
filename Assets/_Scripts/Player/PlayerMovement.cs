using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController controller;

    public float playerHP, regenSpeed;
    public Vector3 velocity;
    public float gravity = -9.81f;
    public WeaponHandler weaponHandler;
    private float _speed;
    public float sprintSpeed;
    public float defaultSpeed;
    public float jumpHeight;

    public float t, t_Goal, regenT, regenGoal; // variable 't' es el tiempo actual del timer, 't_Goal' es el objetivo del timer.

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private bool _isGrounded;
    private void Update()
    {
        if(regenT < regenGoal)
        {
            regenT += Time.deltaTime;
        }
        else
        {
            if(playerHP < 150)
            {
                RegenHP();
            }

        }


        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(_isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        else
        {
            _speed = 5f;
        }

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        if (Input.GetKey(KeyCode.W))
        {
            controller.Move(move.normalized * _speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            controller.Move(move.normalized * _speed / 1.5f * Time.deltaTime);
        }
        else
        {
            controller.Move(move.normalized * _speed / 0.75f * Time.deltaTime);
        }


        Sprint();

        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    private void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _speed = sprintSpeed;
        }
        else
        {
            _speed = defaultSpeed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Damage_Trigger"))
        {
            GameManager.Instance.DamageIndicator(playerHP);
            takeDamage(other.transform.root.GetComponent<ZM_AI>().zm_Hitpoints);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Barrier_Trigger"))
        {
            GameManager.Instance.interactText.text = "Press 'F' to repair barrier";
            if (Input.GetKey(KeyCode.F))
            {
                if (t < t_Goal)
                {
                    t += Time.deltaTime;
                }
                else if (t >= t_Goal && other.GetComponent<BarrierLogic>().hitPoints < 10)
                {
                    t = 0f;
                    other.GetComponent<BarrierLogic>().RepairBarrier();
                }
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
