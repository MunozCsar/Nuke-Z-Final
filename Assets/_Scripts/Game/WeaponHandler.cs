using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

/*
    Este script gestiona las armas del jugador y varias interacciones con diferentes powerups y objetos interactuables, así como el ataque cuerpo a cuerpo.
*/

public class WeaponHandler : MonoBehaviour
{
    public LayerMask enemyMask;

    [Header("Melee")]
    public GameObject knife, pickUpWeaponGameObject;
    public float meleeDamage, meleeRange; 
    public float t_melee, melee_coolDown;

    [Header("Weapons")]
    public Transform gunInstancePosition;
    public List<GameObject> playerWeapons = new();
    public TMP_Text ammoCount, maxAmmoCount;
    bool pickupWeapon = false, activeWeapon, ammoBox;
    public int activeSlot, weaponSlots;
    int weaponID;

    void Start()
    {
        knife.SetActive(false);
    }

    void Update()
    {
        if(t_melee < melee_coolDown)
        {
            t_melee += Time.deltaTime;
        }


        if (activeWeapon)
        {
            foreach (GameObject weapon in playerWeapons)
            {
                if (weapon.activeSelf.Equals(true))
                {
                    ammoCount.text = weapon.GetComponentInChildren<GunController>().ammo.ToString();
                    maxAmmoCount.text = ("/ " + weapon.GetComponentInChildren<GunController>().reserveAmmo.ToString());
                }
            }

            if(playerWeapons.Count > 1)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    activeSlot = 0;
                    playerWeapons[0].SetActive(true);
                    playerWeapons[1].SetActive(false);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    activeSlot = 1;
                    playerWeapons[0].SetActive(false);
                    playerWeapons[1].SetActive(true);
                }
            }

