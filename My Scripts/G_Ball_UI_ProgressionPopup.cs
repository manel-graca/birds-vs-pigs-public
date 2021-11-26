using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class G_Ball_UI_ProgressionPopup : MonoBehaviour
{
    [SerializeField] private GameObject noInternetPopupBody;
    [SerializeField] private GameObject yesInternetPopupBody;
    
    [SerializeField] private CanvasGroup bgCg;
    [SerializeField] private CanvasGroup cg;
    [SerializeField] private SleepyFrogGames_Utilities utilities;
    
    private bool dontShowAgain;

    private void Awake()
    {
        dontShowAgain = false;
        
        if (PlayerPrefs.HasKey("UI_POPUP_HIDE_PROGRESSION_INFO_POPUP"))
        {
            switch (PlayerPrefs.GetInt("UI_POPUP_HIDE_PROGRESSION_INFO_POPUP"))
            {
                case 0:
                    dontShowAgain = false;
                    Debug.Log("can show");
                    break;
                case 1:
                    dontShowAgain = true;
                    Debug.Log("can NOT show");
                    break;
            }
        }
        else
        {
            dontShowAgain = false;
        }
    }

    public void EnablePopupIfNoInternet(bool enable)
    {
        if (!PlayerPrefs.HasKey("HAS_LOGIN") && enable)
        {
            utilities.LeanCanvasGroupAlpha(true, 0.85f, bgCg, 0.1f);
            utilities.LeanInOutCanvasGroup(true, cg, 0.2f);
        
            yesInternetPopupBody.SetActive(false);
            noInternetPopupBody.SetActive(true);
        }
        else
        {
            utilities.LeanCanvasGroupAlpha(false, 0.85f, bgCg, 0.1f);
            utilities.LeanInOutCanvasGroup(false, cg, 0.2f);
            yesInternetPopupBody.SetActive(false);
            noInternetPopupBody.SetActive(false);
        }
    }
    
    public void EnableProgressionInfoPanel(bool enable)
    {
        if(dontShowAgain && enable) return;
        
        var bgCg = cg.GetComponentInChildren<CanvasGroup>();
        utilities.LeanCanvasGroupAlpha(enable, 0.85f, bgCg, 0.1f);
        utilities.LeanInOutCanvasGroup(enable, cg, 0.2f);
        yesInternetPopupBody.SetActive(true);
        noInternetPopupBody.SetActive(false);
    }

    public void DontShowPopupAgain(bool a)
    {
        dontShowAgain = a;
        Debug.Log(dontShowAgain + " dont show again");

        if (dontShowAgain)
        {
            PlayerPrefs.SetInt("UI_POPUP_HIDE_PROGRESSION_INFO_POPUP", 1);
        }
        else
        {
            PlayerPrefs.SetInt("UI_POPUP_HIDE_PROGRESSION_INFO_POPUP", 0);
        }
    }
    
}
