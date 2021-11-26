using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class G_Ball_StageLevel : MonoBehaviour
{
    [SerializeField] private Button button;
    
    public LevelData level;
    
    [SerializeField] private TextMeshProUGUI levelNumberText;
    [SerializeField] private GameObject[] starsObjects;
    [SerializeField] private GameObject lockImage;
    
    private float timeSinceCheck = Mathf.Infinity;
    private float timeBetweenChecks = 0.25f;

    private void Start()
    {
        levelNumberText.text = "LEVEL " + level.levelIndex;
        Invoke(nameof(UpdateLevelsInfo), 0.5f);
    }

    private void Update()
    {
        if (timeSinceCheck > timeBetweenChecks)
        {
            timeSinceCheck = 0;
            UpdateLevelsInfo();
        }
        timeSinceCheck += Time.deltaTime;
    }

    public void LoadLevelScene()
    {
        if (!GetIfCanPlayLevel()) return;
        button.interactable = false;
        
        G_Ball_GameManager.instance.SetLiveLevel(level);
        
        SceneManager.LoadScene(level.sceneName);
    }

    private bool GetIfCanPlayLevel()
    {
        if (level.levelDataClass.locked)
        {
            return false;
        }
        return true;
    }

    void DisableAllStars()
    {
        foreach (var star in starsObjects)
        {
            star.SetActive(false);
        }
    }

    public void UpdateLevelsInfo()
    {
        if (level != null)
        {
            var isLocked = level.levelDataClass.locked;
            var isCompleted = level.levelDataClass.completed;
            var stars = level.levelDataClass.stars;
            
            if (isLocked)
            {
                lockImage.SetActive(true);
                DisableAllStars();
                button.interactable = false;
                return;
            }

            if (!isCompleted)
            {
                DisableAllStars();
                level.levelDataClass.stars = 0;
                lockImage.SetActive(false);
                button.interactable = true;
                return;
            }

            DisplayStarsEarned(stars); // IN CASE LEVEL IS COMPLETED
            button.interactable = true;
            lockImage.SetActive(false);
            return;

        }
        lockImage.SetActive(true);
        button.interactable = false;
        DisableAllStars();
    }

    private void DisplayStarsEarned(int starsEarned)
    {
        switch (starsEarned)
        {
            case 1:
                starsObjects[0].SetActive(true);
                starsObjects[1].SetActive(false);
                starsObjects[2].SetActive(false);
                break;
            case 2: 
                starsObjects[0].SetActive(true);
                starsObjects[1].SetActive(true);
                starsObjects[2].SetActive(false);
                break;
            case 3:
                starsObjects[0].SetActive(true);
                starsObjects[1].SetActive(true);
                starsObjects[2].SetActive(true);
                break;
        }
    }
}
