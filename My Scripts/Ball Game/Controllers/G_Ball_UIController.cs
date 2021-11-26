using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class G_Ball_UIController : MonoBehaviour
{
    #region Singleton

    public static G_Ball_UIController instance;

    #endregion

    [BoxGroup("ELEMENT ARRAYS", CenterLabel = true)] [Title("Only Visible During Gameplay")]
    [SerializeField] GameObject[] gameplayOnlyUIElements;

    [BoxGroup("TEXT ELEMENTS", CenterLabel = true)] [Title("Player Score")]
    [SerializeField] TextMeshProUGUI currentScoreText;
    [BoxGroup("TEXT ELEMENTS")]
    [SerializeField] TextMeshProUGUI winPanelScoreText;
    [BoxGroup("TEXT ELEMENTS")]
    [SerializeField] TextMeshProUGUI losePanelScoreText;
    [BoxGroup("TEXT ELEMENTS")] [Title("Ball Related")]
    [SerializeField] TextMeshProUGUI ballNameText;

    [BoxGroup("PANELS", CenterLabel = true)] [Title("Level State Panels")]
    [SerializeField] CanvasGroup winPanel;
    [BoxGroup("PANELS")]
    [SerializeField] CanvasGroup losePanel;
    [BoxGroup("PANELS")]
    [SerializeField] CanvasGroup pausePanel;
    [BoxGroup("PANELS")][Title("Settings")]
    [SerializeField] CanvasGroup settingsPanelCg;
    [BoxGroup("PANELS")]
    [SerializeField] GameObject settingsPanelBody;
    [BoxGroup("PANELS")]
    [SerializeField] CanvasGroup settingsPanelBackgroundCg;
    
    [BoxGroup("BUTTONS", CenterLabel = true)] [Title("Gameplay Visible Buttons")]
    [SerializeField] GameObject isMutedButton;
    [BoxGroup("BUTTONS")]
    [SerializeField] GameObject isUnmutedButton;
    [BoxGroup("BUTTONS")]
    [SerializeField] Button winPanelContinueButton;
    

    [BoxGroup("POPUPS VERIFICATION", CenterLabel = true)] [Title("Return To Menu")]
    [SerializeField] CanvasGroup returnToMenuPopupCanvasGroup;
    [BoxGroup("POPUPS VERIFICATION")]
    [SerializeField] GameObject returnToMenuPopupBody;
    [BoxGroup("POPUPS VERIFICATION")]
    [SerializeField] GameObject returnToMenuPopupBackground;
    [BoxGroup("POPUPS VERIFICATION")][Title("Restart Level")]
    [SerializeField] CanvasGroup restartPopupCanvasGroup;
    [BoxGroup("POPUPS VERIFICATION")]
    [SerializeField] GameObject restartPopupBody;
    

    [BoxGroup("LEVEL STARTING ELEMENTS", CenterLabel = true)]
    [SerializeField] CanvasGroup startLevelFader;
    
    
    [SerializeField] G_Ball_GameManager manager;
    [SerializeField] SleepyFrogGames_Utilities utilitiesGUI;
    
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (ballNameText != null && currentScoreText != null)
        {
            currentScoreText.text = 0.ToString();
            ballNameText.text = String.Empty;
        }

        if (startLevelFader != null)
        {
            startLevelFader.alpha = 1f;
            startLevelFader.LeanAlpha(0f,.75f).setOnComplete(OnFadeComplete).delay=(1f);
        }
        
        Invoke(nameof(DisableInputBlockAtStart), 0.6f);
    }
    void DisableInputBlockAtStart()
    {
        manager.EnableGlobalInputBlocker(false);
    }

    void OnFadeComplete()
    {
        startLevelFader.interactable = false;
        startLevelFader.blocksRaycasts = false;
    }
    public void UpdateScoreboard(int amount)
    {
        if (currentScoreText == null) return;

        currentScoreText.text = amount.ToString();
    }

    public void UpdateWinPanelScoreboard(int amount)
    {
        string fixedText = "SCORE: ";
        string text = fixedText + amount.ToString();
        
        winPanelScoreText.text = text;
    }

    public void UpdateLosePanelScoreboard(int amount)
    {
        string text = String.Format("SCORE:{0}", amount.ToString());
        
        losePanelScoreText.text = text;
    }

    public void UpdateBallName(G_Ball_DefaultBall ball)
    {
        if(ballNameText == null) return;
        
        ballNameText.text = ball.ballType.ballName;
    }
    
    public void DisableEnableGameplayOnlyUIElements(bool activate)
    {
        foreach (var element in gameplayOnlyUIElements)
        {
            element.SetActive(activate);
        }
    }
    
    public void UIOnLevelStart()
    {
        G_Ball_GameManager.instance.isInputBlock = false;
        ShowHideWinPanel(false);
        ShowHideLosePanel(false);
        ShowHideSettingsPanel(false);
        ShowHidePausePanel(false);
    }
    public void ShowHideWinPanel(bool activate) // WIN PANEL
    {
        ShowHidePausePanel(false);
        if (activate)
        {
            utilitiesGUI.InstantShowHideCanvasGroup(true, winPanel);
            DisableEnableGameplayOnlyUIElements(false);
        }
        else
        {
            utilitiesGUI.InstantShowHideCanvasGroup(false, winPanel);
        }
    }
    public void ShowHideLosePanel(bool activate) // LOSE PANEL
    {
        ShowHidePausePanel(false);

        if (activate)
        {
            utilitiesGUI.InstantShowHideCanvasGroup(true, losePanel);
            manager.OnLevelLostBehaviour();
        }
        else
        {
            DisableEnableGameplayOnlyUIElements(false);
            utilitiesGUI.InstantShowHideCanvasGroup(false, losePanel);
        }
    }

    public void ShowHidePausePanel(bool activate) // PAUSE PANEL
    {
        if (activate)
        {
            utilitiesGUI.LeanInOutCanvasGroup(true,pausePanel,0.075f);
            DisableEnableGameplayOnlyUIElements(false);
            manager.EnableGlobalInputBlocker(true);
        }
        else
        {
            DisableEnableGameplayOnlyUIElements(true);
            if (pausePanel.alpha > 0f)
            {
                utilitiesGUI.LeanInOutCanvasGroup(false,pausePanel,0.075f);
            }
            manager.EnableGlobalInputBlocker(false);
        }
    }

    public void ShowHideSettingsPanel(bool activate) // SETTINGS PANEL
    {
        if (activate)
        {
            G_Ball_SettingsManager.instance.OnSettingsOpened();
            utilitiesGUI.InstantShowHideCanvasGroup(true, settingsPanelCg);
            utilitiesGUI.LeanInOutCanvasGroup(true,settingsPanelBackgroundCg,0.1f);
            
            Vector2 target = Vector2.one;
            settingsPanelBody.transform.localScale = Vector3.zero;
            
            settingsPanelBody.transform.LeanScale(target,0.175f).setEaseOutBack().delay = 0.085f;
            
        }
        else
        {
            utilitiesGUI.LeanInOutCanvasGroup(false,settingsPanelBackgroundCg,0.1f);
            settingsPanelBody.transform.LeanScale(Vector2.zero,0.175f).setEaseInBack().delay = 0.085f;
            utilitiesGUI.LeanInOutCanvasGroupWithDelay(false,settingsPanelCg,0.1f,0.1f);
        }
    }

    public void DisplayStarsEarnedWinPanel(GameObject[] starsObjects, GameObject[] ps, Color offColor, Color defaultColor, int stars)
    {
        switch (stars)
        {
            case 1:
                starsObjects[0].SetActive(true);
                starsObjects[1].SetActive(false);
                starsObjects[2].SetActive(false);

                for (int i = ps.Length; i <= 1; i--)
                {
                    ps[i].SetActive(false);
                }
                for (int j = 0; j < 1; j++)
                {
                    ps[j].SetActive(true);
                }
                break;
            case 2:
                starsObjects[0].SetActive(true);
                starsObjects[1].SetActive(true);
                starsObjects[2].SetActive(false);
                
                for (int i = ps.Length; i <= 3; i--)
                {
                    ps[i].SetActive(false);
                }
                for (int j = 0; j < 3; j++)
                {
                    ps[j].SetActive(true);
                }
                break;
            case 3:
                starsObjects[0].SetActive(true);
                starsObjects[1].SetActive(true);
                starsObjects[2].SetActive(true);

                for (int j = 0; j < ps.Length; j++)
                {
                    ps[j].SetActive(true);
                }
                break;
        }
    }
    
    public void HandleMuteButton() // Handled & called by settings manager
    {
        if (G_Ball_SettingsManager.instance.isGlobalMuted)
        {
            isUnmutedButton.SetActive(false);
            isMutedButton.SetActive(true);
            return;
        }
        isMutedButton.SetActive(false);
        isUnmutedButton.SetActive(true);
    }
    
    public void NormalResumeGame()
    {
        ShowHidePausePanel(false);
        ShowHideLosePanel(false);
        ShowHideWinPanel(false);
    }
    
    public void ShowReturnMenuVerification(bool activate)
    {
        var backgroundCg = returnToMenuPopupBackground.GetComponent<CanvasGroup>();
        
        if (!activate)
        {
            utilitiesGUI.LeanInOutCanvasGroup(false, backgroundCg,  0.1f);
            
            returnToMenuPopupBody.LeanScale(new Vector2(0f,0f), 0.2f);
            
            utilitiesGUI.LeanInOutCanvasGroup(true,pausePanel,0.15f);
            return;
        }
        var targetScale = new Vector2(1f,1f);
        
        returnToMenuPopupBody.transform.localScale = Vector2.zero;

        utilitiesGUI.LeanInOutCanvasGroup(true, backgroundCg,0.1f);
        utilitiesGUI.InstantShowHideCanvasGroup(true, returnToMenuPopupCanvasGroup);
        
        pausePanel.interactable = false;
        pausePanel.blocksRaycasts = false;
        
        returnToMenuPopupBody.transform.DOScale(targetScale,0.4f).SetEase(Ease.OutBounce);
    }
    
    public void ShowRestartVerification(bool activate)
    {
        if (!activate)
        {
            restartPopupBody.LeanScale(new Vector2(0f,0f), 0.2f);
            
            utilitiesGUI.LeanInOutCanvasGroup(true,pausePanel,0.15f);
            return;
        }
        var targetScale = new Vector2(1f,1f);
        
        restartPopupBody.transform.localScale = Vector2.zero;

        utilitiesGUI.InstantShowHideCanvasGroup(true, restartPopupCanvasGroup);
        
        pausePanel.interactable = false;
        pausePanel.blocksRaycasts = false;
        
        restartPopupBody.transform.DOScale(targetScale,0.4f).SetEase(Ease.OutBounce);
    }
    
    
    

}