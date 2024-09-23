using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "String Data", menuName = "Scriptable Objects/String")]
public class StringData : ScriptableObject
{
    [TextArea(6,10)]
    [SerializeField] private string data = " ";

    public string GetString()
    {
        return data;
    }
}
