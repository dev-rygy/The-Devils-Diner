using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct NightPreset
{
    public string name;
    public int numberOfDemon;
    public string endNightText;

    [Header("Spawnable Demon")]
    public DemonSpawnSetting[] demonSpawnSettings;

    [Header("Spawnable Buff")]
    [ValueLabel("Burst Size")]
    public ToggleWithValueInt speedBuff;

    [Header("Spawnable Hazard")]
    [ValueLabel("Burst Size")]
    public ToggleWithValueInt slowHazard;
}

[System.Serializable]
public struct ToggleWithValueInt
{
    public bool toggle;
    public int value;
}

[System.Serializable]
public struct ToggleWithValueFloat
{
    public bool toggle;
    public float value;
}

[System.Serializable]
public struct DemonSpawnSetting
{
    public DemonType demonType;
    public DemonSpawnType demonSpawnType;
    public float spawnChance;
    public int spawnEveryXDemon;
    public int maxPerQueue;
    public int maxPerMap;
    public int maxPerNight;
}

public enum DemonSpawnType
{
    SpawnChance,
    EveryXDemon,   
    EndOfNight,
}

[ExecuteAlways]
[CreateAssetMenu(fileName = "Difficulty", menuName = "Scriptable Objects/Difficulty")]
public class Difficulty : ScriptableObject
{
    [SerializeField]
    private string difficultyName;

    public NightPreset[] nightPresets;

    private void OnValidate()
    {
        for(int i = 0; i < nightPresets.Length; i++)
        {
            if (nightPresets[i].name == "")
                nightPresets[i].name = "Night " + i;
        
            if (nightPresets[i].endNightText == "")
                nightPresets[i].endNightText = "Night Survived";
        }
    }
}
