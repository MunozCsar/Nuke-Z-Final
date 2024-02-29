using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
    Este script gestiona el disparo de un arma de fuego, incluyendo el manejo del comportamiento de disparo, recarga, munición, daño y efectos visuales.
*/

public class GunController : MonoBehaviour
{
    [Header("GunData")]
    public string gunName;
    public int id, ammoCost, pellets, damagePellet, damage;
    public Animator gunAnimator;
    public LayerMask enemyMask;
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
        ammo = ammoCapacity;
        reserveAmmo = ammoCapacity * initialMags;
        gunAnimator = GetComponent<Animator>();
        t_fire = firing_coolDown;
        cam = GameObject.Find("fpsCam").GetComponent<Camera>();
    }

    void Update()
    {
        // Calculo de la cadencia de disparo
        firing_coolDown = 60 / rateOfFire;
        // Si el temporizador de disparo es menor que el enfriamiento le suma Time.deltaTime
        if (t_fire < firing_coolDown)
        {
            t_fire += Time.deltaTime;
        }

        // Inicia la recarga si la munición está agotada
        if (ammo == 0 && reserveAmmo > 0 && !GameManager.Instance.isPaused && !isReloading && t_fire >= firing_coolDown)
        {
            StartCoroutine(ReloadWeapon());
            Debug.Log(ammo);
        }

        // Inicia la recarga si se presiona la tecla de recarga y hay munición disponible
        if (Input.GetKeyDown(KeyCode.R) && reserveAmmo > 0 && ammo < ammoCapacity && t_fire >= firing_coolDown)
        {
            StartCoroutine(ReloadWeapon());
        }

        // Comprueba el modo de disparo y la disponibilidad de munición y si se está recargando antes de disparar
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


    }

    // Realiza un disparo
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
        // Lanza un raycast desde la posición de la cámara hacia delante, sumándole la dispersión y almacenando la información en una variable
        if (Physics.Raycast(cam.transform.position, cam.transform.forward + spread, out RaycastHit hit, range, enemyMask))
        {
            // Efecto de impacto en el cuerpo del enemigo
            if (hit.transform.CompareTag("Body_Collider"))
            {
                // Efecto de impacto en el cuerpo del enemigo
                // Se reproduce un efecto de sangre y se reduce su salud
                int rnd = GameManager.Instance.RandomNumberGenerator(0, (GameManager.Instance.bloodFX.Length - 1));
                Instantiate(GameManager.Instance.bloodFX[rnd], hit.point, transform.rotation);
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
            // Efecto de impacto en la cabeza del enemigo
            if (hit.transform.CompareTag("Head_Collider"))
            {
                int rnd = GameManager.Instance.RandomNumberGenerator(0, (GameManager.Instance.bloodFX.Length - 1));
                Instantiate(GameManager.Instance.bloodFX[rnd], hit.point, transform.rotation);
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
            // Efecto de impacto en la pared
            if (hit.transform.CompareTag("Wall"))
            {
                Instantiate(GameManager.Instance.wallChipFX, hit.point, Quaternion.identity);
            }
        }

    }

    // Realiza un disparo de perdigones
    private void BuckShot()
    {
        ammo--;
        muzzleFX.Play();
        gunAnimator.Play("Gun_Fire");
        // Realiza tantos disparos como perdigones tiene el arma
        for(int i = 0; i <= pellets; i++)
        {
            spreadX = Random.Range(minX, maxX);
            spreadY = Random.Range(minY, maxY);
            spread = new Vector3(spreadX, spreadY, 0);
            // Lanza un raycast desde la posición de la cámara hacia delante, sumándole la dispersión y almacenando la información en una variable
            if (Physics.Raycast(cam.transform.position, cam.transform.forward + spread, out RaycastHit hit, range, enemyMask))
            {
                // Efecto de impacto en el cuerpo del enemigo
                if (hit.transform.CompareTag("Body_Collider"))
                {
                    // Efecto de impacto en el cuerpo del enemigo
                    // Se reproduce un efecto de sangre y se reduce su salud
                    int rnd = GameManager.Instance.RandomNumberGenerator(0, (GameManager.Instance.bloodFX.Length - 1));
                    Instantiate(GameManager.Instance.bloodFX[rnd], hit.point, transform.rotation);
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
                // Efecto de impacto en la cabeza del enemigo
                if (hit.transform.CompareTag("Head_Collider"))
                {
                    int rnd = GameManager.Instance.RandomNumberGenerator(0, (GameManager.Instance.bloodFX.Length - 1));
                    Instantiate(GameManager.Instance.bloodFX[rnd], hit.point, transform.rotation);
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
                // Efecto de impacto en la pared
                if (hit.transform.CompareTag("Wall"))
                {
                    Instantiate(GameManager.Instance.wallChipFX, hit.point, Quaternion.identity);
                }
            }
        }
        t_fire = 0f;
        // Inicia la recarga si se agota la munición durante el disparo
        if (ammo == 0 && reserveAmmo > 0 && t_fire >= firing_coolDown)
        {
            StartCoroutine(ReloadWeapon());
        }
    }

    // Corrutina para recargar el arma
    IEnumerator ReloadWeapon()
    {
        isReloading = true;
        // Si la munición es 0 o menor, activa la animación de recarga con cargador vacio, si no, activa la animación de recarga normal
        if (ammo <= 0)
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

        // Llena el cargador con munición de reserva
        for(int i = ammo; ammo < ammoCapacity && reserveAmmo > 0; i++)
        {
            ammo++;
            reserveAmmo--;
        }
        isReloading = false;
    }
}

