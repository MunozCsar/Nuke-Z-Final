using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_EntryCard : MonoBehaviour
{
    public bool hasKeyCard = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EntryCard"))
        {
            hasKeyCard = true;
            GameManager.Instance.interactText.gameObject.SetActive(true);
            GameManager.Instance.interactText.text = "Picked up a keycard";
            GameManager.Instance.interactText.GetComponent<Animator>().Play("interact_text_fadeout");
            Destroy(other.gameObject);
        }
    }
}
