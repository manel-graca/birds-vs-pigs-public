using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class G_Ball_StageMaster : MonoBehaviour
{
    public int stageIndex;
    public bool isLocked;
    public int requiredLevel;
    [Space]
    public Image lockImage;
    public TextMeshProUGUI lockedText;
    [Space]
    [SerializeField] private CanvasGroup levelsHolder;
    [SerializeField] private CanvasGroup stagesHolder;
    
    public G_Ball_StageLevel[] ui_stageLevels;
    public LevelData[] levels;
    
    [SerializeField] Button button;
    G_Ball_PlayerData pData;
    
    private void Start()
    {
        pData = G_Ball_PlayerData.instance;
        OnLevelStageWindowLoaded();
        button.onClick.AddListener(ShowStageLevels);
    }

    public bool GetIfCanUnlock()
    {
        if (pData.GetPlayerLevel() >= requiredLevel)
        {
            return true;
        }
        return false;
    }

    public void OnLevelStageWindowLoaded()
    {
        if (GetIfCanUnlock())
        {
            isLocked = false;
            lockImage.gameObject.SetActive(false);
            lockedText.gameObject.SetActive(false);
        }
        else
        {
            isLocked = true;
            lockImage.gameObject.SetActive(true);
            lockedText.gameObject.SetActive(true);
        }
    }

    public void OnMenuPlayButton()
    {
        foreach (var level in ui_stageLevels)
        {
            level.UpdateLevelsInfo();
        }
    }

    private void ShowStageLevels()
    {
        if(levelsHolder == null) return;
        
        stagesHolder.interactable = false;
        stagesHolder.blocksRaycasts = false;
        
        var targetScale = Vector2.one;
        levelsHolder.transform.localScale = Vector2.zero;
        
        levelsHolder.LeanAlpha(1f,0.1f);
        levelsHolder.transform.LeanScale(targetScale,0.1f).setEaseInOutBack();
        
        levelsHolder.interactable = true;
        levelsHolder.blocksRaycasts = true;
    }
    public void HideStageLevels()
    {
        if(levelsHolder == null) return;
        
        var targetScale = Vector2.zero;
        levelsHolder.transform.LeanScale(targetScale,0.1f);
        
        stagesHolder.interactable = true;
        stagesHolder.blocksRaycasts = true;


        levelsHolder.LeanAlpha(0f,0.1f);
        levelsHolder.interactable = false;
        levelsHolder.blocksRaycasts = false;
    }
}
