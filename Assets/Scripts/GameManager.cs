using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] int enemiesAlive = 0;
    [SerializeField] int round = 0;

    [SerializeField] GameObject[] spawnPoints;

    [SerializeField] GameObject enemyPrefab;
    [SerializeField] PlayerController playerController;

    [SerializeField] TextMeshProUGUI roundText;
    [SerializeField] TextMeshProUGUI roundsSurvivedText;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] GameObject endScreen;
    [SerializeField] GameObject pauseScreen;

    public int EnemiesAlive { get => enemiesAlive; set => enemiesAlive = value; }
    public int Round { get => round; set => round = value; }
    private void Awake()
    {
        Instance = this;
    }
    // Update is called once per frame
    void Update()
    {
        if (enemiesAlive == 0)
        {
            round++;
            NextWave(round);
        }

        roundText.text = "Round " + round.ToString();
        healthText.text = "Health: " + playerController.Health.ToString();
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

    public void EndGame()
    {
        playerController.enabled = false;
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        endScreen.SetActive(true);
        roundsSurvivedText.text = round.ToString();
    }

    public void Reset()
    {
        playerController.enabled = true;
        SceneManager.LoadScene(1);
        Time.timeScale = 1;
    }

    public void Continue()
    {
        playerController.enabled = true;
        pauseScreen.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1.0f;
    }

    public void Pause()
    {
        playerController.enabled = false;
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        pauseScreen.SetActive(true);
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            Pause();
        }
    }
}
