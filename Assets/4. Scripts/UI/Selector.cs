using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Selector : MonoBehaviour
{
    public static Selector main;

    [Header("Settings")]
    [SerializeField]
    private bool showInEditor;
    [SerializeField]
    private Vector2 defaultSize = new Vector2(2, 2);

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        if (main == null)
            main = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }

    private void Update()
    {
        if (!Application.isPlaying)
            spriteRenderer.enabled = showInEditor;
    }

    public void Override(SelectorOverride selectorOverride)
    {
        MoveToNShow(selectorOverride.PositionOverride);
        OverrideSize(selectorOverride.SizeOverride);
    }

    public void OverrideSize(Vector2 size)
    {
        spriteRenderer.size = size;
    }

    public void MoveTo(Vector2 position)
    {
        transform.position = position;
    }

    public void Show(bool show)
    {
        spriteRenderer.enabled = show;
    }

    public void MoveToNShow(Vector2 position)
    {
        transform.position = position;
        spriteRenderer.enabled = true;
    }

    public void Select(GameObject gameObject)
    {
        if(gameObject.TryGetComponent<SelectorOverride>(out SelectorOverride selectorOverride))
        {
            Override(selectorOverride);
        }
        else
        {
            OverrideSize(defaultSize);
            MoveToNShow(gameObject.transform.position);
        }
    }
}
