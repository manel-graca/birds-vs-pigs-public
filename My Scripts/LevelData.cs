using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
[CreateAssetMenu(fileName = "New Level", menuName = "Ball Game/New Level")]
public class LevelData : ScriptableObject
{
    [InfoBox("Level name MUST follow this format: \n ' S + stageIndex + _ + L + levelIndex \n example: S3_L13")]
    public string levelID;
    public int stage;
    public int levelIndex;
    public string sceneName;
    
    [InfoBox("MAX 8 BALLS")]
    public GameObject[] spawningBallsPrefabs;
    
    [Range(1,8)] public int startingBalls;

    public LevelDataPlayfab levelDataClass;
    
    public void LoadLevel()
    {
        G_Ball_GameManager.instance.SetLiveLevel(this);
        SceneManager.LoadScene(sceneName);
    }

    public void SetLevelDataClass(LevelDataPlayfab classObject)
    {
        levelDataClass = classObject;
    }

    public LevelDataPlayfab GetLevelDataClass()
    {
        return levelDataClass;
    }

}
