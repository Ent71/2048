using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField] private TMP_Text _currentScoreText;
    [SerializeField] private TMP_Text _bestScoreText;
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private Button _restartButton;

    private int _currentScore = 0, _bestScore = 0;

    private void OnEnable()
    {
        _restartButton.onClick.AddListener(OnRestartButton);
        _gridManager.ScoreChanged += OnScoreChanged;
        _gridManager.GameOver += OnGameOver;
    }

    private void OnDisable()
    {
        _restartButton.onClick.RemoveListener(OnRestartButton);
        _gridManager.ScoreChanged -= OnScoreChanged;
        _gridManager.GameOver -= OnGameOver;
    }

    private void OnScoreChanged(int value)
    {
        AddScore(value);
    }

    private void OnRestartButton()
    {
        ChangeMaxScore();
        ChangeScore(0);
    }

    private void OnGameOver()
    {
        ChangeMaxScore();
    }

    private void AddScore(int value)
    {
        ChangeScore(_currentScore + value);
    }

    private void ChangeScore(int value)
    {
        _currentScore = value;
        _currentScoreText.text = _currentScore.ToString();
    }

    private void ChangeMaxScore()
    {
        if(_currentScore > _bestScore)
        {
            _bestScoreText.text = _currentScoreText.text;
        }
    }
}