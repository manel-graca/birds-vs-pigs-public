[System.Serializable]
public class LevelDataPlayfab
{
    public int stage;
    public int levelIndex;
    public int stars;
    public int highscore;
    public bool locked;
    public bool completed;

    public LevelDataPlayfab(int stage, int levelIndex, int stars, int highscore, bool locked, bool completed)
    {
        this.stage = stage;
        this.levelIndex = levelIndex;
        this.stars = stars;
        this.highscore = highscore;
        this.locked = locked;
        this.completed = completed;
    }
}
