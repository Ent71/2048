using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Score : MonoBehaviour
{
    [SerializeField] private TMP_Text _currentScoreText;
    [SerializeField] private TMP_Text _bestScoreText;
    [SerializeField] private Button _restartButton;
    private SignalBus _signalBus;
    private string _saveName;
    private int _currentScore = 0, _bestScore = 0;

    [Inject]
    private void Construct(SignalBus signalBus, string saveName)
    {
        _signalBus = signalBus;
        _saveName = saveName;
    }

    private void Start()
    {
        _bestScore = PlayerPrefs.GetInt(_saveName);
        _bestScoreText.text = _bestScore.ToString();
    }

    private void OnEnable()
    {
        _restartButton.onClick.AddListener(OnRestartButton);
        _signalBus.Subscribe<ScoreChangedSignal>(x => OnScoreChanged(x.Score));
    }

    private void OnDisable()
    {
        _restartButton.onClick.RemoveListener(OnRestartButton);
        _signalBus.TryUnsubscribe<ScoreChangedSignal>(x => OnScoreChanged(x.Score));
    }

    private void OnScoreChanged(int value)
    {
        AddScore(value);
        ChangeMaxScore();
    }

    private void OnRestartButton()
    {
        ChangeScore(0);
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
            _bestScore = _currentScore;
            _bestScoreText.text = _currentScoreText.text;
            SaveResult();
        }
    }

    private void SaveResult()
    {
        
        PlayerPrefs.SetInt(_saveName, _bestScore);
        PlayerPrefs.Save();
    }
}