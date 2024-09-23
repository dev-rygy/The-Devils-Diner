using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] 
    private float highlightDuration = 0.1f;
    [Header("Debugs")]
    [SerializeField]
    private List<SpriteRenderer> highlightableSpriteRenderers = new List<SpriteRenderer>();

    private void Start()
    {
        var spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer renderer in spriteRenderers)
            if (renderer.material.HasProperty("_OutlineAlpha"))
                highlightableSpriteRenderers.Add(renderer);
    }

    public void Highlight(bool enable)
    {
        Highlight(enable, Color.white);
    }

    public void Highlight(bool enable, Color color)
    {
        if (highlightableSpriteRenderers.Count <= 0)
        {
            //DebugConsole.LogWarning(name + "interactable has no highlightable sprite");
            return;
        }

        foreach (SpriteRenderer renderer in highlightableSpriteRenderers)
        {
            StartCoroutine(HighlightCoroutine(renderer, enable, color));
        }
    }

    IEnumerator HighlightCoroutine(SpriteRenderer renderer, bool enable, Color color)
    {
        float alpha = renderer.material.GetFloat("_OutlineAlpha");
        renderer.material.SetColor("_OutlineColor", color);

        float elapsedTime = 0;

        while (elapsedTime < highlightDuration)
        {
            if (enable)
                alpha = Mathf.Lerp(alpha, 1, 0.01f * elapsedTime / highlightDuration);
            else
                alpha = Mathf.Lerp(alpha, 0, 0.01f * elapsedTime / highlightDuration);

            renderer.material.SetFloat("_OutlineAlpha", alpha);

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        renderer.material.SetFloat("_OutlineAlpha", enable ? 1 : 0);
    }

}
