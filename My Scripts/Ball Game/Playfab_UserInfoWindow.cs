using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Playfab_UserInfoWindow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI usernameText;
    [SerializeField] TextMeshProUGUI playfabIDText;
    [SerializeField] TextMeshProUGUI accountCreateDateText;

    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] SleepyFrogGames_Utilities utilities;

    public void UpdateTextValues(string username, string id, string date)
    {
        usernameText.text = username;
        playfabIDText.text = id;
        accountCreateDateText.text = date;
    }

    public void ShowWindow()
    {
        PlayfabManager.instance.GetAccountInfo();
        Invoke(nameof(DoShowWindow), 0.5f);
    }

    void DoShowWindow()
    {
        utilities.LeanInOutCanvasGroup(true, canvasGroup, 0.1f);
    }

    public void HideWindow()
    {
        utilities.LeanInOutCanvasGroup(false, canvasGroup, 0.1f);
    }
}