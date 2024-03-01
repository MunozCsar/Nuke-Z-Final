using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/*
    Este script controla el comportamiento de la inteligencia artificial de los zombies en el juego.
    Gestiona la navegación de los zombies, su interacción con las barreras y jugadores, así como su vida y muerte.
*/

public class ZM_AI : MonoBehaviour
{
    public Animator zm_Animator;
    public GameObject nearestObject, bodyCollider, headCollider, soulPrefab;
    private NavMeshAgent agent = null;
    private Transform target;
    public GameObject[] targetBarrier;
    public bool focusBarrier, isAlive, isInZone;
    public float t_barrier, barrier_coolDown, d_Nearest, hp;

    void Start()
    {
        hp = GameManager.Instance.zm_HP;
        targetBarrier = GameObject.FindGameObjectsWithTag("DestroyBarrier");
        nearestObject = targetBarrier[0];
        d_Nearest = Vector3.Distance(transform.position, nearestObject.transform.position);

        // Encuentra la barrera más cercana al zombie
        for (int i = 1; i < targetBarrier.Length; i++)
        {
            float distanceToCurrent = Vector3.Distance(transform.position, targetBarrier[i].transform.position);

            if (distanceToCurrent < d_Nearest)
            {
                nearestObject = targetBarrier[i];
                d_Nearest = distanceToCurrent;
            }
        }

        target = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = 2f;
    }

    void Update()
    {
        // Controla la velocidad y el daño del zombie según la oleada actual del juego
        switch (GameManager.Instance.wave)
        {
            default:
                agent.speed = 2f; 
                zm_Animator.SetBool("zombie_Run", false);
                GameManager.Instance.zm_Damage = 35;
                break;

            case 10:
                agent.speed = 3f; 
                zm_Animator.SetBool("zombie_Run", false);
                GameManager.Instance.zm_Damage = 50;
                break;

            case 20:
                agent.speed = 4f; 
                zm_Animator.SetBool("zombie_Run", true);
                GameManager.Instance.zm_Damage = 75;
                break;
        }

        // Controla el movimiento del zombie hacia su objetivo
        if (!focusBarrier && isAlive)
        {
            MoveToTarget();
        }
        else if (isAlive)
        {
            DestroyBarrier();
        }

    }

    // Hace que el zombie se mueva hacia su objetivo
    private void MoveToTarget()
    {
        agent.SetDestination(target.position);
    }

    // Hace que el zombie se mueva hacia y destruya una barrera
    private void DestroyBarrier()
    {
        agent.SetDestination(nearestObject.transform.position);
    }

    // Reduce los puntos de salud del zombie y gestiona su muerte en caso de llegar a cero
    // Detecta disparos a la cabeza
    public void ReduceHP(float damage, bool headShot)
    {
        hp -= damage;
        if (hp <= 0)
        {
            ZM_Death(headShot);
        }
        else
        {
            GameManager.Instance.AddPoints(GameManager.Instance.pointsOnHit);
        }
    }

