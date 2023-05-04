using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private string level;
    [SerializeField] private GameObject lockSprite;

    private Button _button;

    private void LoadLevel()
    {
        SceneManager.LoadScene(level);
    }

    private void Start()
    {
        if (!level.Equals(string.Empty) && LevelManager.Instance.LevelUnlocked(level))
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(LoadLevel);
            lockSprite.SetActive(false);
        }
    }
}
