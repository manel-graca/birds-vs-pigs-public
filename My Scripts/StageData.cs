using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Stage", menuName = "Ball Game/New Stage")]
public class StageData : ScriptableObject
{
    public int stageIndex;
    public LevelData[] levels;
}
