using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Audio Data", menuName = "Ball Game/New Audio Data")]

public class AudioData : ScriptableObject
{
    [BoxGroup("GAME CORE", CenterLabel = true)]
    public AudioClip gameTheme;
    
    [Space(10f)]

    [BoxGroup("USER INTERFACE", CenterLabel = true)]
    public AudioClip onPopupOpen;
    public AudioClip onPanelOpen;
    public AudioClip buttonClickSound;
    public AudioClip toggleClickSound;
    
    [Space(10f)]
    
    [BoxGroup("GAMEPLAY", CenterLabel = true)]
    public AudioClip birdToLauncherSound;
    public AudioClip levelWonSound;
    public AudioClip levelLostSound;
    public AudioClip levelThreeStarSound;
    public AudioClip levelTwoStarSound;
    public AudioClip levelOneStarSound;
   
    [Space(10f)]
    
    [BoxGroup("ENEMIES", CenterLabel = true)]
    public AudioClip enemyDeathSound;
    public AudioClip enemyJumpSound;
    public AudioClip enemyGruntSound;
    public AudioClip enemyHurtSound;
    
    [Space(10f)]

    [BoxGroup("AUTHENTICATION SERVICES", CenterLabel = true)]
    public AudioClip onLoginSound;
    public AudioClip onRegisterSound;
}