    // Sobrecarga de método para reducir los puntos de salud del zombie sin especificar disparo a la cabeza
    public void ReduceHP(float damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            ZM_Death();
        }
        else
        {
            GameManager.Instance.AddPoints(GameManager.Instance.pointsOnHit);
        }
    }

    // Gestiona la muerte del zombie y otorga puntos al jugador
    public void ZM_Death(bool headShot)
    {
        GameManager.Instance.killScore++;
        GameManager.Instance.zm_alive--;
        if (headShot)
        {
            GameManager.Instance.AddPoints(GameManager.Instance.pointsOnHead);
            GetComponent<Animator>().Play("Zombie_Headshot");
            agent.speed = 0f;
            agent.isStopped = true;
            isAlive = false;
            headCollider.SetActive(false);
            bodyCollider.SetActive(false);
            GetComponent<SphereCollider>().enabled = false;
        }
        else
        {
            GameManager.Instance.AddPoints(GameManager.Instance.pointsOnKill);
            GetComponent<Animator>().Play("Zombie_Death");
            agent.speed = 0f;
            agent.isStopped = true;
            isAlive = false;
            headCollider.SetActive(false);
            bodyCollider.SetActive(false);
            GetComponent<SphereCollider>().enabled = false;
        }
        // Si no está atacando a una barrera, permite que se instancie un powerup
        if (!focusBarrier)
        {
            DropPowerUp();
        }
        HarvestSoul();
        Destroy(this.gameObject, 15f);
        GameManager.Instance.zombieList.Remove(this.gameObject);
    }

    // Sobrecarga de método para gestionar la muerte del zombie sin especificar disparo a la cabeza
    public void ZM_Death()
    {
        GameManager.Instance.AddPoints(GameManager.Instance.pointsOnHead);
        GameManager.Instance.killScore++;
        GameManager.Instance.zm_alive--;
        GetComponent<Animator>().Play("Zombie_Death");
        agent.speed = 0f;
        agent.isStopped = true;
        isAlive = false;
        headCollider.SetActive(false);
        bodyCollider.SetActive(false);
        // Si no está atacando a una barrera, permite que se instancie un powerup
        if (!focusBarrier)
        {
            DropPowerUp();
        }
        HarvestSoul();
        GameManager.Instance.zombieList.Remove(gameObject);
        Destroy(this.gameObject, 15f);
    }

    // Mata a todos los zombies en la escena y limpia la lista
    public void ZM_Nuke()
    {
        Instantiate(GameManager.Instance.bloodFX[4], transform.position, GameManager.Instance.bloodFX[4].transform.rotation);
        GetComponent<Animator>().Play("Zombie_Death");
        agent.speed = 0f;
        agent.isStopped = true;
        isAlive = false;
        headCollider.SetActive(false);
        bodyCollider.SetActive(false);
        // Si no está atacando a una barrera, permite que se instancie un powerup
        if (!focusBarrier)
        {
            DropPowerUp();
        }
        HarvestSoul();
        Destroy(this.gameObject, 15f);
    }

    // Crea un alma en la zona correspondiente si el zombie muere en ella
    public void HarvestSoul()
    {
        if (isInZone && GameManager.Instance.powerOn)
        {
            Instantiate(soulPrefab, this.gameObject.transform.position, this.gameObject.transform.rotation);
        }
    }

    // Genera un objeto de potenciador al azar cuando el zombie muere
    public void DropPowerUp()
    {
        if (GameManager.Instance.powerUp_current < GameManager.Instance.powerUp_max)
        {
            bool dropPowerUp = false;
            float rnd = GameManager.Instance.RandomNumberGenerator(0f, 1f);
            if (rnd < GameManager.Instance.powerUpChance)
            {
                dropPowerUp = true;
            }
            if (dropPowerUp.Equals(true))
            {
                int rndINT = GameManager.Instance.RandomNumberGenerator(0, GameManager.Instance.powerUpArray.Length);
                Vector3 pos = new(this.transform.position.x, this.transform.position.y + 2, this.transform.position.z);
                GameManager.Instance.InstancePowerUp(rndINT, pos);
                GameManager.Instance.powerUp_current++;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Detecta si el zombie ha colisionado con el jugador y activa la animación de ataque
        if (other.CompareTag("Player"))
        {
            zm_Animator.SetBool("isAttacking", true);
        }
    }

    // Detecta la salida del jugador o el zombie de una zona específica
    private void OnTriggerExit(Collider other)
    {
        // Detecta si el zombie ha dejado de hacer colision con el jugador y desactiva la animación de ataque
        if (other.CompareTag("Player"))
        {
            agent.speed = 2f;
            zm_Animator.SetBool("isAttacking", false);
        }
        // Detecta si el zombie ha entrado en una zona de destrucción de barrera, asigna el valor a la variable y activa la animación de ataque
        if (other.CompareTag("DestroyBarrier"))
        {
            focusBarrier = false;
            zm_Animator.SetBool("isAttacking", false);
        }
    }

    // Detecta la permanencia del jugador o el zombie en una zona específica
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("DestroyBarrier") && isAlive)
        {
            if (t_barrier < barrier_coolDown)
            {
                t_barrier += Time.deltaTime;
            }
            else if (t_barrier >= barrier_coolDown && other.transform.parent.GetComponent<BarrierLogic>().hitPoints > 0)
            {
                zm_Animator.SetBool("isAttacking", true);
                t_barrier = 0f;
                other.transform.parent.GetComponent<BarrierLogic>().ReduceHitPoints();
            }

            if (other.transform.parent.GetComponent<BarrierLogic>().hitPoints <= 0)
            {
                focusBarrier = false;
            }
        }
        if (other.CompareTag("SoulZone") && other.gameObject.activeSelf == true)
        {
            isInZone = true;
        }
        else
        {
            isInZone = false;
        }
    }
}

