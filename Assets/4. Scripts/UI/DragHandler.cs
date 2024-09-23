using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour
{
    [SerializeField]
    private bool isDraggable;

    private Canvas canvas;
    private RectTransform canvasRect;
    private new Camera camera;
    
    public bool IsDraggable { get { return isDraggable; } set { isDraggable = value; } }

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();
        camera = canvas.worldCamera;
    }

    public void OnEventTrigger(BaseEventData data)
    {
        if (!IsDraggable) return;

        PointerEventData pointerData = (PointerEventData)data;
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, pointerData.position, camera, out position);
        transform.position = canvas.transform.TransformPoint(position);
    }
}
