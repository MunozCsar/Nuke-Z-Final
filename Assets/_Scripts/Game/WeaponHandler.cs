using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponHandler : MonoBehaviour
{
    public GameObject[] weapons; // Array containing all game weapons (Figure out if possible to incorporate into GameManager)
    public List<GameObject> weaponList = new(); //List containing weapons currently held by the player.
    public GameObject cam; //Player camera.
    public Transform gunPos; //Position data for weapon instancing.
    public TMP_Text ammoCount, maxAmmoCount; //UI text.
    public bool wallWeaponBool, buyWeapon = false; //Various booleans to control different interactions.
    public int activeSlot, weaponSlots; //The currently active slot and the amount of weapon slots the player has.
    int weaponID, weaponCost; //Weapon ID and cost (Data gathered from weapon script)

    // Start is called before the first frame update
    void Start()
    {
        GameObject instantiatedWeapon = Instantiate(weapons[0], gunPos.position, gunPos.rotation, cam.transform); //When starting the game, the base weapon is instanced.
        weaponList.Add(instantiatedWeapon); //Weapon is added to the list.
        activeSlot = 0; //Set the active slot to 0.
        weaponList[0].SetActive(true); //Activate the slot.

    }

    // Update is called once per frame
    void Update()
    {
        foreach(GameObject weapon in weaponList) //Loops through the weapon list.
        {
            if (weapon.activeSelf.Equals(true)) //If the weapon is active, set UI elements to weapon values.
            {
                ammoCount.text = weapon.GetComponent<GunShooting>().ammo.ToString();
                maxAmmoCount.text = ("/ " + weapon.GetComponent<GunShooting>().reserveAmmo.ToString());
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) //When pressing '1', activate slot 0 and store the information.
        {
            activeSlot = 0;
            weaponList[0].SetActive(true);
            weaponList[1].SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) //When pressing '2', activate slot 1 and store the information.
        {
            activeSlot = 1;
            weaponList[0].SetActive(false);
            weaponList[1].SetActive(true);
        }

        if (wallWeaponBool) //If the player triggers a wallbuy interaction, enter this 'if' statement.
        {
            if (Input.GetKeyDown(KeyCode.E) && GameManager.Instance.score >= weaponCost && weaponList[activeSlot].GetComponent<GunShooting>().id != weaponID) //If the player presses the 'E' key and their score is greater than the cost of the weapon and the weapon the player is currently holding is different, enter the 'if' statement.
            {
                if(weaponList.Count > 1) //If the player is holding 2 or more weapons:
                {
                    switch (activeSlot) //Send the active slot value to a switch statement
                    {
                        case 0:
                            if(weaponList[1].GetComponent<GunShooting>().id == weaponID) //Detect whether the weapon stashed in the other slot is the same as the wallweapon
                            {
                                BuyAmmo(1); //Call BuyAmmo(int i) function to buy ammo for the stashed weapon
                                weaponList[0].SetActive(false); //Set the currently held weapon stashed
                                weaponList[1].SetActive(true); //Activate the currently stashed weapon
                                activeSlot = 1; //Store this information

                            }
                            else //If the stashed weapon is different, buy the wallweapon.
                            {
                                AddWeapon(weaponID);
                                GameManager.Instance.interactText.gameObject.SetActive(false);
                                GameManager.Instance.ReduceScore(weaponCost);
                            }
                            break;
                        case 1:
                            if (weaponList[0].GetComponent<GunShooting>().id == weaponID) //Detect whether the weapon stashed in the other slot is the same as the wallweapon
                            {
                                BuyAmmo(0); //Call BuyAmmo(int i) function to buy ammo for the stashed weapon
                                weaponList[0].SetActive(true); //Set the currently held weapon stashed
                                weaponList[1].SetActive(false); //Activate the currently stashed weapon
                                activeSlot = 0; //Store this information
                            }
                            else //If the stashed weapon is different, buy the wallweapon.
                            {
                                AddWeapon(weaponID);
                                GameManager.Instance.interactText.gameObject.SetActive(false);
                                GameManager.Instance.ReduceScore(weaponCost);
                            }
                            break;
                    }

                    GameManager.Instance.interactText.gameObject.SetActive(false);
                    GameManager.Instance.ReduceScore(weaponCost);
                }
                else
                {
                    AddWeapon(weaponID);
                    GameManager.Instance.interactText.gameObject.SetActive(false);
                    GameManager.Instance.ReduceScore(weaponCost);
                }

            }
            else if(Input.GetKeyDown(KeyCode.E) && GameManager.Instance.score >= weaponList[activeSlot].GetComponent<GunShooting>().ammoCost && weaponList[activeSlot].GetComponent<GunShooting>().id == weaponID)
            {
                BuyAmmo(activeSlot);
            }
        }
    }

    public void MaxAmmo()
    {
        foreach (GameObject weapon in weaponList)
        {
            weapon.GetComponent<GunShooting>().reserveAmmo = weapon.GetComponent<GunShooting>().ammoCapacity * weapon.GetComponent<GunShooting>().extraMags;
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
        if (weaponList.Count == 1)
        {
            weaponList.Add(Instantiate(weapons[weaponID], gunPos.position, gunPos.rotation, cam.transform));
            weaponList[0].SetActive(false);
            weaponList[1].SetActive(true);
            activeSlot = 1;
        }

        else if (!weaponList[0].GetComponent<GunShooting>().id.Equals(weaponID) || !weaponList[1].GetComponent<GunShooting>().id.Equals(weaponID))
        {

            if (activeSlot == 0)
            {
                Debug.Log(weaponID);
                Destroy(weaponList[activeSlot]);
                weaponList.RemoveAt(activeSlot);
                weaponList.Insert(0, Instantiate(weapons[weaponID], gunPos.position, gunPos.rotation, cam.transform));
            }
            else if (activeSlot == 1)
            {
                Debug.Log(weaponID + 1);
                Destroy(weaponList[activeSlot]);
                weaponList.RemoveAt(activeSlot);
                weaponList.Insert(1, Instantiate(weapons[weaponID], gunPos.position, gunPos.rotation, cam.transform));
            }
        }

        wallWeaponBool = false;
    }

    public void BuyAmmo(int i)
    {
        weaponList[i].GetComponent<GunShooting>().reserveAmmo = weaponList[i].GetComponent<GunShooting>().ammoCapacity * weaponList[i].GetComponent<GunShooting>().extraMags;
        GameManager.Instance.ReduceScore(weaponList[i].GetComponent<GunShooting>().ammoCost);
        wallWeaponBool = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WeaponChalk"))
        {
            GameManager.Instance.interactText.GetComponent<Animator>().Play("interact_text_idle");
            weaponID = other.GetComponent<WeaponChalk>().weapon.GetComponent<GunShooting>().id;
            weaponCost = other.GetComponent<WeaponChalk>().weapon.GetComponent<GunShooting>().gunCost;


            if (!weaponList[activeSlot].GetComponent<GunShooting>().id.Equals(weaponID))
            {
                GameManager.Instance.interactText.text = "Press 'E' to buy " + other.GetComponent<WeaponChalk>().weapon.GetComponent<GunShooting>().gunName + "(" + weaponCost + ")";
            }
            else
            {
                GameManager.Instance.interactText.text = "Press 'E' to buy ammunition for " + other.GetComponent<WeaponChalk>().weapon.GetComponent<GunShooting>().gunName + "(" + weaponList[activeSlot].GetComponent<GunShooting>().ammoCost + ")";
                Debug.Log("Buyammo");
            }

            if (weaponList.Count > 1)
            {
                switch (activeSlot)
                {
                    case 0:
                        if (weaponList[1].GetComponent<GunShooting>().id == weaponID)
                        {
                            GameManager.Instance.interactText.text = "Press 'E' to buy ammunition for " + weaponList[1].GetComponent<GunShooting>().gunName + "(" + weaponList[1].GetComponent<GunShooting>().ammoCost + ")";
                            Debug.Log(weaponList[1].GetComponent<GunShooting>().gunName);
                        }
                        else
                        {

                        }
                        break;
                    case 1:
                        if (weaponList[0].GetComponent<GunShooting>().id == weaponID)
                        {
                            GameManager.Instance.interactText.text = "Press 'E' to buy ammunition for " + weaponList[0].GetComponent<GunShooting>().gunName + "(" + weaponList[0].GetComponent<GunShooting>().ammoCost + ")";
                            Debug.Log(weaponList[0].GetComponent<GunShooting>().gunName);
                        }
                        break;
                }
            }

            GameManager.Instance.interactText.gameObject.SetActive(true);
            wallWeaponBool = true;
        }

        if (other.CompareTag("AmmoBox"))
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
    }

    private void OnTriggerExit(Collider other)
    {
        GameManager.Instance.interactText.gameObject.SetActive(false);
        wallWeaponBool = false;
    }
}


