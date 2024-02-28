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
        knife.SetActive(false); //Al iniciar la partida se esconde el cuchillo
    }

    // Update is called once per frame
    void Update()
    {
        if(t_melee < melee_coolDown) //Timer que controla cada cuanto se puede acuchillar
        {
            t_melee += Time.deltaTime;
        }


        if (activeWeapon)
        {
            foreach (GameObject weapon in playerWeapons) //Recorre las armas que el jugador posea
            {
                if (weapon.activeSelf.Equals(true)) //Detecta si el arma está activa y asigna sus datos de munición a los valores de la UI correspondientes
                {
                    ammoCount.text = weapon.GetComponentInChildren<GunController>().ammo.ToString();
                    maxAmmoCount.text = ("/ " + weapon.GetComponentInChildren<GunController>().reserveAmmo.ToString());
                }
            }

            if(playerWeapons.Count > 1) //Si el jugador posee más de un arma
            {
                if (Input.GetKeyDown(KeyCode.Alpha1)) //Al presionar 1 se activa el slot 0 y se esconde el slot 1
                {
                    activeSlot = 0;
                    playerWeapons[0].SetActive(true);
                    playerWeapons[1].SetActive(false);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2)) //Al presionar 2 se activa el slot 1 y se esconde el slot 0
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
                    BuyAmmo(activeSlot); //LLama a la funcion de compra de municion si el jugador está en un trigger correspondiente
                }
            }

            // Animaciones arma

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

            //Fin Animaciones arma
        }

        if (pickupWeapon) //Permite recoger el arma si estás en el trigger correspondiente
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                AddWeapon(weaponID);
                pickupWeapon = false;
                Destroy(pickUpWeaponGameObject);
                pickupWeapon = false; //Da el valor false a la variable
                GameManager.Instance.interactText.gameObject.SetActive(false);

            }
        }



        if (Input.GetKeyDown(KeyCode.E) && t_melee >= melee_coolDown) //Gestión de ataque melee
        {
            MeleeAttack();
        }
    }

    private void MeleeAttack()
    {
        knife.SetActive(true); //Activa el cuchillo
        if (playerWeapons.Count > 0)
        {
            playerWeapons[activeSlot].SetActive(false);
            playerWeapons[activeSlot].GetComponentInChildren<GunController>().isReloading = false;
        }
        if (Physics.Raycast(GameManager.Instance.playerCam.transform.position, GameManager.Instance.playerCam.transform.forward, out RaycastHit hit, meleeRange, enemyMask)) //Lanza un raycast desde la cámara hacia delante y recoge la información de impacto
        {

            if (hit.transform.CompareTag("Body_Collider") || hit.transform.CompareTag("Head_Collider")) //Si el raycast impacta al cuerpo o cabeza del enemigo entra en este bloque de código
            {

                int rnd = GameManager.Instance.RandomNumberGenerator(0, (GameManager.Instance.bloodFX.Length - 1)); //Genera un numero entre 0 y la cantidad de objetos del array
                Instantiate(GameManager.Instance.bloodFX[rnd], hit.point, transform.rotation); //Instancia el objeto en el index dado por el RNG
                if (GameManager.Instance.instaKill)
                {
                    hit.transform.parent.gameObject.GetComponent<ZM_AI>().ZM_Death(); //Si el jugador ha obtenido un instakill, mata al enemigo
                }
                else
                {
                    hit.transform.parent.gameObject.GetComponent<ZM_AI>().ReduceHP(meleeDamage); //De forma normal, reduce la vida del enemigo.
                }

            }
        }
        t_melee = 0f; //Reseta el timer
    }

    #region Powerups

    public void MaxAmmo() //Municion maxima
    {
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
                Destroy(playerWeapons[activeSlot]); //Destruye el GameObject viejo del arma
                playerWeapons.RemoveAt(activeSlot); //Destruye el valor de la lista en la posición del arma activa
                playerWeapons.Insert(0, Instantiate(GameManager.Instance.weaponPrefabs[weaponID], gunInstancePosition.position, gunInstancePosition.rotation, GameManager.Instance.playerCam.transform)); //Inserta el nuevo arma en la posición dada
            }
            else if (activeSlot == 1)
            {
                Destroy(playerWeapons[activeSlot]); //Destruye el GameObject viejo del arma
                playerWeapons.RemoveAt(activeSlot); //Destruye el valor de la lista en la posición del arma activa
                playerWeapons.Insert(1, Instantiate(GameManager.Instance.weaponPrefabs[weaponID], gunInstancePosition.position, gunInstancePosition.rotation, GameManager.Instance.playerCam.transform)); //Inserta el nuevo arma en la posición dada
            }
        }
    }

    public void BuyAmmo(int i) //Compra de munición
    {
        //Similar a MaxAmmo, pero solo se ejecuta en el arma que el jugador tiene activa
        playerWeapons[i].GetComponentInChildren<GunController>().reserveAmmo = playerWeapons[i].GetComponentInChildren<GunController>().ammoCapacity * playerWeapons[i].GetComponentInChildren<GunController>().extraMags;
        GameManager.Instance.ReduceScore(playerWeapons[i].GetComponentInChildren<GunController>().ammoCost); //Reduce la puntuación.
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
        if (other.CompareTag("PickupWeapon")) //Si el trigger se llama "PickupWeapon"
        {
            pickUpWeaponGameObject = other.gameObject;
            pickupWeapon = true; //Da el valor true a la variable
            weaponID = other.GetComponent<WeaponID>().gunID; //Almacena el ID del arma en una variable local
            GameManager.Instance.interactText.gameObject.SetActive(true); //Activa el texto de interacción
            GameManager.Instance.interactText.GetComponent<Animator>().Play("interact_text_idle"); //Activa la animación idle del texto de interacción
            GameManager.Instance.interactText.text = "Press \"F\" to pick up " + GameManager.Instance.weaponPrefabs[weaponID].GetComponentInChildren<GunController>().gunName; //Le da información al jugador sobre la acción que va a realizar

        }

        if (other.CompareTag("AmmoBox"))
        {
            ammoBox = true; //Da el valor true a la variable
            GameManager.Instance.interactText.gameObject.SetActive(true); //Activa el texto de interacción
            GameManager.Instance.interactText.GetComponent<Animator>().Play("interact_text_idle"); //Activa la animación idle del texto de interacción
            GameManager.Instance.interactText.text = "Press \"F\" to buy ammo (Cost: 500)"; //Le da información al jugador sobre la acción que va a realizar y su coste
        }

        if (other.CompareTag("MaxAmmo"))
        {
            MaxAmmo(); //LLama a la funcion de MaxAmmo
            Destroy(other.gameObject); //Destruye el objeto trigger
            Instantiate(GameManager.Instance.powerUp_fx, other.transform.position, other.transform.rotation); //Instancia las particulas del pickup
        }
        if (other.CompareTag("Instakill"))
        {
            InstaKill(); //LLama a la funcion de InstaKill
            Destroy(other.gameObject); //Destruye el objeto trigger
            Instantiate(GameManager.Instance.powerUp_fx, other.transform.position, other.transform.rotation); //Instancia las particulas del pickup
        }
        if (other.CompareTag("DoublePoints"))
        {
            DoublePoints(); //LLama a la funcion de DoublePoints
            Destroy(other.gameObject); //Destruye el objeto trigger
            Instantiate(GameManager.Instance.powerUp_fx, other.transform.position, other.transform.rotation); //Instancia las particulas del pickup
        }
        if (other.CompareTag("NukePowerup"))
        {
            GameManager.Instance.NukePowerUp(); //LLama a la funcion de NukePowerup
            Destroy(other.gameObject); //Destruye el objeto trigger
            Instantiate(GameManager.Instance.powerUp_fx, other.transform.position, other.transform.rotation); //Instancia las particulas del pickup
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameManager.Instance.interactText.gameObject.SetActive(false); //Desactiva el texto de interacción

        if (other.CompareTag("AmmoBox"))
        {
            ammoBox = false; //Da el valor falase a la variable
        }

        if (other.CompareTag("PickupWeapon"))
        {
            pickupWeapon = false; //Da el valor false a la variable
        }
    }
}


