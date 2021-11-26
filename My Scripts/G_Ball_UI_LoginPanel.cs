using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class G_Ball_UI_LoginPanel : MonoBehaviour
{
    [SerializeField] InputField emailInput;
    [SerializeField] InputField passwordInput;
    [SerializeField] GameObject warningMessage;
    
    [SerializeField] CanvasGroup resetPasswordCanvasGroup;
    
    [SerializeField] SleepyFrogGames_Utilities utilities;

    private string email;
    private string password;

    public void Login()
    {
        if (passwordInput.text.Length < 5)
        {
            ThrowMessageError("Password is too short");
            return;
        }
        PlayfabManager.instance.PlayerAccountLoginButton();
    }
    
    public void ThrowMessageError(string errorMessage)
    {
        warningMessage.GetComponent<TextMeshProUGUI>().text = errorMessage;
        warningMessage.SetActive(true);
    }

    public string GetLoginEmail()
    {
        return email;
    }

    public string GetLoginPassword()
    {
        return password;
    }

    public void ShowHideResetPasswordPanel(bool activate)
    {
        utilities.LeanInOutCanvasGroup(activate, resetPasswordCanvasGroup, 0.1f);
    }

    public void OnEmailChanged(string input)
    {
        email = input;
        Debug.Log(input);
    }

    public void OnPasswordChanged(string input)
    {
        password = input;
        Debug.Log(input);
    }
}
