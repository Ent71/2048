using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.iOS.Extensions.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button _startGameIn2DButton;
    [SerializeField] private Button _startGameIn3DButton;
    [SerializeField] private Button _exitButton;

    private void OnEnable()
    {
        _startGameIn2DButton.onClick.AddListener(OnStart2DGameButtonClick);
        _startGameIn3DButton.onClick.AddListener(OnStarat3DGameButtonClick);
        _exitButton.onClick.AddListener(OnExitButtonClick);
        
    }

    private void OnDisable()
    {
        _startGameIn2DButton.onClick.RemoveListener(OnStart2DGameButtonClick);
        _startGameIn3DButton.onClick.RemoveListener(OnStarat3DGameButtonClick);
        _exitButton.onClick.RemoveListener(OnExitButtonClick);
    }

    private void OnStart2DGameButtonClick()
    {
        SceneManager.LoadScene("Scenes/2D2048");
    }
    
    private void OnStarat3DGameButtonClick()
    { 
        SceneManager.LoadScene("Scenes/3D2048");
    }

    private void OnExitButtonClick()
    {
        Application.Quit();
    }
}
