using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] int enemiesAlive = 0;
    [SerializeField] int round = 0;

    [SerializeField] GameObject[] spawnPoints;

    [SerializeField] GameObject enemyPrefab;

    public int EnemiesAlive { get => enemiesAlive; set => enemiesAlive = value; }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (enemiesAlive == 0)
        {
            round++;
            NextWave(round);
        }
    }

    public void NextWave(int round)
    {
        for (int x = 0; x < round; x++)
        {
            GameObject spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPoint.transform.position, Quaternion.identity);
            spawnedEnemy.GetComponent<EnemyManager>().GameManager = GetComponent<GameManager>();

            enemiesAlive++;
        }

    }
}
