using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryBox : MonoBehaviour
{
    WeaponHandler weapon;
    public int[] weapons;
    public int maxUses = 1;
    public int currentUses = 0, randomWeapon;
    public bool boxActive;
    public Animator boxAnimator;

    private void Update()
    {

        if (boxActive)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (GameManager.Instance.score >= 950 && currentUses < maxUses)
                {
                    boxActive = false;
                    currentUses++;
                    //boxAnimator.Play("tapa2");
                    RollWeapon();
                }
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.interactText.gameObject.SetActive(true);
            GameManager.Instance.interactText.GetComponent<Animator>().Play("interact_text_idle");
            GameManager.Instance.interactText.text = "Press \"F\" to roll a random weapon (Cost: 950)";
            boxActive = true;
            weapon = other.GetComponent<WeaponHandler>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.interactText.gameObject.SetActive(false);
            boxActive = false;
        }
    }

    void RollWeapon()
    {

        if(weapon.playerWeapons.Count == 0)
        {
            randomWeapon = weapons[Random.Range(0, weapons.Length)];
        }

        else if(weapon.playerWeapons.Count == 1)
        {

            while (weapon.playerWeapons[0].GetComponentInChildren<GunShooting>().id.Equals(randomWeapon))
            {
                randomWeapon = weapons[Random.Range(0, weapons.Length)];
                GameManager.Instance.ReduceScore(950);
                GameManager.Instance.UpdateScoreText();
            }
        }
        else if (weapon.playerWeapons.Count == 2)
        {
            while (weapon.playerWeapons[0].GetComponentInChildren<GunShooting>().id.Equals(randomWeapon) || weapon.playerWeapons[1].GetComponentInChildren<GunShooting>().id.Equals(randomWeapon))
            {
                randomWeapon = weapons[Random.Range(0, weapons.Length)];
                GameManager.Instance.ReduceScore(950);
                GameManager.Instance.UpdateScoreText();
            }
        }

        weapon.AddWeapon(randomWeapon);
    }

}
