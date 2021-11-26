using System;
using System.Collections.Generic;
using PlayFab;
using UnityEngine;
using Newtonsoft.Json;
using PlayFab.ClientModels;

public class G_Ball_GameManager : MonoBehaviour
{
    #region Singleton

    public static G_Ball_GameManager instance;

    [HideInInspector]
    public LevelData liveLevel;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    #endregion
    
    [SerializeField] private GameObject globalInputBlocker;
    public float loseDelay;
    public float winDelay;

    private DataVersionInDevice dataVersionInDevice;

    public bool isInputBlock = false;
    
    public GameObject debugErrorMessage;

    public StageData[] stages;

    public bool canLoadNextLevel;
    private List<LevelDataPlayfab> playfabLevelData_stage1 = new List<LevelDataPlayfab>();
    private List<LevelDataPlayfab> playfabLevelData_stage2 = new List<LevelDataPlayfab>();
    private List<LevelDataPlayfab> playfabLevelData_stage3 = new List<LevelDataPlayfab>();
    private List<LevelDataPlayfab> playfabLevelData_stage4 = new List<LevelDataPlayfab>();
    private List<LevelDataPlayfab> playfabLevelData_stage5 = new List<LevelDataPlayfab>();


    public SleepyFrogGames_Utilities utilities;
    
    private GameObject inputBlocker;
    private PlayerPrefsManager saveManager;
    PlayfabManager playfab;

    private void Start()
    {
        saveManager = PlayerPrefsManager.instance;
        HandleGameFirstLoadEver();
    }

    public void HandleGameFirstLoadEver()
    {
        if (!PlayerPrefs.HasKey("HAS_LOGIN")) // if its device first login or first time opening app since install
        {
            foreach (var stage in stages)
            {
                foreach (var level in stage.levels)
                {
                    if (level.levelIndex == 1)
                    {
                        var _newClass = new LevelDataPlayfab(level.stage, level.levelIndex, 0, 0, false, false);
                        level.SetLevelDataClass(_newClass);
                        saveManager.SaveLevelStarsEarned(level,0);
                        saveManager.SaveLevelHighScore(level,0);
                        saveManager.SaveLevelIsLocked(level,false);
                        saveManager.SaveLevelCompleted(level, false);
                    }
                    else
                    {
                        var _newClass = new LevelDataPlayfab(level.stage, level.levelIndex, 0, 0, true, false);
                        level.SetLevelDataClass(_newClass);
                        saveManager.SaveLevelStarsEarned(level,0);
                        saveManager.SaveLevelHighScore(level,0);
                        saveManager.SaveLevelIsLocked(level,true);
                        saveManager.SaveLevelCompleted(level, false);
                    }
                }
            }
            PlayerPrefs.SetString("HAS_LOGIN", "true");
        }
    }

    public enum DataVersionInDevice{needToUpdate,same,needToUpload,noInternet}
    public void GetDataVersions() // handles send and get data when app starts
    {
        if (!GetIfLoggedIn())
        {
            dataVersionInDevice = DataVersionInDevice.noInternet;
            Debug.Log("NO INTERNET");
            return;
        }
            
        var playfabID = PlayfabManager.instance.GetUserPlayFabID();
        var request = new GetUserDataRequest
        {
            PlayFabId = playfabID
        };
        PlayFabClientAPI.GetUserData(request, result =>
        {
            if (result != null)
            {
                var key = "completedLevels";
                if (result.Data.ContainsKey(key))
                {
                    var completedLevelsPlayerPrefs = 0;
                    if (PlayerPrefs.HasKey("completedLevels"))
                    {
                        completedLevelsPlayerPrefs = PlayerPrefs.GetInt("completedLevels");
                    }

                    var numString = result.Data[key].Value;
                    int num;
                    if (Int32.TryParse(numString, out num))
                    {
                        Debug.Log(num + " completed levels in playfab");
                    }
                    if (completedLevelsPlayerPrefs == num)
                    {
                        dataVersionInDevice = DataVersionInDevice.same;
                        return;
                    }

                    if (completedLevelsPlayerPrefs > num)
                    {
                        dataVersionInDevice = DataVersionInDevice.needToUpload;
                        return;
                    }

                    if (completedLevelsPlayerPrefs < num)
                    {
                        dataVersionInDevice = DataVersionInDevice.needToUpdate;
                    }
                }
                else
                {
                    dataVersionInDevice = DataVersionInDevice.needToUpload;
                }
            }
        }, error => { Debug.Log(error.ErrorMessage); });
        Debug.Log("CURRENT VERSION " + dataVersionInDevice);
        LoadData();
    }
    
