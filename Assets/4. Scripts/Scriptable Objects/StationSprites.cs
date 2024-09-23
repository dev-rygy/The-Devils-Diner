using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Station Sprites", menuName = "Scriptable Objects/Station Sprites")]
public class StationSprites : ScriptableObject
{
    [SerializeField]
    private Sprite grillSprite;
    [SerializeField]
    private Sprite cauldronSprite;
    [SerializeField]
    private Sprite cboardSprite;

    public Sprite GetSprite(StationType type)
    {
        switch (type)
        {
            case StationType.Grill:
                return grillSprite;
            case StationType.Cauldron:
                return cauldronSprite;
            case StationType.ChoppingBoard:
                return cboardSprite;
            default:
                Debug.LogError("Station type does not exist");
                return null;
        }
    }
}
