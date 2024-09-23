using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FoodChoice", menuName = "Scriptable Objects/Food Choice")]
[Serializable]
public class FoodChoice : ScriptableObject
{
    [SerializeField]
    private FoodData[] foodDatas;

    public FoodData Random()
    {
        return Helper.GetRandomElement(foodDatas);
    }
}