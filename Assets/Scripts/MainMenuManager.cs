using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public string firstLevel;
    public string levelSelector;

    private void Start()
    {
        LevelManager.Instance.UnlockLevel(firstLevel);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(firstLevel);
    }

    public void OpenLevelSelector()
    {
        SceneManager.LoadScene(levelSelector);
    }
}
