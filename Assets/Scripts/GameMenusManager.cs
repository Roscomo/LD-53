using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenusManager : MonoBehaviour
{

    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private GameObject resultsMenu;
    [SerializeField] private TMP_Text _levelText;
    
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
                if(!playButton.activeSelf) playButton.SetActive(true);
                if(restartButton.activeSelf) restartButton.SetActive(false);
                if(resultsMenu.activeSelf) resultsMenu.SetActive(false);
                break;
            case GamePhase.PlayOut:
                if(playButton.activeSelf) playButton.SetActive(false);
                if(!restartButton.activeSelf) restartButton.SetActive(true);
                if(resultsMenu.activeSelf) resultsMenu.SetActive(false);
                break;
            case GamePhase.Solved:
                if(playButton.activeSelf) playButton.SetActive(false);
                if(restartButton.activeSelf) restartButton.SetActive(false);
                if(!resultsMenu.activeSelf) resultsMenu.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
