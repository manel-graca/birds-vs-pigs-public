using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class G_Ball_UI_ResetPasswordPanel : MonoBehaviour
{
    [SerializeField] InputField emailToRecover;
    [SerializeField] TextMeshProUGUI messageText;

    public string GetEmailToReset()
    {
        return emailToRecover.text;
    }

    public void ThrowMessage(string message, Color color)
    {
        messageText.color = color;
        messageText.text = message;
    }

    public void CloseWindow()
    {
       Invoke("DoCloseWindow", 2.5f);
    }

    void DoCloseWindow()
    {
        var cg = GetComponent<CanvasGroup>();
        cg.interactable =false;
        cg.blocksRaycasts = false;
        cg.alpha = 0f;
    }
}
