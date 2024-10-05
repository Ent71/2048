using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(CanvasGroup))]
public class PauseScreen : MonoBehaviour
{
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Score _score;
    private SignalBus _signalBus;
    private TimeHandler _timeHandler;

    private CanvasGroup _pauseScreenGroup;
    
    [Inject]
    private void Construct(SignalBus signalBus, TimeHandler timeHandler)
    {
        _timeHandler = timeHandler;
        _signalBus = signalBus;
    }

    void Start()
    {
        _pauseScreenGroup = GetComponent<CanvasGroup>();
        ClosePauseScreen();
    }

    private void OnEnable()
    {
        _signalBus.Subscribe<GameOverSignal>(x => OnGameOver());
        _continueButton.onClick.AddListener(OnContinueButtonClick);
        _restartButton.onClick.AddListener(OnRestartButtonClick);
        _exitButton.onClick.AddListener(OnExitButtonClick);
        _pauseButton.onClick.AddListener(OnPauseButtonClick);
    }

    private void OnDisable()
    {
        _signalBus.TryUnsubscribe<GameOverSignal>(x => OnGameOver());
        _continueButton.onClick.RemoveListener(OnContinueButtonClick);
        _restartButton.onClick.RemoveListener(OnRestartButtonClick);
        _exitButton.onClick.RemoveListener(OnExitButtonClick);
        _pauseButton.onClick.RemoveListener(OnPauseButtonClick);
    }

    private void OnGameOver()
    {
        OpenPauseScreen();
        _continueButton.gameObject.SetActive(false);
    }

    private void OnContinueButtonClick()
    {
        ClosePauseScreen();
    }

    private void OnRestartButtonClick()
    {
        _signalBus.Fire<RestartPressedSignal>();
        ClosePauseScreen();
    }

    private void OnExitButtonClick()
    {
        _score.SaveResult();
        SceneManager.LoadScene("MainMenu");
    }

    private void ClosePauseScreen()
    {
        _pauseScreenGroup.alpha = 0;
        _pauseScreenGroup.blocksRaycasts = false;
        _pauseButton.gameObject.SetActive(true);
        StartTime();
    }

    private void OpenPauseScreen()
    {
        _pauseScreenGroup.alpha = 1;
        _pauseScreenGroup.blocksRaycasts = true;
        _pauseButton.gameObject.SetActive(false);
        StopTime();
    }

    private void StartTime()
    {
        _timeHandler.StartTime();
    }

    private void StopTime()
    {
        _timeHandler.StopTime();
    }

    private void OnPauseButtonClick()
    {
        OpenPauseScreen();
        _continueButton.gameObject.SetActive(true);
    }
}
