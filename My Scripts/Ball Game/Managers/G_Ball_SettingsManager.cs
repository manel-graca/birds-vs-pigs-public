using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
public class G_Ball_SettingsManager : MonoBehaviour
{
    public static G_Ball_SettingsManager instance;
    
    [BoxGroup("Panels", CenterLabel = true)]
    public CanvasGroup loginPanel;
    [BoxGroup("Panels")]
    public CanvasGroup registerPanel;
    
    [Space(10f)]
    
    [BoxGroup("Playfab UI Elements", CenterLabel = true)]
    public GameObject registerAccountButton;
    [BoxGroup("Playfab UI Elements")]
    public GameObject loginAccountButton;
    [BoxGroup("Playfab UI Elements")]
    public GameObject deleteAccountButton;
    [BoxGroup("Playfab UI Elements")]
    public GameObject logoutAccountButton;
    
    [Space(10f)]
    
    [BoxGroup("Toggles", CenterLabel = true)]
    public GameObject musicToggleOn;
    [BoxGroup("Toggles")]
    public GameObject musicToggleOff;
    [BoxGroup("Toggles")]
    public GameObject soundFxToggleOn;
    [BoxGroup("Toggles")]
    public GameObject soundFxToggleOff;
    [BoxGroup("Toggles")]
    public GameObject notificationsToggleOn;
    [BoxGroup("Toggles")]
    public GameObject notificationsToggleOff;
    [BoxGroup("Toggles")]
    public GameObject vibrationToggleOn;
    [BoxGroup("Toggles")]
    public GameObject vibrationToggleOff;
    
    [BoxGroup("Booleans", CenterLabel = true)]
    public bool isGlobalMuted;
    [BoxGroup("Booleans")]
    public bool isMusicOn;
    [BoxGroup("Booleans")]
    public bool isSoundEffectsOn;
    [BoxGroup("Booleans")]
    public bool isNotificationsOn;
    [BoxGroup("Booleans")]
    public bool isVibrationOn;
    
    public SleepyFrogGames_Utilities utilities;
    SoundManager soundManager;
    G_Ball_UIController ui;
    
    AudioClip toggleSound;

    private void Awake()
    {
        if (instance == null)
        {
            instance =  this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        soundManager = SoundManager.instance;
        ui = G_Ball_UIController.instance;
        ReadSettingsPlayerPrefs();
        toggleSound = soundManager.audioData.toggleClickSound;
    }
    
    public void ShowLoginPanel(bool activate)
    {
        utilities.LeanInOutCanvasGroup(activate,loginPanel,0.25f);
    }
    public void ShowRegisterPanel(bool activate)
    {
        utilities.LeanInOutCanvasGroup(activate,registerPanel,0.25f);
    }
    
    void ReadSettingsPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey("MUSIC_ON"))
        {
            isMusicOn = true;
            soundManager.MuteMusicAudioMixer(false);
            PlayerPrefs.SetString("MUSIC_ON", "True");
        }
        if (!PlayerPrefs.HasKey("SOUND_EFFECTS_ON"))
        {
            isSoundEffectsOn = true;
            soundManager.MuteEffectsAudioMixer(false);
            PlayerPrefs.SetString("SOUND_EFFECTS_ON", "True");
        }
        
        string music = PlayerPrefs.GetString("MUSIC_ON");
        string soundFX = PlayerPrefs.GetString("SOUND_EFFECTS_ON");
        string notification = PlayerPrefs.GetString("NOTIFICATIONS_ON");
        string vibration = PlayerPrefs.GetString("VIBRATION_ON");
        if (music == "True")
        {
            isMusicOn = true;
            soundManager.MuteMusicAudioMixer(false);
        }
        else
        {
            isMusicOn = false;
            soundManager.MuteMusicAudioMixer(true);
        }

        if (soundFX == "True")
        {
            isSoundEffectsOn = true;
            soundManager.MuteEffectsAudioMixer(false);
        }
        else
        {
            isSoundEffectsOn = false;
            soundManager.MuteEffectsAudioMixer(true);
        }
        if (notification == "True")
        {
            isNotificationsOn = true;
        }
        else
        {
            isNotificationsOn = false;
        }

