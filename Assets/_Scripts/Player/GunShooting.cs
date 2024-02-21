using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GunShooting : MonoBehaviour
{
    [Header("GunData")]
    public string gunName;
    public int id, gunCost, ammoCost;

    private Vector3 initialGunPos, currentGunPos, currentRot, targetGunRot, targetGunPos;
    [Header("Recoil")]
    public Animator gunAnimator;
    public float recoilX, recoilY, recoilZ, kickback, returnTime, snappiness;

    public static GunShooting GunInstance { get; private set; }

    public Camera cam;


    [HideInInspector]
    public bool instaKill;
    [Header("Firing")]
    public bool fullAuto;
    private bool headShot;
    public float rateOfFire, damage = 25f, headshotMultiplier, range;
    private float rof, fireDelay;
    public ParticleSystem muzzleFX;
    public ParticleSystem[] bloodFX;

    [Header("DisplayAmmunition")]
    public int ammo;
    public int reserveAmmo;
    [Header("InternalAmmunition")]
    public int ammoCapacity;
    public int extraMags, initialMags;
    [Header("Reload")]
    public bool isReloading;
    public float reloadTime, emptyReloadTime;

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

                int rnd = Random.Range(0, 5);
                Instantiate(bloodFX[rnd], hit.point, transform.rotation);

                if (GameManager.Instance.instaKill)
                {
                    hit.transform.parent.gameObject.GetComponent<ZM_AI>().zm_Death(headShot);
                }
                else
                {
                    hit.transform.parent.gameObject.GetComponent<ZM_AI>().ReduceHP(damage, headShot);
                    headShot = false;
                }

            }

            if (hit.transform.CompareTag("Head_Collider"))
            {
                int rnd = Random.Range(0, 5);
                Instantiate(bloodFX[rnd], hit.point, transform.rotation);

                if (GameManager.Instance.instaKill)
                {

                    hit.transform.parent.gameObject.GetComponent<ZM_AI>().zm_Death(headShot);
                }
                else
                {
                    Debug.Log("Headshot!");
                    hit.transform.parent.gameObject.GetComponent<ZM_AI>().ReduceHP(damage * headshotMultiplier, headShot);
                    headShot = true;
                }
            }




            if (ammo == 0 && reserveAmmo > 0)
            {
                StartCoroutine(Reload());
            }
        }
    }

    public bool InstaKill()
    {
        instaKill = true;

        return instaKill;
    }

    IEnumerator Reload()
    {
        if(ammo <= 0)
        {
            gunAnimator.Play("Gun_ReloadEmpty");
            gunAnimator.Play("Arms_ReloadEmpty");
            yield return new WaitForSeconds(emptyReloadTime);
        }
        else
        {
            gunAnimator.Play("Gun_Reload");
            gunAnimator.Play("Arms_Reload");
            yield return new WaitForSeconds(reloadTime);
        }


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
