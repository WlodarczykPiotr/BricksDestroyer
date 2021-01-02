using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Text TargetText;
    public Text ScoreText;
    public Text LivesText;

    public int Score { get; set; }

    private void Awake()
    {
        Brick.OnBrickDestruction += OnBrickDestruction;
        BricksManager.Instance.OnLevelLoaded += OnLevelLoaded;
        GameManager.Instance.OnLiveLost += OnLiveLost;
    }

    private void Start ()
    {
        OnLiveLost(GameManager.Instance.currentLives);
    }

    private void OnLiveLost(int remainingLives)
    {
        LivesText.text = "LIVES:\n" + remainingLives;
    }

    private void OnLevelLoaded()
    {
        UpdateRemainingBricksText();
        UpdateScoreText(0);
    }

    private void UpdateScoreText(int increment)
    {
        this.Score += increment;
        string scoreString = this.Score.ToString().PadLeft(5, '0');
        ScoreText.text = "SCORE:\n" + scoreString;
    }

    private void OnBrickDestruction(Brick obj)
    {
        UpdateRemainingBricksText();
        UpdateScoreText(10);
    }

    private void UpdateRemainingBricksText()
    {
        TargetText.text = "TARGET:\n" + (BricksManager.Instance.InitialBricksCount - BricksManager.Instance.RemainingBricks.Count) + "/" + BricksManager.Instance.InitialBricksCount;
    }

    private void OnDisable()
    {
        Brick.OnBrickDestruction -= OnBrickDestruction;
        BricksManager.Instance.OnLevelLoaded -= OnLevelLoaded;
    }
}