using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.AdminModels;
using UnityEngine;

public class PlayfabManager : MonoBehaviour
{
    public static PlayfabManager instance;

    private void Awake()
    {
        instance = this;
    }

    [SerializeField] G_Ball_UI_RegisterPanel registerPanel;
    [SerializeField] G_Ball_UI_LoginPanel loginPanel;
    [SerializeField] G_Ball_UI_ResetPasswordPanel resetPasswordPanel;

    string playerPlayfabID;
    

    G_Ball_SettingsManager settings;

    private void Start()
    {
        settings = G_Ball_SettingsManager.instance;
    }

    public string GetUserPlayFabID()
    {
        if (!string.IsNullOrEmpty(playerPlayfabID))
        {
            return playerPlayfabID;
        }

        return null;
    }

    #region LOGIN

    public void SetPlayFabID(string ID)
    {
        playerPlayfabID = ID;
    }

    public void PlayerAccountLoginButton()
    {
        string email = loginPanel.GetLoginEmail();
        string password = loginPanel.GetLoginPassword();

        var request = new LoginWithEmailAddressRequest
        {
            Email = email,
            Password = password
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, result =>
        {
            playerPlayfabID = result.PlayFabId;
            /*if (rememberMe)
            {
                var linkRequest = new LinkCustomIDRequest
                {
                    CustomId = SystemInfo.deviceUniqueIdentifier,
                    ForceLink = true
                };
                PlayFabClientAPI.LinkCustomID(linkRequest, linkSuccess =>
                {
                    Debug.Log("account linked");
                }, error =>
                {
                    Debug.Log(error.GenerateErrorReport());
                    Debug.Log(error.ErrorMessage);
                });
            }*/
            PlayerPrefs.SetString("REMEMBER_ME_ID", SystemInfo.deviceUniqueIdentifier);
            loginPanel.ThrowMessageError("Login success");
            settings.CloseLoginPanelDelay(1f);
            settings.OnLoginAccount();
        }, OnLoginError);
    }

    void OnLoginError(PlayFabError error)
    {
        loginPanel.ThrowMessageError(error.ErrorMessage);
        Debug.Log(error.GenerateErrorReport());
        Debug.Log(error.ErrorMessage);
    }

    #endregion

    #region REGISTER

    public void RegisterButton()
    {
        string user = registerPanel.GetRegisterUsername();
        string email = registerPanel.GetRegisterEmail();
        string password = registerPanel.GetRegisterPassword();

        var request = new AddUsernamePasswordRequest()
        {
            Username = user,
            Email = email,
            Password = password,
        };
        PlayFabClientAPI.AddUsernamePassword(request, OnRegisterSuccess, OnRegisterError);
    }

    void OnRegisterSuccess(AddUsernamePasswordResult result)
    {
        registerPanel.ThrowMessageError("Account registered successfully");
        settings.OnRegisterAccount();
        Logout();
        settings.CloseRegisterPanelDelay(1.5f);
    }

    void OnRegisterError(PlayFabError error)
    {
        registerPanel.ThrowMessageError(error.ErrorMessage);
        Debug.Log(error.GenerateErrorReport());
    }

    #endregion

    #region LOGOUT

    public void Logout()
    {
        PlayFabClientAPI.ForgetAllCredentials();
        PlayerPrefs.SetString("REMEMBER_ME_ID", String.Empty);
        settings.OnLogoutAccount();
    }

    #endregion

    #region DELETE ACCOUNT

    public void DeleteAccount()
    {
        GetAccountInfo();
        Invoke(nameof(DoDelete), 1f);
    }

    void DoDelete()
    {
        var request = new DeleteMasterPlayerAccountRequest()
        {
            PlayFabId = playerPlayfabID
        };
        PlayFabAdminAPI.DeleteMasterPlayerAccount(request, success =>
        {
            PlayFabClientAPI.ForgetAllCredentials();
            PlayerPrefs.SetString("REMEMBER_ME_ID", String.Empty);
            Debug.Log("account successfully deleted");
            settings.OnDeleteAccount();
        }, error => { Debug.Log(error.ErrorMessage); });
    }

    #endregion

    #region RESET PASSWORD

    public void SendPasswordResetEmail()
    {
        string email = resetPasswordPanel.GetEmailToReset();

        var request = new PlayFab.ClientModels.SendAccountRecoveryEmailRequest
        {
            Email = email,
            TitleId = PlayFabSettings.TitleId
        };

        PlayFabClientAPI.SendAccountRecoveryEmail(request, success =>
        {
            Debug.Log("password reset");
            resetPasswordPanel.ThrowMessage("Account recovery email sent successfully", Color.green);
            resetPasswordPanel.CloseWindow();
        }, error => { Debug.Log(error.ErrorMessage); });
    }

    #endregion

    #region GET ACCOUNT INFO

    public void GetAccountInfo()
    {
        if(!G_Ball_GameManager.instance.GetIfLoggedIn()) return;
        
        GetAccountInfoRequest request = new GetAccountInfoRequest();
        PlayFabClientAPI.GetAccountInfo(request, result =>
        {
            var username = result.AccountInfo.Username;
            var playfabID = result.AccountInfo.PlayFabId;
            var accountDate = result.AccountInfo.Created.Date.ToShortDateString();

            var userInfoWindow = FindObjectOfType<Playfab_UserInfoWindow>();
            userInfoWindow.UpdateTextValues(username, playfabID, accountDate);

            playerPlayfabID = result.AccountInfo.PlayFabId;
        }, error => { Debug.Log(error.ErrorMessage); });
    }

    #endregion

    #region UPDATE DATA

    public void SendDataPlayfab(Dictionary<string,string> dataDictionary)
    {
        if(!G_Ball_GameManager.instance.GetIfLoggedIn()) return;
        
        var request = new PlayFab.ClientModels.UpdateUserDataRequest
        {
            Data = dataDictionary
        };
        PlayFabClientAPI.UpdateUserData(request, result =>
        {
            Debug.Log("data sent ");
        }, error =>
        {
            Debug.Log(error.ErrorMessage);

        });
    }


    #endregion
    
}