using UnityEngine;
using UnityEngine.UI;

public class G_Ball_UI_WinPanel : MonoBehaviour
{
    [SerializeField] Button nextLevelBtt;
    [SerializeField] Button doubleScoreAdBtt;

    private void Start()
    {
        if (nextLevelBtt != null)
        {
            nextLevelBtt.onClick.AddListener(LoadNextLevel);
        }
        if (doubleScoreAdBtt != null)
        {
            doubleScoreAdBtt.onClick.AddListener(DoubleScoreWithAd);
        }
    }

    void LoadNextLevel()
    {
        G_Ball_GameManager.instance.LoadNextLevel();
    }

    void DoubleScoreWithAd()
    {
        var levelM = G_Ball_LevelManager.instance;
        
        var score = levelM.GetCurrentScore();
        levelM.AddToScore(score);
        var doubledScore = levelM.GetCurrentScore();
        G_Ball_UIController.instance.UpdateWinPanelScoreboard(doubledScore);
    }
}
