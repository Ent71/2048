using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(CanvasGroup))]
public class PauseScreen : MonoBehaviour
{
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _pauseButton;
    private GridManager _gridManager;
    private SignalBus _signalBus;

    private CanvasGroup _pauseScreenGroup;
    
    [Inject]
    private void Construct(GridManager gridManager, SignalBus signalBus)
    {
        _gridManager = gridManager;
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
        _gridManager.ResetGrid();
        ClosePauseScreen();
    }

    private void OnExitButtonClick()
    {
        Application.Quit();
    }

    private void ClosePauseScreen()
    {
        _pauseScreenGroup.alpha = 0;
        _pauseScreenGroup.blocksRaycasts = false;
        _pauseButton.gameObject.SetActive(true);
        Time.timeScale = 1;
    }

    private void OpenPauseScreen()
    {
        _pauseScreenGroup.alpha = 1;
        _pauseScreenGroup.blocksRaycasts = true;
        _pauseButton.gameObject.SetActive(false);
        Time.timeScale = 0;
    }

    private void OnPauseButtonClick()
    {
        OpenPauseScreen();
        _continueButton.gameObject.SetActive(true);
    }
}