            if (ammoBox) 
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    BuyAmmo(activeSlot);
                }
            }

            if (Input.GetKey(KeyCode.W))
            {
                playerWeapons[activeSlot].GetComponentInChildren<Animator>().SetBool("isWalking", true);
            }
            else
            {
                playerWeapons[activeSlot].GetComponentInChildren<Animator>().SetBool("isWalking", false);
            }
            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
            {
                playerWeapons[activeSlot].GetComponentInChildren<Animator>().SetBool("isRunning", true);
            }
            else
            {
                playerWeapons[activeSlot].GetComponentInChildren<Animator>().SetBool("isRunning", false);
            }

        }

        if (pickupWeapon) //Permite recoger el arma si estás en el trigger correspondiente
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                AddWeapon(weaponID);
                pickupWeapon = false;
                Destroy(pickUpWeaponGameObject);
                pickupWeapon = false;
                GameManager.Instance.interactText.gameObject.SetActive(false);

            }
        }



        if (Input.GetKeyDown(KeyCode.E) && t_melee >= melee_coolDown)
        {
            MeleeAttack();
        }
    }
    // Ataque cuerpo a cuerpo
    private void MeleeAttack()
    {
        knife.SetActive(true);
        if (playerWeapons.Count > 0)
        {
            playerWeapons[activeSlot].SetActive(false);
            playerWeapons[activeSlot].GetComponentInChildren<GunController>().isReloading = false;
        }
        if (Physics.Raycast(GameManager.Instance.playerCam.transform.position, GameManager.Instance.playerCam.transform.forward, out RaycastHit hit, meleeRange, enemyMask))
        {

            if (hit.transform.CompareTag("Body_Collider") || hit.transform.CompareTag("Head_Collider"))
            {

                int rnd = GameManager.Instance.RandomNumberGenerator(0, (GameManager.Instance.bloodFX.Length - 1));
                Instantiate(GameManager.Instance.bloodFX[rnd], hit.point, transform.rotation);
                if (GameManager.Instance.instaKill)
                {
                    hit.transform.parent.gameObject.GetComponent<ZM_AI>().ZM_Death();
                }
                else
                {
                    hit.transform.parent.gameObject.GetComponent<ZM_AI>().ReduceHP(meleeDamage);
                }

            }
        }
        t_melee = 0f;
    }

    #region Powerups

    public void MaxAmmo() //Municion maxima
    {
        GameManager.Instance.maxAmmoUI.GetComponent<Animator>().Play("MaxAmmo");
        foreach (GameObject weapon in playerWeapons) //Recorre la lista y da al jugador la municion maxima de todas sus armas
        {
            weapon.GetComponentInChildren<GunController>().reserveAmmo = weapon.GetComponentInChildren<GunController>().ammoCapacity * weapon.GetComponentInChildren<GunController>().extraMags;
        }
    }

    public void InstaKill() //Baja Instantanea
    {
        GameManager.Instance.instaKill = true;
    }

    public void DoublePoints() //Puntos Dobles 
    {
        GameManager.Instance.doublePoints = true;
    }

    #endregion

    #region Armas y munición
    // Añade el arma específicada al jugador
    public void AddWeapon(int weaponID)
    {

        if (playerWeapons.Count == 0) //Si el jugador no tiene armas, añade el arma, la activa y sale de la función
        {
            playerWeapons.Add(Instantiate(GameManager.Instance.weaponPrefabs[weaponID], gunInstancePosition.position, gunInstancePosition.rotation, GameManager.Instance.playerCam.transform));
            playerWeapons[0].SetActive(true);
            activeSlot = 0;
            activeWeapon = true;
            return;
        }

        if (playerWeapons.Count == 1) //Si el jugador tiene un arma, añade el arma y la activa.
        {
            playerWeapons.Add(Instantiate(GameManager.Instance.weaponPrefabs[weaponID], gunInstancePosition.position, gunInstancePosition.rotation, GameManager.Instance.playerCam.transform));
            playerWeapons[0].SetActive(false);
            playerWeapons[1].SetActive(true);
            activeSlot = 1;
        }

        //Si el jugador tiene 2 armas, detecta la posición en la que tiene el arma actualmente y la sustituye por la nueva
        else if (!playerWeapons[0].GetComponentInChildren<GunController>().id.Equals(weaponID) || !playerWeapons[1].GetComponentInChildren<GunController>().id.Equals(weaponID))
        {

            if (activeSlot == 0)
            {
                Destroy(playerWeapons[activeSlot]);
                playerWeapons.RemoveAt(activeSlot);
                playerWeapons.Insert(0, Instantiate(GameManager.Instance.weaponPrefabs[weaponID], gunInstancePosition.position, gunInstancePosition.rotation, GameManager.Instance.playerCam.transform)); //Inserta el nuevo arma en la posición dada
            }
            else if (activeSlot == 1)
            {
                Destroy(playerWeapons[activeSlot]);
                playerWeapons.RemoveAt(activeSlot);
                playerWeapons.Insert(1, Instantiate(GameManager.Instance.weaponPrefabs[weaponID], gunInstancePosition.position, gunInstancePosition.rotation, GameManager.Instance.playerCam.transform)); //Inserta el nuevo arma en la posición dada
            }
        }
    }

    public void BuyAmmo(int i) //Compra de munición
    {
        playerWeapons[i].GetComponentInChildren<GunController>().reserveAmmo = playerWeapons[i].GetComponentInChildren<GunController>().ammoCapacity * playerWeapons[i].GetComponentInChildren<GunController>().extraMags;
        GameManager.Instance.ReduceScore(playerWeapons[i].GetComponentInChildren<GunController>().ammoCost);
    }
    public void ShowWeapon() //Muestra el arma una vez el ataque melee ha acabado
    {
        if (activeWeapon)
        {

            playerWeapons[activeSlot].SetActive(true);
        }

        knife.SetActive(false);
    }
    #endregion

    private void OnTriggerEnter(Collider other) //Detección de triggers
    {
        if (other.CompareTag("PickupWeapon"))
        {
            pickUpWeaponGameObject = other.gameObject;
            pickupWeapon = true;
            weaponID = other.GetComponent<WeaponID>().gunID;
            GameManager.Instance.interactText.gameObject.SetActive(true);
            GameManager.Instance.interactText.GetComponent<Animator>().Play("interact_text_idle");
            GameManager.Instance.interactText.text = "Press \"F\" to pick up " + GameManager.Instance.weaponPrefabs[weaponID].GetComponentInChildren<GunController>().gunName; //Le da información al jugador sobre la acción que va a realizar

        }

        if (other.CompareTag("AmmoBox"))
        {
            ammoBox = true;
            GameManager.Instance.interactText.gameObject.SetActive(true);
            GameManager.Instance.interactText.GetComponent<Animator>().Play("interact_text_idle");
            GameManager.Instance.interactText.text = "Press \"F\" to buy ammo (Cost: 500)";
        }

        if (other.CompareTag("MaxAmmo"))
        {
            MaxAmmo();
            Destroy(other.gameObject);
            Instantiate(GameManager.Instance.powerUp_fx, other.transform.position, other.transform.rotation);
        }
        if (other.CompareTag("Instakill"))
        {
            InstaKill();
            Destroy(other.gameObject);
            Instantiate(GameManager.Instance.powerUp_fx, other.transform.position, other.transform.rotation);
        }
        if (other.CompareTag("DoublePoints"))
        {
            DoublePoints();
            Destroy(other.gameObject);
            Instantiate(GameManager.Instance.powerUp_fx, other.transform.position, other.transform.rotation);
        }
        if (other.CompareTag("NukePowerup"))
        {
            GameManager.Instance.NukePowerUp();
            Destroy(other.gameObject);
            Instantiate(GameManager.Instance.powerUp_fx, other.transform.position, other.transform.rotation);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameManager.Instance.interactText.gameObject.SetActive(false);

        if (other.CompareTag("AmmoBox"))
        {
            ammoBox = false;
        }

        if (other.CompareTag("PickupWeapon"))
        {
            pickupWeapon = false;
        }
    }
}


