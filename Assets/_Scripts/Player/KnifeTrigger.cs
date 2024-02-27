using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeTrigger : MonoBehaviour
{
    public WeaponHandler weaponHandler;
    public BoxCollider knifeCollider;
    public float damage;
    public bool filler = false;
    //public void HideWeapon()
    //{
    //    weaponHandler.HideWeapon();
    //}

    public void ShowWeapon()
    {
        weaponHandler.ShowWeapon();
    }
}
