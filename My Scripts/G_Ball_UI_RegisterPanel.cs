using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class G_Ball_UI_RegisterPanel : MonoBehaviour
{
    [SerializeField] InputField usernameInput;
    [SerializeField] InputField emailInput;
    [SerializeField] InputField passwordInput;

    
    [SerializeField] string privacyPolicyLink;
    [SerializeField] Toggle privacyPolicyToggle;
    [SerializeField] Button registerButton;
    
    [SerializeField] GameObject warningMessage;

    [SerializeField] SleepyFrogGames_Utilities utilities;
    
    public void ThrowMessageError(string errorMessage)
    {
        warningMessage.GetComponent<TextMeshProUGUI>().text = errorMessage;
        warningMessage.SetActive(true);
    }

    public void OpenPrivacyPolicy()
    {
        Application.OpenURL(privacyPolicyLink);
    }

    private void Update()
    {
        if (privacyPolicyToggle.isOn)
        {
            registerButton.interactable = true;
        }
        else
        {
            registerButton.interactable = false;
        }
    }

    public void Register()
    {
        PlayfabManager.instance.RegisterButton();
    }

    public string GetRegisterUsername()
    {
        return usernameInput.text;
    }
    public string GetRegisterEmail()
    {
        return emailInput.text;
    }
    public string GetRegisterPassword()
    {
        return passwordInput.text;
    }

}
