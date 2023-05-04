using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEditor;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    
    private List<string> _unlockedLevels;

    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _unlockedLevels = new List<string>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UnlockLevel(string levelName)
    {
        _unlockedLevels.Add(levelName);
    }

    public bool NextLevelUnlocked()
    {
  
        var nextName = SceneUtility.GetScenePathByBuildIndex(SceneManager.GetActiveScene().buildIndex + 1);
        var pathParts = nextName.Split('/');
        nextName = pathParts[^1].Replace(".unity", "");

        return _unlockedLevels.FirstOrDefault(x => x.Equals(nextName)) != null;
    }

    public bool LevelUnlocked(string path)
    {
        return _unlockedLevels.FirstOrDefault(x => x.Equals(path)) != null;
    }
}
