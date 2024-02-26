using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//Tengo que limpiar esto...
public class WeaponHandler : MonoBehaviour
{
    [Header("Melee")]
    public float meleeDamage, meleeRange; 
    public float t_melee, melee_coolDown;
    public LayerMask enemyMask;
    [Header("Effects")]
    [Header("Weapons")]
    //public GameObject[] weaponPrefabs; // Array containing all game weapons (Figure out if possible to incorporate into GameManager)
    public List<GameObject> playerWeapons = new(); //List containing weapons currently held by the player.
    public GameObject knife; //Player camera || Knife GameObject
    public Transform gunPos; //Position data for weapon instancing.
    public TMP_Text ammoCount, maxAmmoCount; //UI text.
    bool pickupWeapon = false, activeWeapon, ammoBox; //Various booleans to control different interactions.
    public int activeSlot, weaponSlots, defaultWeapon; //The currently active slot and the amount of weapon slots the player has.
    int weaponID, weaponCost; //Weapon ID and cost (Data gathered from weapon script)

    // Start is called before the first frame update
    void Start()
    {
        //GameObject instantiatedWeapon = Instantiate(weapons[defaultWeapon], gunPos.position, gunPos.rotation, cam.transform); //When starting the game, the base weapon is instanced.
        //weaponList.Add(instantiatedWeapon); //Weapon is added to the list.
        //activeSlot = 0; //Set the active slot to 0.
        //weaponList[0].SetActive(true); //Activate the slot.
        knife.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if(t_melee < melee_coolDown)
        {
            t_melee += Time.deltaTime;
            Debug.Log("MeleeCooldown");
        }


        if (activeWeapon)
        {
            foreach (GameObject weapon in playerWeapons) //Loops through the weapon list.
            {
                if (weapon.activeSelf.Equals(true)) //If the weapon is active, set UI elements to weapon values.
                {
                    ammoCount.text = weapon.GetComponentInChildren<GunShooting>().ammo.ToString();
                    maxAmmoCount.text = ("/ " + weapon.GetComponentInChildren<GunShooting>().reserveAmmo.ToString());
                }
            }

            if(playerWeapons.Count > 1)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1)) //When pressing '1', activate slot 0 and store the information.
                {
                    activeSlot = 0;
                    playerWeapons[0].SetActive(true);
                    playerWeapons[1].SetActive(false);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2)) //When pressing '2', activate slot 1 and store the information.
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
        } //Code blocks that only works when the character has at least a weapon

        if (pickupWeapon) //Enters this 'if' statement if the character is colliding with a pickup weapon
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                AddWeapon(weaponID);
                pickupWeapon = false;

            }
        }



        if (Input.GetKeyDown(KeyCode.E) && t_melee >= melee_coolDown) //Melee codeblock
        {
            knife.SetActive(true);
            if (Physics.Raycast(GameManager.Instance.playerCam.transform.position, GameManager.Instance.playerCam.transform.forward, out RaycastHit hit, meleeRange, enemyMask))
            {

                if (hit.transform.CompareTag("Body_Collider"))
                {

                    int rnd = Random.Range(0, 4);
                    Instantiate(GameManager.Instance.bloodFX[rnd], hit.point, transform.rotation);
                    if (GameManager.Instance.instaKill)
                    {
                        hit.transform.parent.gameObject.GetComponent<ZM_AI>().zm_Death(true);
                    }
                    else
                    {
                        hit.transform.parent.gameObject.GetComponent<ZM_AI>().ReduceHP(meleeDamage, true);
                    }

                }
            }
            t_melee = 0f;
        }
    }

    public void MaxAmmo()
    {
        foreach (GameObject weapon in playerWeapons)
        {
            weapon.GetComponentInChildren<GunShooting>().reserveAmmo = weapon.GetComponentInChildren<GunShooting>().ammoCapacity * weapon.GetComponentInChildren<GunShooting>().extraMags;
        }
    }

    public void InstaKill()
    {
            GameManager.Instance.instaKill = true;
    }

    public void DoublePoints()
    {
        GameManager.Instance.doublePoints = true;
    }

    public void AddWeapon(int weaponID)
    {

        if (playerWeapons.Count == 0)
        {
            playerWeapons.Add(Instantiate(GameManager.Instance.weaponPrefabs[weaponID], gunPos.position, gunPos.rotation, GameManager.Instance.playerCam.transform));
            playerWeapons[0].SetActive(true);
            activeSlot = 0;
            activeWeapon = true;
            return;
        }

        if (playerWeapons.Count == 1)
        {
            playerWeapons.Add(Instantiate(GameManager.Instance.weaponPrefabs[weaponID], gunPos.position, gunPos.rotation, GameManager.Instance.playerCam.transform));
            playerWeapons[0].SetActive(false);
            playerWeapons[1].SetActive(true);
            activeSlot = 1;
        }

        else if (!playerWeapons[0].GetComponentInChildren<GunShooting>().id.Equals(weaponID) || !playerWeapons[1].GetComponentInChildren<GunShooting>().id.Equals(weaponID))
        {

            if (activeSlot == 0)
            {
                Destroy(playerWeapons[activeSlot]);
                playerWeapons.RemoveAt(activeSlot);
                playerWeapons.Insert(0, Instantiate(GameManager.Instance.weaponPrefabs[weaponID], gunPos.position, gunPos.rotation, GameManager.Instance.playerCam.transform));
            }
            else if (activeSlot == 1)
            {
                Destroy(playerWeapons[activeSlot]);
                playerWeapons.RemoveAt(activeSlot);
                playerWeapons.Insert(1, Instantiate(GameManager.Instance.weaponPrefabs[weaponID], gunPos.position, gunPos.rotation, GameManager.Instance.playerCam.transform));
            }
        }
        else
        {
            Debug.Log("Corresponde");
        }
    }

    public void BuyAmmo(int i)
    {
        playerWeapons[i].GetComponentInChildren<GunShooting>().reserveAmmo = playerWeapons[i].GetComponentInChildren<GunShooting>().ammoCapacity * playerWeapons[i].GetComponentInChildren<GunShooting>().extraMags;
        GameManager.Instance.ReduceScore(playerWeapons[i].GetComponentInChildren<GunShooting>().ammoCost);
    }
    public void ShowWeapon()
    {
        if (activeWeapon)
        {

            playerWeapons[activeSlot].SetActive(true);
        }

        knife.SetActive(false);
    }

    public void Melee()
    {
        if (activeWeapon)
        {
            playerWeapons[activeSlot].SetActive(false);
        }

        knife.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WeaponChalk"))
        {
            if (activeWeapon)
            {
                GameManager.Instance.interactText.GetComponent<Animator>().Play("interact_text_idle");
                weaponID = other.GetComponent<WeaponChalk>().gunID;
                weaponCost = GameManager.Instance.weaponPrefabs[weaponID].GetComponentInChildren<GunShooting>().gunCost;

                //Eliminar codigo residual de wallbuys e incorporar codigo de ammobox 
                //Eliminar assets relacionados con wallbuys 

                if (!playerWeapons[activeSlot].GetComponentInChildren<GunShooting>().id.Equals(weaponID))
                {
                    GameManager.Instance.interactText.text = "Press 'E' to buy " + GameManager.Instance.weaponPrefabs[weaponID].GetComponentInChildren<GunShooting>().gunName + "(" + weaponCost + ")";
                }
                else
                {
                    GameManager.Instance.interactText.text = "Press 'E' to buy ammunition for " + GameManager.Instance.weaponPrefabs[weaponID].GetComponentInChildren<GunShooting>().gunName + "(" + playerWeapons[activeSlot].GetComponentInChildren<GunShooting>().ammoCost + ")";
                    Debug.Log("Buyammo");
                }

                if (playerWeapons.Count > 1)
                {
                    switch (activeSlot)
                    {
                        case 0:
                            if (playerWeapons[1].GetComponent<GunShooting>().id == weaponID)
                            {
                                GameManager.Instance.interactText.text = "Press 'E' to buy ammunition for " + playerWeapons[1].GetComponentInChildren<GunShooting>().gunName + "(" + playerWeapons[1].GetComponentInChildren<GunShooting>().ammoCost + ")";
                                Debug.Log(playerWeapons[1].GetComponent<GunShooting>().gunName);
                            }
                            else
                            {

                            }
                            break;
                        case 1:
                            if (playerWeapons[0].GetComponentInChildren<GunShooting>().id == weaponID)
                            {
                                GameManager.Instance.interactText.text = "Press 'E' to buy ammunition for " + playerWeapons[0].GetComponentInChildren<GunShooting>().gunName + "(" + playerWeapons[0].GetComponentInChildren<GunShooting>().ammoCost + ")";
                                Debug.Log(playerWeapons[0].GetComponentInChildren<GunShooting>().gunName);
                            }
                            break;
                    }
                }

                GameManager.Instance.interactText.gameObject.SetActive(true);
            }

            else
            {
                GameManager.Instance.interactText.gameObject.SetActive(true);
                GameManager.Instance.interactText.GetComponent<Animator>().Play("interact_text_idle");
                weaponID = other.GetComponent<WeaponChalk>().gunID;
                GameManager.Instance.interactText.text = "Press 'E' to pickup " + GameManager.Instance.weaponPrefabs[weaponID].GetComponentInChildren<GunShooting>().gunName;
                pickupWeapon = true;
            }


        }

        if (other.CompareTag("PickupWeapon"))
        {
            pickupWeapon = true;
            weaponID = other.GetComponent<WeaponChalk>().gunID;
            GameManager.Instance.interactText.gameObject.SetActive(true);
            GameManager.Instance.interactText.GetComponent<Animator>().Play("interact_text_idle");
            GameManager.Instance.interactText.text = "Press \"F\" to pick up " + GameManager.Instance.weaponPrefabs[weaponID].GetComponentInChildren<GunShooting>().gunName;

        }

        if (other.CompareTag("AmmoBox"))
        {
            GameManager.Instance.interactText.gameObject.SetActive(true);
            GameManager.Instance.interactText.GetComponent<Animator>().Play("interact_text_idle");
            GameManager.Instance.interactText.text = "Press \"F\" to buy ammo (Cost: 500)";
            ammoBox = true;
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
            GameManager.Instance.Nuke();
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