        if (vibration == "True")
        {
            isVibrationOn = true;
        }
        else
        {
            isVibrationOn = false;
        }
    }
    public void CloseLoginPanelDelay(float delay)
    {
        utilities.LeanInOutCanvasGroupWithDelay(false, loginPanel, 0.15f,delay);
    }

    public void CloseRegisterPanelDelay(float delay)
    {
        utilities.LeanInOutCanvasGroupWithDelay(false,registerPanel,0.15f,delay);
    }
    public void HandleOnlineButtonsIfNoInternet()
    {
        var manager = G_Ball_GameManager.instance;
        
        if (!manager.GetIfHasInternet())
        {
            registerAccountButton.SetActive(false);
            loginAccountButton.SetActive(false);
            logoutAccountButton.SetActive(false);
            deleteAccountButton.SetActive(false);
        }
    }
    public void OnFirstAnonymousLoginAccount()
    {
        registerAccountButton.SetActive(true);
        loginAccountButton.SetActive(true);
        deleteAccountButton.SetActive(false);
        logoutAccountButton.SetActive(false);
        Debug.Log("on 1st login");
    }
    public void OnLoginAccount()
    {
        registerAccountButton.SetActive(false);
        loginAccountButton.SetActive(false);
        deleteAccountButton.SetActive(true);
        logoutAccountButton.SetActive(true);
        Debug.Log("on login");

    }
    public void OnRegisterAccount()
    {
        registerAccountButton.SetActive(false);
        loginAccountButton.SetActive(false);
        deleteAccountButton.SetActive(true);
        logoutAccountButton.SetActive(true);
        Debug.Log("on regist");

    }
    public void OnLogoutAccount()
    {
        registerAccountButton.SetActive(true);
        loginAccountButton.SetActive(true);
        deleteAccountButton.SetActive(false);
        logoutAccountButton.SetActive(false);
    }
    public void OnDeleteAccount()
    {
        registerAccountButton.SetActive(true);
        loginAccountButton.SetActive(true);
        deleteAccountButton.SetActive(false);
        logoutAccountButton.SetActive(false);
    }
    

    public void OnSettingsOpened()
    {
        HandleOnlineButtonsIfNoInternet();
        string remember = PlayerPrefs.GetString("rememberMeID");
        if (!string.IsNullOrEmpty(remember))
        {
            OnLoginAccount();
        }

        string music = PlayerPrefs.GetString("MUSIC_ON");
        string soundFX = PlayerPrefs.GetString("SOUND_EFFECTS_ON");
        string notification = PlayerPrefs.GetString("NOTIFICATIONS_ON");
        string vibration = PlayerPrefs.GetString("VIBRATION_ON");
        
        if (music == "True")
        {
            musicToggleOn.SetActive(true);
            musicToggleOff.SetActive(false);
        }
        else
        {
            musicToggleOff.SetActive(true);
            musicToggleOn.SetActive(false);
        }

        if (soundFX == "True")
        {
            soundFxToggleOn.SetActive(true);
            soundFxToggleOff.SetActive(false);
        }
        else
        {
            soundFxToggleOff.SetActive(true);
            soundFxToggleOn.SetActive(false);
        }
        if (notification == "True")
        {
            notificationsToggleOn.SetActive(true);
            notificationsToggleOff.SetActive(false);
        }
        else
        {
            notificationsToggleOff.SetActive(true);
            notificationsToggleOn.SetActive(false);
        }

        if (vibration == "True")
        {
            vibrationToggleOn.SetActive(true);
            vibrationToggleOff.SetActive(false);
        }
        else
        {
            vibrationToggleOff.SetActive(true);
            vibrationToggleOn.SetActive(false);
        }
    }
    public void SetUnsetMusicMuted(bool on)
    {
        isMusicOn = !isMusicOn;
        soundManager.PlayEffectsSound(toggleSound);

        if (on) // NOT MUTED
        {
            soundManager.MuteMusicAudioMixer(false);
            PlayerPrefs.SetString("MUSIC_ON", "True");
            return;
        }
        // MUTED
        soundManager.MuteMusicAudioMixer(true);
        PlayerPrefs.SetString("MUSIC_ON", "False");
    }
    public void SetUnsetSoundFxMuted(bool on)
    {
        isSoundEffectsOn = !isSoundEffectsOn;
        soundManager.PlayEffectsSound(toggleSound);

        if (on)
        {
            soundManager.MuteEffectsAudioMixer(false);
            
            PlayerPrefs.SetString("SOUND_EFFECTS_ON", "True");
            return;
        }
        soundManager.MuteEffectsAudioMixer(true);
        PlayerPrefs.SetString("SOUND_EFFECTS_ON", "False");
    }
    public void SetUnsetNotifications(bool activated)
    {
        isNotificationsOn = !isNotificationsOn;
        soundManager.PlayEffectsSound(toggleSound);
        if (activated)
        {
            PlayerPrefs.SetString("NOTIFICATIONS_ON", "True");
            return;
        }
        PlayerPrefs.SetString("NOTIFICATIONS_ON", "False");

    }
    public void SetUnsetVibration(bool activated)
    {
        isVibrationOn = !isVibrationOn;
        soundManager.PlayEffectsSound(toggleSound);
        if (activated)
        {
            PlayerPrefs.SetString("VIBRATION_ON", "True");
            return;
        }
        PlayerPrefs.SetString("VIBRATION_ON", "False");
    }
    public void SetIsMuted()
    {
        isGlobalMuted = !isGlobalMuted;
        ui.HandleMuteButton();
        soundManager.MuteEffectsAudioMixer(true);
        soundManager.MuteMusicAudioMixer(true);
    }

    public void SetTimeScale(float f)
    {
        Time.timeScale = f;
    }
    public bool GetIfMusicOn()
    {
        return isMusicOn;
    }
    public bool GetIfSoundFxOn()
    {
        return isSoundEffectsOn;
    }
    public bool GetIfNotificationOn()
    {
        return isNotificationsOn;
    }
    public bool GetIfVibrateOn()
    {
        return isVibrationOn;
    }
}
