using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Scenes Organizer", menuName = "Ball Game/New Scene Organizer")]

public class ScenesOrganizer : ScriptableObject
{
    [BoxGroup("STAGE 1", CenterLabel = true)]
    public string level1Name;
    public string level2Name;
    // continue here.......
}
