using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LevelData : ScriptableObject
{
    public AudioClip LevelSoundtrack;
    public float SoundtrackStartTime;
    [Space]
    public string LevelName = "EditorLevel";
    public string AuthorName = "Unknown";
    [Space]
    public int MaxDiamondCount = 10;
    public int CrownCount = 3;
}
