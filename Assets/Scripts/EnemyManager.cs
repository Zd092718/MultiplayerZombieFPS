using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    GameObject player;
    NavMeshAgent agent;
    GameManager gameManager;
    [SerializeField] Animator anim;
    [SerializeField] float damage = 5f;
    [SerializeField] float health = 20f;

    public GameManager GameManager { get => gameManager; set => gameManager = value; }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
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
            player.GetComponent<PlayerController>().PlayerHit(damage);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            gameManager.EnemiesAlive--;
            //Destroy Zombie
            Destroy(gameObject);
        }
    }
}
