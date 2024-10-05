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

    private void Awake()
    {
        Debug.Log("load");
        _bestScore = PlayerPrefs.GetInt(_saveName);
        ChangeMaxScore();
    }

    private void OnEnable()
    {
        _restartButton.onClick.AddListener(OnRestartButton);
        _signalBus.Subscribe<GameOverSignal>(x => OnGameOver());
        _signalBus.Subscribe<ScoreChangedSignal>(x => OnScoreChanged(x.Score));
    }

    private void OnDisable()
    {
        _restartButton.onClick.RemoveListener(OnRestartButton);
        _signalBus.TryUnsubscribe<GameOverSignal>(x => OnGameOver());
        _signalBus.TryUnsubscribe<ScoreChangedSignal>(x => OnScoreChanged(x.Score));
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

    public void SaveResult()
    {
        Debug.Log("save");
        PlayerPrefs.SetInt(_saveName, _bestScore);
    }
}