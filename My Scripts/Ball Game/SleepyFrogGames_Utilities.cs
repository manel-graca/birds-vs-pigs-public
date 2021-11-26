using UnityEngine;

[CreateAssetMenu(fileName = "UI Utilities", menuName = "Cotevelino Games/New UI Utility SO")]
public class SleepyFrogGames_Utilities : ScriptableObject
{
    public void InstantShowHideCanvasGroup(bool activate, CanvasGroup cg)
    {
        if (activate)
        {
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;
            return;
        }
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }
    public void LeanInOutCanvasGroup(bool activate, CanvasGroup cg, float duration)
    {
        float targetAlpha;
        
        if (activate)
        {
            targetAlpha = 1f;
            cg.LeanAlpha(targetAlpha,duration);
            cg.interactable = true;
            cg.blocksRaycasts = true;
            return;
        } 
        targetAlpha = 0f;
        cg.LeanAlpha(targetAlpha,duration);
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }
    public void LeanInOutCanvasGroupWithDelay(bool activate, CanvasGroup cg, float duration, float delayTime)
    {
        float targetAlpha;
        
        if (activate)
        {
            targetAlpha = 1f;
            cg.LeanAlpha(targetAlpha,duration).delay = delayTime;
            cg.interactable = true;
            cg.blocksRaycasts = true;
            return;
        } 
        targetAlpha = 0f;
        cg.LeanAlpha(targetAlpha,duration).delay = delayTime;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    public void InstantCanvasGroupAlpha(bool activate, float alpha, CanvasGroup cg)
    {
        if (activate)
        {
            cg.alpha = alpha;
            cg.interactable = false;
            cg.blocksRaycasts = false;
            return;
        }
        cg.alpha = alpha;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }
    public void LeanCanvasGroupAlpha(bool activate,float alpha, CanvasGroup cg, float duration)
    {
        if (activate)
        {
            cg.LeanAlpha(alpha,duration);
            cg.interactable = true;
            cg.blocksRaycasts = true;
            return;
        }
        cg.LeanAlpha(alpha,duration);
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    public Vector3 ConvertScreenPointToWorld(Vector3 screenPoint, Camera mainCamera)
    {
        return mainCamera.ScreenToWorldPoint(screenPoint);
    }
    
}