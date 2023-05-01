using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    GameObject player;
    NavMeshAgent agent;
    GameManager gameManager;
    [SerializeField] Animator anim;
    [SerializeField] Slider slider;
    [SerializeField] Canvas canvas;
    [SerializeField] float damage = 5f;
    [SerializeField] float health = 20f;
    [SerializeField] bool playerInReach;
    [SerializeField] float attackAnimStartDelay;
    [SerializeField] float delayBetweenAttacks;

    AudioSource audioSource;

    [SerializeField] AudioClip[] zombieSounds;

    float attackDelayTimer;

    public GameManager GameManager { get => gameManager; set => gameManager = value; }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        slider.maxValue = health;
        slider.value = health;
    }

    // Update is called once per frame
    void Update()
    {
        if(!audioSource.isPlaying)
        {
            audioSource.clip = zombieSounds[Random.Range(0, zombieSounds.Length)];
            audioSource.Play();
        }
        slider.transform.LookAt(player.transform);
        agent.destination = player.transform.position;
        if (agent.velocity.magnitude > 1)
        {
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInReach = true;

        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (playerInReach)
        {
            attackDelayTimer += Time.deltaTime;
        }

        if(attackDelayTimer >= delayBetweenAttacks - attackAnimStartDelay && attackDelayTimer <= delayBetweenAttacks && playerInReach)
        {
            anim.SetTrigger("isAttacking");
        }

        if(attackDelayTimer >= delayBetweenAttacks && playerInReach)
        {
            player.GetComponent<PlayerController>().PlayerHit(damage);
            attackDelayTimer = 0;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject == player)
        {
            playerInReach = false;
            attackDelayTimer = 0;
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        slider.value = health;
        if (health <= 0)
        {
            gameManager.EnemiesAlive--;
            //Destroy Zombie
            anim.SetTrigger("isDead");
            Destroy(gameObject, 10f);

            Destroy(agent);
            Destroy(GetComponent<EnemyManager>());
            Destroy(GetComponent<Collider>());
            canvas.enabled = false;
        }
    }
}
