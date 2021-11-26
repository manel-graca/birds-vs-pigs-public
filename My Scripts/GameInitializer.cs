using System;
using System.Collections;
using System.Collections.Generic;
using Lovatto.SceneLoader;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.UI;
using Random = UnityEngine.Random;
public class GameInitializer : MonoBehaviour
{
    [SerializeField] private G_Ball_GameManager manager;
    [SerializeField] private Image loadingBar;
    [SerializeField] bl_UIGradient nameGradient;
    [SerializeField] private TextMeshProUGUI loadingInfoText;
    
    [SerializeField] private string[] randomLoadingMessages;
    
    PlayfabManager playfab;
    private void Start()
    {
        playfab = PlayfabManager.instance;
        
        StartCoroutine(InitializationRoutine());
        loadingInfoText.text = "Building the levels...";
    }

    void Login()
    {
        var androidDeviceID = SystemInfo.deviceUniqueIdentifier;
        var request = new LoginWithAndroidDeviceIDRequest
        {
            AndroidDeviceId = androidDeviceID,
            CreateAccount = true,
            TitleId = PlayFabSettings.TitleId
        };
        PlayFabClientAPI.LoginWithAndroidDeviceID(request, result =>
        {
            if (result.NewlyCreated)
            {
                var dict = new Dictionary<string,string>();
                var key = "accountCreated";
                var value = "True";
                
                dict.Add(key,value);
                
                var request_ = new UpdateUserDataRequest
                {
                    Data = dict
                };
                PlayFabClientAPI.UpdateUserData(request_,null,null);
                Debug.Log("account fresh created");
            }
            
            playfab.SetPlayFabID(result.PlayFabId);
            Debug.Log("LOGIN: "  + result.PlayFabId);

            
            Init();
            
        }, error =>
        {
            Debug.Log(error.ErrorMessage);
        });
    }

    void Init()
    {
        manager.GetDataVersions();
    }

    private string GetRandomPhrase()
    {
        var random = Random.Range(0,randomLoadingMessages.Length);
        var phrase = randomLoadingMessages[random];
        
        if(phrase == null) return "Almost done...";
        
        return phrase;
    }

    IEnumerator InitializationRoutine()
    {
        yield return null;
        
        bool showOnce = false;
        bool showTwice = false;
        
        loadingBar.fillAmount = 0f;
        nameGradient.Offset = 0f;
        
        float progress = 0f;
        float time = Time.deltaTime;
        float completion = 1f;

        if (manager.GetIfHasInternet())
        {
            Login();
        }
        
        while (progress < completion)
        {
            progress += time;
            loadingBar.fillAmount = progress;
            nameGradient.Offset = progress;

            if (!showOnce && progress > 0.25f && progress < 0.75f)
            {
                showOnce = true;
                loadingInfoText.text = GetRandomPhrase();
            }
            if (!showTwice && progress > 0.75f)
            {
                showTwice = true;
                loadingInfoText.text = GetRandomPhrase();
            }
            yield return null;
        }
        yield return new WaitForSeconds(0.25f);
        var menuScene = SceneManager.LoadSceneAsync(1);
        menuScene.allowSceneActivation = false;
        yield return new WaitForSeconds(1f);
        menuScene.allowSceneActivation = true;

    }
    
}
