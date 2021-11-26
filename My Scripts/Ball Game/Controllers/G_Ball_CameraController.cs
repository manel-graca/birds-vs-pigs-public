using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class G_Ball_CameraController : MonoBehaviour
{
    public static G_Ball_CameraController instance;
    private void Awake()
    {
        instance = this;
    }

    [SerializeField] GameObject[] zoomableCameras;
    
    [SerializeField] Image[] zoomButtonsImages;
    [SerializeField] GameObject[] zoomButtonsObjs;
    
    [SerializeField] GameObject toggleCamButton;

    bool a;
    
    private readonly Vector2 targetCam1xButtonPos = new Vector2(-78.6f, -12.1f);
    private readonly Vector2 targetCam2xButtonPos = new Vector2( -57.5f, 14.2f);
    private readonly Vector2 targetCam3xButtonPos = new Vector2( -22.9f, 10.4f);

    private float timeZooming = 0.35f;
    private bool isChangingCam;
    
    private float timeBetweenToggleClick = 0.5f;
    private float timeSinceToggleClick = Mathf.Infinity;
    private bool canToggleClick = true;

    private void Update()
    {
        if (timeSinceToggleClick > timeBetweenToggleClick)
        {
            canToggleClick = true;
        }
        timeSinceToggleClick += Time.deltaTime;
    }

    public void ShowHideCamButtons()
    {
        if(!canToggleClick) return;
        timeSinceToggleClick = 0f;
        canToggleClick = false;
        
        a = !a;
        if (a)
        {
            toggleCamButton.transform.LeanRotateZ(180f, 0.1f).setEaseInSine();
            LeanCamButtonsShowing();
            return;
        }
        toggleCamButton.transform.LeanRotateZ(0f, 0.1f).setEaseInSine();
        LeanCamButtonsHiding();
    }

    private void LeanCamButtonsShowing()
    {
        zoomButtonsObjs[0].transform.localScale = Vector3.zero;
        zoomButtonsObjs[1].transform.localScale = Vector3.zero;
        zoomButtonsObjs[2].transform.localScale = Vector3.zero;
        
        zoomButtonsObjs[0].transform.LeanScale(Vector3.one, 0.1f).setEaseOutElastic().delay = 0.05f;
        zoomButtonsObjs[1].transform.LeanScale(Vector3.one, 0.1f).setEaseOutElastic().delay = 0.1f;
        zoomButtonsObjs[2].transform.LeanScale(Vector3.one, 0.1f).setEaseOutElastic().delay = 0.15f;
    }
    
    private void LeanCamButtonsHiding()
    {
        zoomButtonsObjs[2].transform.LeanScale(Vector3.zero, 0.1f).setEaseInElastic().delay = 0.05f;
        zoomButtonsObjs[1].transform.LeanScale(Vector3.zero, 0.1f).setEaseInElastic().delay = 0.1f;
        zoomButtonsObjs[0].transform.LeanScale(Vector3.zero, 0.1f).setEaseInElastic().delay = 0.15f;
    }

    
    public void Change1xCamera()
    {
        if(isChangingCam) return;
        StartCoroutine(ChangeCameraRoutine());
        
        zoomableCameras[0].SetActive(true);
        zoomableCameras[1].SetActive(false);
        zoomableCameras[2].SetActive(false);
        
        zoomButtonsImages[0].color = Color.white;
        zoomButtonsImages[1].color = new Color32(255,255,255,210);
        zoomButtonsImages[2].color = new Color32(255,255,255,210);
        Debug.Log("1x cam");

    }
    public void Change2xCamera()
    {
        if(isChangingCam) return;
        StartCoroutine(ChangeCameraRoutine());
        
        zoomableCameras[1].SetActive(true);
        zoomableCameras[0].SetActive(false);
        zoomableCameras[2].SetActive(false);
        
        zoomButtonsImages[1].color = Color.white;
        zoomButtonsImages[0].color = new Color32(255,255,255,210);
        zoomButtonsImages[2].color = new Color32(255,255,255,210);
        Debug.Log("2x cam");

    }
    public void Change3xCamera()
    {
        if(isChangingCam) return;
        StartCoroutine(ChangeCameraRoutine());
        
        zoomableCameras[2].SetActive(true);
        zoomableCameras[0].SetActive(false);
        zoomableCameras[1].SetActive(false);
        
        zoomButtonsImages[2].color = Color.white;
        zoomButtonsImages[0].color = new Color32(255,255,255,210);
        zoomButtonsImages[1].color = new Color32(255,255,255,210);
        Debug.Log("3x cam");

    }

    IEnumerator ChangeCameraRoutine()
    {
        float t = 0f;
        isChangingCam = true;
        while (t < timeZooming)
        {
            t += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(timeZooming);
        isChangingCam = false;
    }
}
