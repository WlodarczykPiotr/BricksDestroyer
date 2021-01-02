using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public GameObject gameOverScreen;
    public GameObject gameOverScreen2;
    public GameObject victoryScreen;
    public GameObject pauseScreen;
    public GameObject startScreen;

    public int currentLives = 3;
    private int countChance = 4;

    public bool IsGameStarted { get; set; }
    public int Lives { get; set; }

    public event Action<int> OnLiveLost;

    void Start()
    {
        Screen.SetResolution(320, 480, true);
        startScreen.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void StartTheGame()
    {
        startScreen.SetActive(false);
        this.Lives = this.currentLives;
        Ball.OnBallDeath += OnBallDeath;
        Brick.OnBrickDestruction += OnBrickDestruction;
    }

    private void OnBrickDestruction(Brick obj)
    {
        if (BricksManager.Instance.RemainingBricks.Count <= 0)
        {
            BallsManager.Instance.ResetBalls();
            GameManager.Instance.IsGameStarted = false;
            BricksManager.Instance.LoadNextLevel();
        }
    }

    private void RestartGame()
    {
            gameOverScreen.SetActive(false);
            this.Lives = this.currentLives;
            OnLiveLost?.Invoke(this.Lives);
            BallsManager.Instance.ResetBalls();
            IsGameStarted = false;
            BricksManager.Instance.LoadLevel(BricksManager.Instance.CurrentLevel);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnBallDeath(Ball obj)
    {
        if(BallsManager.Instance.Balls.Count <= 0)
        {
            this.Lives--;

            if (this.Lives < 1)
            {
                OnLiveLost?.Invoke(this.Lives);
                countChance--;

                if (countChance >= 1)
                {
                    gameOverScreen.SetActive(true);
                }
                else
                {
                    gameOverScreen2.SetActive(true);
                }
            }
            else
            {
                OnLiveLost.Invoke(this.Lives);
                BallsManager.Instance.ResetBalls();
                IsGameStarted = false;
            }
        }
    }

    public void ShowVictoryScreen()
    {
        victoryScreen.SetActive(true);
    }

    private void OnDisable()
    {
        Ball.OnBallDeath -= OnBallDeath;
    }

    public void ResumeGame()
    {
        pauseScreen.SetActive(false);
    }

    void Update()
    {
        if(Input.GetKeyDown("escape"))
        {
            BallsManager.Instance.ResetBalls();
            IsGameStarted = false;
            pauseScreen.SetActive(true);
            
        }
    }
}