using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GunShooting : MonoBehaviour
{
    public string gunName;
    public int id, gunCost, ammoCost;
    private Vector3 initialGunPos, currentGunPos, currentRot, targetGunRot, targetGunPos;
    public Transform adsPos;
    [Header("Recoil")]
    public float recoilX;
    public float recoilY;
    public float recoilZ;
    public float kickback;
    public float returnTime;
    public float snappiness;

    public float adsSpeed;
    public static GunShooting GunInstance { get; private set; }
    public Animator gunAnimator;
    public Camera cam;

    [Header("Firing")]
    public bool isReloading, headShot, instaKill;
    public float rateOfFire, damage = 25f, rof, fireDelay ,headshotMultiplier, range;
    private float internalDamage;
    public ParticleSystem muzzleFX;

    public bool fullAuto;

    [Header("Ammunition")]
    public int ammo;
    public int extraMags, initialMags, ammoCapacity;
    public int reserveAmmo;
    public float reloadTime;

    private void Awake()
    {
        if (GunInstance == null)
        {
            GunInstance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Se asignan los valores de las variables
        internalDamage = damage;
        ammo = ammoCapacity;
        reserveAmmo = ammoCapacity * initialMags;
        gunAnimator = GetComponent<Animator>();
        fireDelay = rof;
        initialGunPos = transform.localPosition;
        cam = GameObject.Find("fpsCam").GetComponent<Camera>();


    }

    // Update is called once per frame
    void Update()
    {
        targetGunRot = Vector3.Lerp(targetGunRot, Vector3.zero, Time.deltaTime * returnTime);
        currentRot = Vector3.Slerp(currentRot, targetGunRot, Time.deltaTime * snappiness);
        transform.localRotation = Quaternion.Euler(currentRot);
        cam.transform.localRotation *= Quaternion.Euler(currentRot);
        ReturnToPosition();

        rof = 60 / rateOfFire;
        if (fireDelay < rof)
        {
            fireDelay += Time.deltaTime;
        }


        if (fullAuto)
        {
            if (Input.GetButton("Fire1") && ammo > 0 && fireDelay >= rof && !isReloading && GameManager.Instance.isPaused == false)
            {
                Fire();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1") && ammo > 0 && fireDelay >= rof && !isReloading && GameManager.Instance.isPaused == false)
            {
                Fire();
            }
        }
        if(Input.GetKeyDown(KeyCode.R) && reserveAmmo > 0 && ammo < ammoCapacity || ammo == 0 && !isReloading && GameManager.Instance.isPaused == false && reserveAmmo > 0)
        {
            isReloading = true;
            StartCoroutine(Reload());
        }
        if (Input.GetButton("Fire2"))
        {
            //AimDownSight();
        }
    }

    private void Fire()
    {
        fireDelay = 0f;
        ammo--;
        muzzleFX.Play();
        Recoil();
        gunAnimator.Play("Gun_Fire");
        gunAnimator.Play("Arms_Fire");
        Debug.DrawRay(cam.transform.position, cam.transform.forward * 20, Color.red, 5f);
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, range))
        {

            if (hit.transform.CompareTag("Body_Collider"))
            {
                if (GameManager.Instance.instaKill)
                {
                    hit.transform.parent.gameObject.GetComponent<ZM_HP>().zm_Death(headShot);
                }
                else
                {
                    hit.transform.parent.gameObject.GetComponent<ZM_HP>().ReduceHP(internalDamage, headShot);
                    headShot = false;
                }

            }
            if (hit.transform.CompareTag("Head_Collider"))
                if (GameManager.Instance.instaKill)
                {

                    hit.transform.parent.gameObject.GetComponent<ZM_HP>().zm_Death(headShot);
                }
                else
                {
                    Debug.Log("Headshot!");
                    hit.transform.parent.gameObject.GetComponent<ZM_HP>().ReduceHP(internalDamage * headshotMultiplier, headShot);
                    headShot = true;
                }
        }


        if (ammo == 0 && reserveAmmo > 0)
        {
            StartCoroutine(Reload());
        }
    }

    public bool InstaKill()
    {
        instaKill = true;

        return instaKill;
    }

    IEnumerator Reload()
    {

        gunAnimator.Play("Gun_Reload");
        gunAnimator.Play("Arms_Reload");
        yield return new WaitForSeconds(reloadTime);
        for(int i = ammo; ammo < ammoCapacity && reserveAmmo > 0; i++)
        {
            ammo++;
            reserveAmmo--;
        }
        isReloading = false;

    }

    public void Recoil()
    {
        targetGunPos -= new Vector3(0, 0, kickback);
        targetGunRot += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }
    public void ReturnToPosition()
    {
        targetGunPos = Vector3.Lerp(targetGunPos, initialGunPos, Time.deltaTime * returnTime);
        currentGunPos = Vector3.Lerp(currentGunPos, targetGunPos, Time.fixedDeltaTime * snappiness);
        transform.localPosition = currentGunPos;
    }
}
