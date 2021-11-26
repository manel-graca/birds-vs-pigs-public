using System;
using Animancer;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class G_Ball_MainMenu : MonoBehaviour
{
    public static G_Ball_MainMenu instance;
    
    [SerializeField] private TextMeshProUGUI diamondsText;
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private TextMeshProUGUI currentLevelText;
    [SerializeField] private TextMeshProUGUI experienceBarText;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private GameObject movingClouds;
    
    [SerializeField] CanvasGroup stageSelection;
    [SerializeField] CanvasGroup mainMenuCg;
    
    [SerializeField] G_Ball_UI_ProgressionPopup progressionPopup;
    
    G_Ball_PlayerData pData;
    G_Ball_StageMaster stageMaster;
    PlayfabManager playfabManager;
    G_Ball_SettingsManager settings;
    G_Ball_GameManager gameManager;
    SoundManager soundManager;
    
    [SerializeField] SleepyFrogGames_Utilities utilities;
    
    private void Awake()
    {
        instance = this;
        pData = G_Ball_PlayerData.instance;
        stageMaster = FindObjectOfType<G_Ball_StageMaster>();
    }

    private void Start()
    {
        playfabManager = PlayfabManager.instance;
        gameManager = G_Ball_GameManager.instance;
        settings = G_Ball_SettingsManager.instance;
        soundManager = SoundManager.instance;
        UpdateUiElements();
        Invoke(nameof(UpdateAuthenticationPanels), 0.5f);
        PlayMusic();
    }

    public void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    public void ClearSpecificPlayerPrefsKey(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.DeleteKey(key);
        }
    }

    private void PlayMusic()
    {
        var clip = soundManager.audioData.gameTheme;
        soundManager.PlayMusicSound(clip);
    }

    public void TurnCloudsOnOff(bool on)
    {
        movingClouds.SetActive(on);
    }
    
    void UpdateUiElements()
    {
        playerNameText.text = pData.GetPlayerName();
        currentLevelText.text = pData.GetPlayerLevel().ToString();
        experienceBarText.text = pData.GetCurrentExperienceString();
        diamondsText.text = pData.GetCurrentDiamonds().ToString();
        energyText.text = pData.GetCurrentEnergyString();
    }

    void UpdateAuthenticationPanels()
    {
        if (!gameManager.GetIfHasInternet()) // NO INTERNET
        {
            progressionPopup.EnablePopupIfNoInternet(true);
            return;
        }

        if (gameManager.GetIfLoggedIn())
        {
            if (PlayerPrefs.HasKey("REMEMBER_ME_ID"))
            {
                settings.OnLoginAccount();
                progressionPopup.EnableProgressionInfoPanel(false);
            }
            else
            {
                settings.OnFirstAnonymousLoginAccount();
                progressionPopup.EnableProgressionInfoPanel(true);
            }
        }
    }

    
    public void ShowStageSelection()
    {
        TurnCloudsOnOff(false);
        
        stageSelection.LeanAlpha(1f,0.1f);
        stageSelection.interactable = true;
        stageSelection.blocksRaycasts = true;
        
        mainMenuCg.interactable = false;
        mainMenuCg.blocksRaycasts = false;
        
        //load level info
        stageMaster.OnMenuPlayButton();
    }

    public void HideStageSelection()
    {
        var levelWindow = GameObject.FindGameObjectWithTag("UI_LevelsWindow");
        if (levelWindow.GetComponent<CanvasGroup>())
        {
            var cg = levelWindow.GetComponent<CanvasGroup>();
            cg.alpha = 0f;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }
        
        stageSelection.LeanAlpha(0f,0.1f);
        stageSelection.interactable = false;
        stageSelection.blocksRaycasts = false;
        
        mainMenuCg.interactable = true;
        mainMenuCg.blocksRaycasts = true;
        
        TurnCloudsOnOff(true);

    }
}