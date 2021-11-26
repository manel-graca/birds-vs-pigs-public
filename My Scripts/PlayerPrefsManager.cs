using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour
{
    public static PlayerPrefsManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }
    
    #region PLAYER PREFS
    
    public void SaveLevelHighScore(LevelData levelData, int score)
    {
        var key = levelData.levelID + "_HIGHSCORE";
        var previousHighscore = PlayerPrefs.GetInt(key);
        var scoreToUpdate = score;
        
        if (score <= previousHighscore)
        {
            scoreToUpdate = previousHighscore;
        }
        PlayerPrefs.SetInt(key, scoreToUpdate);
        levelData.levelDataClass.highscore = scoreToUpdate;
        Debug.Log("SAVING SCORE: " + scoreToUpdate);
    }

    public void SaveLevelStarsEarned(LevelData levelData, int stars)
    {
        var key = levelData.levelID + "_STARS";
        var value = stars;

        PlayerPrefs.SetInt(key, value);
        levelData.levelDataClass.stars = stars;
    }

    public void SaveLevelIsLocked(LevelData levelData, bool locked)
    {
        var key = levelData.levelID + "_LOCKED";
        string value;
        if (locked)
        {
            value = "True";
        }
        else
        {
            value = "False";
        }
        
        PlayerPrefs.SetString(key, value);
        levelData.levelDataClass.locked = locked;
    }

    public void SaveLevelCompleted(LevelData levelData, bool completed)
    {
        var key = levelData.levelID + "_COMPLETED";
        string value;

        if (completed)
        {
            value = "True";
        }
        else
        {
            value = "False";
        }
        
        PlayerPrefs.SetString(key, value);
        levelData.levelDataClass.completed = completed;
    }
    
    #endregion
    
    public bool GetIfLevelLocked(string level) // returns TRUE if level LOCKED      or  returns FALSE if level UNLOCKED
    {
        if (!PlayerPrefs.HasKey(level + "_LOCKED"))
        {
            return true; // in case key doesn't exist we return level IS LOCKED
        }

        var key = level + "_LOCKED";
        var value = PlayerPrefs.GetString(key);
        
        if (value == "True")
        {
            return true;
        }
        if (value == "False")
        {
            return false;
        }
        return true;
    }
    public bool GetIfLevelCompleted(string level)
    {
        if (!PlayerPrefs.HasKey(level + "_COMPLETED")) return false; // in case key doesn't exist we return level is UNCOMPLETED
        
        var key = level + "_COMPLETED";
        var value = PlayerPrefs.GetString(key);
        
        if (value == "True")
        {
            return true;
        }

        if (value == "False")
        {
            return false;
        }
        return false;
    }
    public int GetLevelStars(string level)
    {
        if (!PlayerPrefs.HasKey(level + "_STARS")) return 0;

        var key = level + "_STARS";
        var value = PlayerPrefs.GetInt(key);
        return value;
    }

    public int GetLevelHighScore(string level)
    {
        if (!PlayerPrefs.HasKey(level + "_HIGHSCORE")) return 0;
        var key = level + "_HIGHSCORE";
        var value = PlayerPrefs.GetInt(key);
        return value;
    }
}