using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogicManager : MonoBehaviour
{
    private int _goalCount = 0;

    [SerializeField] private bool _isThanksScreen = false;
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
        if (_isThanksScreen)
        {
            Phase = GamePhase.PlayOut;
        }
        else
        {
            Phase = GamePhase.Layout;
            OnPhaseChanged?.Invoke(Phase);
        }
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

            try
            {
                var nextLevelName = SceneUtility.GetScenePathByBuildIndex(SceneManager.GetActiveScene().buildIndex + 1);
                var pathParts = nextLevelName.Split('/');
                nextLevelName = pathParts[^1].Replace(".unity", "");

                LevelManager.Instance.UnlockLevel(nextLevelName);
            }
            catch(Exception e)
            {
                Debug.LogException(e);
            }

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

    public void ExitToMain()
    {
        SceneManager.LoadScene(0);
    }
    
    public void GameFailed()
    {
        Phase = GamePhase.Failed;
        OnPhaseChanged?.Invoke(Phase);
    }
}

public enum GamePhase
{
    Layout,
    PlayOut,
    Solved,
    Failed
}
