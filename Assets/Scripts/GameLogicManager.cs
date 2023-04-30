using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogicManager : MonoBehaviour
{
    private int _goalCount = 0;
    
    
    public event Action<GamePhase> OnPhaseChanged;
    
    public static GameLogicManager Instance { get; private set; }
    public GamePhase Phase { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Phase = GamePhase.Layout;
        OnPhaseChanged?.Invoke(Phase);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RegisterGoal(Goal toRegister)
    {
        _goalCount += 1;
        toRegister.OnGoalAccomplished += DeregisterGoal;
    }

    private void DeregisterGoal(Goal toDeregister)
    {
        _goalCount -= 1;
        toDeregister.OnGoalAccomplished -= DeregisterGoal;
        
        if (Phase is GamePhase.PlayOut && _goalCount <= 0)
        {
            Phase = GamePhase.Solved;
            OnPhaseChanged?.Invoke(Phase);
        }
    }

    public void StartVisualization()
    {
        Phase = GamePhase.PlayOut;
        OnPhaseChanged?.Invoke(Phase);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}

public enum GamePhase
{
    Layout,
    PlayOut,
    Solved
}
