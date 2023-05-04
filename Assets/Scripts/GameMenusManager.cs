using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenusManager : MonoBehaviour
{

    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject draggablesMenu;
    [SerializeField] private GameObject exitButton;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private GameObject resultsMenu;
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private GameObject nextButton;
    
    private string _levelName;

    private void Start()
    {
        GameLogicManager.Instance.OnPhaseChanged += OnPhaseChanged;

        _levelName = SceneManager.GetActiveScene().name;
        _levelText.text = _levelName;
    }

    private void OnPhaseChanged(GamePhase phase)
    {
        switch (phase)
        {
            case GamePhase.Layout:
                if(!exitButton.activeSelf) exitButton.SetActive(true);
                if(!playButton.activeSelf) playButton.SetActive(true);
                if(!draggablesMenu.activeSelf) draggablesMenu.SetActive(true);
                if(restartButton.activeSelf) restartButton.SetActive(false);
                if(resultsMenu.activeSelf) resultsMenu.SetActive(false);
                break;
            case GamePhase.PlayOut:
                if(!exitButton.activeSelf) exitButton.SetActive(true);
                if(playButton.activeSelf) playButton.SetActive(false);
                if(draggablesMenu.activeSelf) draggablesMenu.SetActive(false);
                if(!restartButton.activeSelf) restartButton.SetActive(true);
                if(resultsMenu.activeSelf) resultsMenu.SetActive(false);
                break;
            case GamePhase.Solved:
                gameOverText.text = "Pizzas Delivered";
                if(exitButton.activeSelf) exitButton.SetActive(false);
                if(playButton.activeSelf) playButton.SetActive(false);
                if(draggablesMenu.activeSelf) draggablesMenu.SetActive(false);
                if(restartButton.activeSelf) restartButton.SetActive(false);
                if(!resultsMenu.activeSelf) resultsMenu.SetActive(true);
                break;
            case GamePhase.Failed:
                gameOverText.text = "Cold Pizza";
                if(exitButton.activeSelf) exitButton.SetActive(false);
                if(playButton.activeSelf) playButton.SetActive(false);
                if(draggablesMenu.activeSelf) draggablesMenu.SetActive(false);

                if (!LevelManager.Instance.NextLevelUnlocked())
                {
                    if(nextButton.activeSelf) nextButton.SetActive(false); 
                }
                
                if(restartButton.activeSelf) restartButton.SetActive(false);
                if(!resultsMenu.activeSelf) resultsMenu.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