    public void LoadData()
    {
        if (!GetIfHasInternet() || !GetIfLoggedIn())
        {
            foreach (var stage in stages)
            {
                foreach (var level in stage.levels)
                {
                    var stars = saveManager.GetLevelStars(level.levelID);
                    var highscore = saveManager.GetLevelHighScore(level.levelID);
                    var locked = saveManager.GetIfLevelLocked(level.levelID);
                    var completed = saveManager.GetIfLevelCompleted(level.levelID);
                        
                    var _class = new LevelDataPlayfab(level.stage, level.levelIndex, stars, highscore, locked, completed);
                    level.SetLevelDataClass(_class);
                }
            }
            return;
        }
        
        switch (dataVersionInDevice)
        {
            case DataVersionInDevice.needToUpload:  // device data is newer
                
                SendUpdatedDataToPlayfab();
                
                break; 
            
            case DataVersionInDevice.same:  // device data is the same


                break;
            case DataVersionInDevice.needToUpdate:  // device data is older
                
                GetUpdatedDataFromPlayfab();
                break;
            
            case DataVersionInDevice.noInternet:

                
                break;
        }
        
    }
    private void GetUpdatedDataFromPlayfab()
    {
        LevelData[] stage1_Levels = null;
        LevelData[] stage2_Levels = null;
        LevelData[] stage3_Levels = null;
        LevelData[] stage4_Levels = null;
        LevelData[] stage5_Levels = null;

        foreach (var stage in stages)
        {
            switch (stage.stageIndex)
            {
                case 1:
                    stage1_Levels = stage.levels;
                    break;
                case 2:
                    stage2_Levels = stage.levels;
                    break;
                case 3:
                    stage3_Levels = stage.levels;
                    break;
                case 4:
                    stage4_Levels = stage.levels;
                    break;
                case 5:
                    stage5_Levels = stage.levels;
                    break;
            }
        }
        
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result =>
        {
            if (result.Data != null)
            {
                if (!result.Data.ContainsKey("Stage_1") || !result.Data.ContainsKey("Stage_2") ||
                    !result.Data.ContainsKey("Stage_3") || !result.Data.ContainsKey("Stage_4") ||
                    !result.Data.ContainsKey("Stage_5"))
                {
                    return;
                }
                
                if (result.Data.ContainsKey("completedLevels"))
                {
                    var receivedCompletedLevelsString = result.Data["completedLevels"].Value;
                    int num;
                    if(Int32.TryParse(receivedCompletedLevelsString, out num))
                    {
                        PlayerPrefs.SetInt("completedLevels", num);
                    }
                }

                if (result.Data["Stage_1"] != null)
                {
                    List<LevelDataPlayfab> stage1_Data = JsonConvert.DeserializeObject<List<LevelDataPlayfab>>(result.Data["Stage_1"].Value);
                    
                    foreach (var level in stage1_Data)
                    {
                        var _newClass = new LevelDataPlayfab(1,level.levelIndex, level.stars, level.highscore, level.locked, level.completed);
                        foreach (var deviceLevel in stage1_Levels)
                        {
                            if (level.levelIndex == deviceLevel.levelIndex)
                            {
                                deviceLevel.SetLevelDataClass(_newClass);
                            }
                        }
                    }
                }

                if (result.Data["Stage_2"] != null)
                {
                    List<LevelDataPlayfab> stage2_Data = JsonConvert.DeserializeObject<List<LevelDataPlayfab>>(result.Data["Stage_2"].Value);
                    foreach (var level in stage2_Data)
                    {
                        var _newClass = new LevelDataPlayfab(2,level.levelIndex, level.stars, level.highscore, level.locked, level.completed);
                        foreach (var deviceLevel in stage2_Levels)
                        {
                            if (level.levelIndex == deviceLevel.levelIndex)
                            {
                                deviceLevel.SetLevelDataClass(_newClass);
                            }
                        }
                    }
                }

                if (result.Data["Stage_3"] != null)
                {
                    List<LevelDataPlayfab> stage3_Data = JsonConvert.DeserializeObject<List<LevelDataPlayfab>>(result.Data["Stage_3"].Value);
                    foreach (var level in stage3_Data)
                    {
                        var _newClass = new LevelDataPlayfab(3,level.levelIndex, level.stars, level.highscore, level.locked, level.completed);
                        foreach (var deviceLevel in stage3_Levels)
                        {
                            if (level.levelIndex == deviceLevel.levelIndex)
                            {
                                deviceLevel.SetLevelDataClass(_newClass);
                            }
                        }
                    }
                }
                
                if (result.Data["Stage_4"] != null)
                {
                    List<LevelDataPlayfab> stage4_Data = JsonConvert.DeserializeObject<List<LevelDataPlayfab>>(result.Data["Stage_4"].Value);
                    foreach (var level in stage4_Data)
                    {
                        var _newClass = new LevelDataPlayfab(4,level.levelIndex, level.stars, level.highscore, level.locked, level.completed);
                        foreach (var deviceLevel in stage4_Levels)
                        {
                            if (level.levelIndex == deviceLevel.levelIndex)
                            {
                                deviceLevel.SetLevelDataClass(_newClass);
                            }
                        }
                    }
                }
                
                if (result.Data["Stage_5"] != null)
                {
                    List<LevelDataPlayfab> stage5_Data = JsonConvert.DeserializeObject<List<LevelDataPlayfab>>(result.Data["Stage_5"].Value);
                    foreach (var level in stage5_Data)
                    {
                        var _newClass = new LevelDataPlayfab(5,level.levelIndex, level.stars, level.highscore, level.locked, level.completed);
                        foreach (var deviceLevel in stage5_Levels)
                        {
                            if (level.levelIndex == deviceLevel.levelIndex)
                            {
                                deviceLevel.SetLevelDataClass(_newClass);
                            }
                        }
                    }
                }
                
            }
        }, error =>
        {
            Debug.Log(error.ErrorMessage);
        });
        
    }

    private void SendUpdatedDataToPlayfab()
    {
        playfabLevelData_stage1.Clear();
        playfabLevelData_stage2.Clear();
        playfabLevelData_stage3.Clear();
        playfabLevelData_stage4.Clear();
        playfabLevelData_stage5.Clear();
        
        foreach (var stage in stages)
        {
            if (stage.stageIndex == 1)
            {
                for (int i = 0; i < stage.levels.Length; i++)
                {
                    playfabLevelData_stage1.Add(stage.levels[i].levelDataClass);
                }
            }
            if (stage.stageIndex == 2)
            {
                for (int i = 0; i < stage.levels.Length; i++)
                {
                    playfabLevelData_stage2.Add(stage.levels[i].levelDataClass);
                }
            }
            if (stage.stageIndex == 3)
            {
                for (int i = 0; i < stage.levels.Length; i++)
                {
                    playfabLevelData_stage3.Add(stage.levels[i].levelDataClass);
                }
            }
            if (stage.stageIndex == 4)
            {
                for (int i = 0; i < stage.levels.Length; i++)
                {
                    playfabLevelData_stage4.Add(stage.levels[i].levelDataClass);
                }
            }
            if (stage.stageIndex == 5)
            {
                for (int i = 0; i < stage.levels.Length; i++)
                {
                    playfabLevelData_stage5.Add(stage.levels[i].levelDataClass);
                }
            }
        }

        var json_stage1 = JsonConvert.SerializeObject(playfabLevelData_stage1);
        var json_stage2 = JsonConvert.SerializeObject(playfabLevelData_stage2);
        var json_stage3 = JsonConvert.SerializeObject(playfabLevelData_stage3);
        var json_stage4 = JsonConvert.SerializeObject(playfabLevelData_stage4);
        var json_stage5 = JsonConvert.SerializeObject(playfabLevelData_stage5);
        
        List<string> jsonsToSend = new List<string>
        {
            json_stage1,
            json_stage2,
            json_stage3,
            json_stage4,
            json_stage5
        };
        
        int stageIndexJson = 1;
        
        foreach (var json in jsonsToSend)
        {
            Dictionary<string, string> dataDict = new Dictionary<string, string>();
            
            var key = "Stage_" + stageIndexJson ;
            var value = json;
            dataDict.Add(key, value);
            var request = new UpdateUserDataRequest
            {
                Data = dataDict
            };
            PlayFabClientAPI.UpdateUserData(request, success => { Debug.Log("SUCCESS"); },
                error => { Debug.Log(error.ErrorMessage); });
            stageIndexJson += 1;
        }
        
        var completedLevels = "completedLevels";
        var completedLevelsValue = PlayerPrefs.GetInt("completedLevels").ToString();
        var dict = new Dictionary<string,string> {{completedLevels, completedLevelsValue}};
        PlayfabManager.instance.SendDataPlayfab(dict);
        

    }
    public void SetLiveLevel(LevelData level)
    {
        liveLevel = level;
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            PlayerPrefs.Save();
            if (GetIfLoggedIn())
            {
                Debug.LogWarning(dataVersionInDevice + " DATA VERSION");
                LoadData();
            }
        }
    }

    public void SetAllKinematic(bool a)
    {
        var rbs = FindObjectsOfType<Rigidbody2D>();
        foreach (var rb in rbs)
        {
            rb.isKinematic = a;
        }
    }
    public void OnLevelLostBehaviour()
    {
        var rbs = FindObjectsOfType<Rigidbody2D>();
        foreach (var rb in rbs)
        {
            if (rb != null)
            {
                if (rb.GetComponent<G_Ball_Block>())
                {
                    Destroy(rb.GetComponent<G_Ball_Block>());
                }

                if (rb.GetComponent<CompositeCollider2D>())
                {
                    Destroy(rb.GetComponent<CompositeCollider2D>());
                }

                if (rb.GetComponent<BoxCollider2D>())
                {
                    Destroy(rb.GetComponent<BoxCollider2D>());
                }

                if (rb.GetComponent<CircleCollider2D>())
                {
                    Destroy(rb.GetComponent<CircleCollider2D>());
                }

                Destroy(rb);
            }
        }

        canLoadNextLevel = false;
    }
    public void EnableGlobalInputBlocker(bool activate)
    {
        if (activate)
        {
            isInputBlock = true;
            inputBlocker = Instantiate(globalInputBlocker);
            inputBlocker.transform.position = Vector3.zero;
        }
        else
        {
            isInputBlock = false;
            if (inputBlocker != null)
            {
                Destroy(inputBlocker);
            }
        }
    }
    public void LoadNextLevel()
    {
        if (!canLoadNextLevel) return;

        canLoadNextLevel = false;

        foreach (var stage in stages)
        {
            if (stage.stageIndex == liveLevel.stage)
            {
                foreach (var level in stage.levels)
                {
                    if (level.levelIndex == liveLevel.levelIndex + 1)
                    {
                        level.LoadLevel();
                        return;
                    }
                }
            }
        }

    }
    
    public bool GetIfLoggedIn()
    {
        return PlayFabAuthenticationAPI.IsEntityLoggedIn();
    }
    public bool GetIfHasInternet()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("no internet");
            return false;
        }

        Debug.Log("yes internet");
        return true;
    }
    
}