using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GunShooting : MonoBehaviour
{
    [Header("GunData")]
    public string gunName; //Nombre del arma
    public int id, ammoCost, pellets, damagePellet, damage; //id, coste de la municion, perdigones (Escopeta), daño por perdigón (Escopeta) y daño del arma.
    public Animator gunAnimator;

    public LayerMask enemyMask;


    [HideInInspector]
    public bool instaKill;
    [Header("Firing")]
    public ParticleSystem muzzleFX;
    private float firing_coolDown, t_fire;
    private bool headShot;
    public float rateOfFire, headshotMultiplier, range;
    enum FiringMode { semi, full, buckshot }
    [SerializeField] FiringMode firingMode;


    [Header("Spread")]
    public Camera cam;
    public float minX, maxX, minY, maxY;
    float spreadX, spreadY;
    Vector3 spread;

    [Header("DisplayAmmunition")]
    public int ammo;
    public int reserveAmmo;
    [Header("InternalAmmunition")]
    public int ammoCapacity;
    public int extraMags, initialMags;
    [Header("Reload")]
    public bool isReloading;
    public float reloadTime, emptyReloadTime;

    void Start()
    {
        //Se asignan los valores de las variables
        ammo = ammoCapacity;
        reserveAmmo = ammoCapacity * initialMags;
        gunAnimator = GetComponent<Animator>();
        t_fire = firing_coolDown;
        cam = GameObject.Find("fpsCam").GetComponent<Camera>();

    }

    // Update is called once per frame
    void Update()
    {

        firing_coolDown = 60 / rateOfFire;
        if (t_fire < firing_coolDown)
        {
            t_fire += Time.deltaTime;
        }

        if (firingMode.Equals(FiringMode.semi))
        {
            if (Input.GetButtonDown("Fire1") && ammo > 0 && t_fire >= firing_coolDown && !isReloading && GameManager.Instance.isPaused == false)
            {
                Fire();
            }
        }
        else if (firingMode.Equals(FiringMode.full))
        {
            if (Input.GetButton("Fire1") && ammo > 0 && t_fire >= firing_coolDown && !isReloading && GameManager.Instance.isPaused == false)
            {
                Fire();
            }
        }
        else if (firingMode.Equals(FiringMode.buckshot))
        {
            if (Input.GetButtonDown("Fire1") && ammo > 0 && t_fire >= firing_coolDown && !isReloading && GameManager.Instance.isPaused == false)
            {
                BuckShot();
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && reserveAmmo > 0 && ammo < ammoCapacity && t_fire >= firing_coolDown || ammo == 0 && !isReloading && GameManager.Instance.isPaused == false && reserveAmmo > 0 && t_fire >= firing_coolDown)
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
        t_fire = 0f;
        ammo--;
        muzzleFX.Play();
        gunAnimator.Play("Gun_Fire");
        gunAnimator.Play("Arms_Fire");
        spreadX = Random.Range(minX, maxX);
        spreadY = Random.Range(minY, maxY);
        spread = new Vector3(spreadX, spreadY, 0);
        Debug.DrawRay(cam.transform.position, (cam.transform.forward + spread) * range , Color.red, 5f);
        if (Physics.Raycast(cam.transform.position, cam.transform.forward + spread, out RaycastHit hit, range, enemyMask))
        {

            if (hit.transform.CompareTag("Body_Collider"))
            {
                int rnd = GameManager.Instance.RandomNumberGenerator(0, (GameManager.Instance.bloodFX.Length - 1)); //Genera un numero entre 0 y la cantidad de objetos del array
                Instantiate(GameManager.Instance.bloodFX[rnd], hit.point, transform.rotation); //Instancia el objeto en el index dado por el RNG

                if (GameManager.Instance.instaKill)
                {
                    hit.transform.parent.gameObject.GetComponent<ZM_AI>().ZM_Death(headShot);
                }
                else
                {
                    hit.transform.parent.gameObject.GetComponent<ZM_AI>().ReduceHP(damage, headShot);
                    headShot = false;
                }

            }

            if (hit.transform.CompareTag("Head_Collider"))
            {
                int rnd = GameManager.Instance.RandomNumberGenerator(0, (GameManager.Instance.bloodFX.Length - 1)); //Genera un numero entre 0 y la cantidad de objetos del array
                Instantiate(GameManager.Instance.bloodFX[rnd], hit.point, transform.rotation); //Instancia el objeto en el index dado por el RNG

                if (GameManager.Instance.instaKill)
                {

                    hit.transform.parent.gameObject.GetComponent<ZM_AI>().ZM_Death(headShot);
                }
                else
                {
                    hit.transform.parent.gameObject.GetComponent<ZM_AI>().ReduceHP(damage * headshotMultiplier, headShot);
                    headShot = true;
                }
            }

            if (hit.transform.CompareTag("Wall"))
            {
                Instantiate(GameManager.Instance.wallChipFX, hit.point, Quaternion.identity);
            }


            if (ammo == 0 && reserveAmmo > 0)
            {
                StartCoroutine(Reload());
            }
        }
    }

    private void BuckShot()
    {

        ammo--;
        muzzleFX.Play();
        gunAnimator.Play("Gun_Fire");
        for(int i = 0; i <= pellets; i++)
        {
            spreadX = Random.Range(minX, maxX);
            spreadY = Random.Range(minY, maxY);
            spread = new Vector3(spreadX, spreadY, 0);
            Debug.DrawRay(cam.transform.position, cam.transform.forward + spread, Color.red, 5f);
            if (Physics.Raycast(cam.transform.position, cam.transform.forward + spread, out RaycastHit hit, range, enemyMask))
            {

                if (hit.transform.CompareTag("Body_Collider"))
                {
                    int rnd = GameManager.Instance.RandomNumberGenerator(0, (GameManager.Instance.bloodFX.Length - 1)); //Genera un numero entre 0 y la cantidad de objetos del array
                    Instantiate(GameManager.Instance.bloodFX[rnd], hit.point, transform.rotation); //Instancia el objeto en el index dado por el RNG
                    Debug.DrawRay(cam.transform.position, cam.transform.forward * 20 + spread, Color.green, 5f);
                    if (GameManager.Instance.instaKill)
                    {
                        hit.transform.parent.gameObject.GetComponent<ZM_AI>().ZM_Death(headShot);
                    }
                    else
                    {
                        hit.transform.parent.gameObject.GetComponent<ZM_AI>().ReduceHP(damagePellet, headShot);
                        headShot = false;
                    }

                }

                if (hit.transform.CompareTag("Head_Collider"))
                {
                    int rnd = GameManager.Instance.RandomNumberGenerator(0, (GameManager.Instance.bloodFX.Length - 1)); //Genera un numero entre 0 y la cantidad de objetos del array
                    Instantiate(GameManager.Instance.bloodFX[rnd], hit.point, transform.rotation); //Instancia el objeto en el index dado por el RNG
                    if (GameManager.Instance.instaKill)
                    {

                        hit.transform.parent.gameObject.GetComponent<ZM_AI>().ZM_Death(headShot);
                    }
                    else
                    {
                        hit.transform.parent.gameObject.GetComponent<ZM_AI>().ReduceHP(damagePellet * headshotMultiplier, headShot);
                        headShot = true;
                    }
                }

                if (hit.transform.CompareTag("Wall"))
                {
                    Instantiate(GameManager.Instance.wallChipFX, hit.point, Quaternion.identity);
                }
            }
        }
        t_fire = 0f;
        if (ammo == 0 && reserveAmmo > 0 && t_fire >= firing_coolDown)
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

}
