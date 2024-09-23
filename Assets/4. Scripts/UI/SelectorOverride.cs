using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorOverride : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private Vector2 anchoredPosition;
    [SerializeField]
    private Vector2 sizeOverride = new Vector2(2, 2);
    
    [SerializeField]
    private Transform positionOverride;

    public Vector2 PositionOverride => (positionOverride ? positionOverride.position : transform.position);
    public Vector2 SizeOverride => sizeOverride;

    [ContextMenu("Create Selector Override")]
    private void CreateSelectorOverride()
    {
        var obj = new GameObject("Selector Override");
        
        obj.transform.parent = transform;
        positionOverride = obj.transform;
        obj.transform.localPosition = Vector2.zero;
    }

    [ContextMenu("Test")]
    private void Test()
    {
        positionOverride.localPosition = anchoredPosition;
        Selector.main.Override(this);
    }
}
