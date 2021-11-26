using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_Ball_PlayerData : MonoBehaviour
{
    public static G_Ball_PlayerData instance;

    [SerializeField] private int maxExperienceThisLevel;
    [SerializeField] private int maxEnergy;
    
    [SerializeField] private int diamondsEarnedPerLevel;

    private string playerName = "unknown";
    private int currentExperience;
    private int currentEnergy;
    private int currentDiamonds;
    [SerializeField] private int playerLevel;

    private void Awake()
    {
        if (instance == null){

            instance = this;
    
    
        } else {
            Destroy(this);
        }
    }

    public void SetPlayerName(string name)
    {
        playerName = name;
    }

    public void AddToExperience(int amount)
    {
        currentExperience += amount;
    }

    public void AddSubtractDiamonds(int amount)
    {
        currentDiamonds += amount;
    }

    public void AddSubtractEnergy(int amount)
    {
        currentEnergy += amount;
    }

    private void LevelUp()
    {
        playerLevel++;
        AddSubtractDiamonds(diamondsEarnedPerLevel);
        currentEnergy = maxEnergy;
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    public string GetCurrentExperienceString()
    {
        var text = $"{currentExperience} / {maxExperienceThisLevel}";
        return text;
    }

    public int GetCurrentDiamonds()
    {
        return currentDiamonds;
    }

    public string GetCurrentEnergyString()
    {
        var text = $"{currentEnergy} / {maxEnergy}";
        return text;
    }

    public int GetPlayerLevel()
    {
        return playerLevel;
    }
}