using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ToggleType
{
    MUSIC,
    SOUNDFX,
    NOTIFICATIONS,
    VIBRATION
}
[RequireComponent(typeof(Button))]
public class G_Ball_SettingsToggle : MonoBehaviour
{
    [SerializeField] ToggleType toggleType;
    [SerializeField] private GameObject onToggle;
    [SerializeField] private GameObject offToggle;
    [SerializeField] private bool isOnButton;
    private Button button;
    private G_Ball_SettingsManager settings;
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        settings = G_Ball_SettingsManager.instance;
    }

    private void OnClick()
    {
        switch (toggleType)
        {
            case ToggleType.MUSIC:
                HandleMusicToggle();
                break;
            case ToggleType.SOUNDFX:
                HandleSoundFXToggle();
                break;
            case ToggleType.NOTIFICATIONS:
                HandleNotificationsToggle();
                break;
            case ToggleType.VIBRATION:
                HandleVibrationToggle();
                break;
        }
    }
    void SwitchToggle()
    {
        if (isOnButton)
        {
            offToggle.SetActive(true);
            onToggle.SetActive(false);
            return;
        }
        offToggle.SetActive(false);
        onToggle.SetActive(true);
    }

    void HandleMusicToggle()
    {
        if (isOnButton)
        {
            settings.SetUnsetMusicMuted(false);
            SwitchToggle();
            return;
        }
        settings.SetUnsetMusicMuted(true);
        SwitchToggle();

    }

    void HandleSoundFXToggle()
    {
        if (isOnButton)
        {
            settings.SetUnsetSoundFxMuted(false);
            SwitchToggle();
            return;
        }
        SwitchToggle();
        settings.SetUnsetSoundFxMuted(true);
    }

    void HandleNotificationsToggle()
    {
        if (isOnButton)
        {
            //notifications off
            settings.SetUnsetNotifications(false);
            SwitchToggle();
            return;
        }
        SwitchToggle();
        settings.SetUnsetNotifications(true);

    }

    void HandleVibrationToggle()
    {
        if (isOnButton)
        {
            settings.SetUnsetVibration(false);
            SwitchToggle();
            return;
        }
        SwitchToggle();
        settings.SetUnsetVibration(true);
    }

    
}
