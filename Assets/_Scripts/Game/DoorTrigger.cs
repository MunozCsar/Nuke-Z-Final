using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Serialization;

public class DoorTrigger : MonoBehaviour
{

    [SerializeField] private GameObject doorText;
    public bool triggerOn = false;
    public Animator doorAnim;

    private void Start()
    {
        doorText.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(("Player")))
        {
            GameManager.Instance.interactText.GetComponent<Animator>().Play("interact_text_idle");
            if (other.GetComponent<PlayerController>().hasKeyCard.Equals(true))
            {
                triggerOn = true;
                doorText.SetActive(true);
                doorText.GetComponent<TMP_Text>().text = ("Press \"F\" to insert Keycard");
            }
            else
            {
                doorText.SetActive(true);
                doorText.GetComponent<TMP_Text>().text = ("You need a Keycard to open this door");
            }


        }


    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(("Player")))
        {
            triggerOn = false;
            doorText.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && triggerOn)
        {
            doorText.SetActive(false);
            doorAnim.SetTrigger("Fold");
            GetComponent<BoxCollider>().enabled = false;
        }
    }
}
