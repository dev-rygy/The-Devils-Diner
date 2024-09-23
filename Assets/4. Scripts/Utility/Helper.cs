using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
    public static Vector2 GetRandomPoint(this Bounds bounds)
    {
        return new Vector2(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y)
        );
    }

    public static Vector3 halfUnit = new Vector3(0.5f, 0.5f);
    public static Vector2 SnapToGrid(this Vector3 position, int size = 1)
    {
        if (size == 2)
        {
            var snapX = Mathf.RoundToInt(position.x);
            var snapY = Mathf.RoundToInt(position.y);
            var offsetX = position.x < snapX ? -0.5f : 0.5f;
            var offsetY = position.y < snapY ? -0.5f : 0.5f;

            return new Vector2(snapX + offsetX, snapY + offsetY);
        }
        else
            return new Vector2(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
    }

    public static Vector2 SnapToGrid(this Vector2 position, int size = 1)
    {
        if (size == 2)
        {
            var snapX = Mathf.RoundToInt(position.x);
            var snapY = Mathf.RoundToInt(position.y);
            var offsetX = position.x < snapX ? -0.5f : 0.5f;
            var offsetY = position.y < snapY ? -0.5f : 0.5f;

            return new Vector2(snapX + offsetX, snapY + offsetY);
        }
        else
            return new Vector2(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
    }

    public static bool RangeCheck(Vector2 positionA, Vector2 positionB, float range)
    {
        return Vector3.Distance(positionA, positionB) < range;
    }

    public static IEnumerator GetValidCircleInRadius(Vector2 position, float radius, float circleRadius, LayerMask blockLayer, System.Action<Vector2> callback)
    {
        Vector2 result = Vector2.zero;
        Vector2 randomPoint = Vector2.zero;

        while (result == Vector2.zero)
        {
            randomPoint = (Vector2)(Random.insideUnitSphere * radius) + position;
            if (!Physics2D.OverlapCircle(randomPoint, circleRadius, blockLayer))
                result = randomPoint;

            yield return new WaitForSeconds(0.1f);
        }

        callback(result);
    }

    public static IEnumerator GetValidCircleInRadius(Vector2 position, float radius, float circleRadius, GameObject gameObject, LayerMask blockLayer, System.Action<Vector2, GameObject> callback)
    {
        Vector2 result = Vector2.zero;
        Vector2 randomPoint = Vector2.zero;

        while (result == Vector2.zero)
        {
            randomPoint = (Vector2)(Random.insideUnitSphere * radius) + position;
            if (!Physics2D.OverlapCircle(randomPoint, circleRadius, blockLayer))
                result = randomPoint;

            yield return new WaitForSeconds(0.1f);
        }

        callback(result, gameObject);
    }

    // Used to guarantee an event to happen after 24 hours in game with a base chance of 0.25
    public static float DeminishingIncreasedChance(int value, int population)
    {
        if (value > population - 1)
            value = population - 1;
        return (-1f / (10f * population) * value) + 0.1f;
    }

    public static IEnumerator PopIn(Transform transform, float duration = 0.5f)
    {
        float lerp = 0;
        while (lerp < 1)
        {
            transform.localScale =
                Vector3.Lerp(Vector3.zero, Vector3.one, lerp);

            lerp += Time.deltaTime / duration;
            yield return new WaitForFixedUpdate();
        }

        transform.localScale = Vector3.one;
    }

    public static IEnumerator PopOut(Transform transform, float duration = 0.5f)
    {
        float lerp = 0;
        while (lerp < 1)
        {
            transform.localScale =
                Vector3.Lerp(Vector3.one, Vector3.zero, lerp);

            lerp += Time.deltaTime / duration;
            yield return new WaitForFixedUpdate();
        }

        transform.localScale = Vector3.zero;
    }

    /// <summary>
    /// Adaptive refresh rate base on if the object is null(faster) or not(slower)
    /// </summary>
    public static float AdaptiveRefreshRate<T>(T obj)
    {
        return obj == null ? 0.1f : 0.25f;
    }

    /// <summary>
    /// Adaptive refresh rate base on if the condition is true(faster) or false(slower)
    /// </summary>
    public static float AdaptiveRefreshRate(bool condition)
    {
        return condition ? 0.1f : 0.25f;
    }

    public static bool NotNullAndHasElem<T>(this T[] array)
    {
        return (array != null && array.Length > 0);
    }

    public static T GetRandomElement<T>(this T[] array)
    {
        if (array == null || array.Length == 0)
            return default(T);
        else
            return array[Random.Range(0, array.Length)];
    }

    public static T GetRandomElement<T>(this List<T> list)
    {
        if (list == null || list.Count == 0)
            return default(T);
        else
            return list[Random.Range(0, list.Count)];
    }

    public static IEnumerator FlashColorCoroutine(this SpriteRenderer renderer, Color color, float duration = 0.1f)
    {
        if (renderer.color == Color.white)
        {
            renderer.color = color;
            yield return new WaitForSeconds(duration);
            renderer.color = Color.white;
        }
    }

    

    #region Fade In

    public static IEnumerator FadeIn(Transform transform, float duration = 0.5f)
    {
        var renderers = transform.GetComponentsInChildren<SpriteRenderer>();
        yield return FadeIn(renderers, duration);
    }

    public static IEnumerator FadeIn(SpriteRenderer[] renderers, float duration = 0.5f)
    {
        float lerp = 0;
        while (lerp < 1)
        {
            foreach (var renderer in renderers)
            {
                renderer.color = Color.Lerp(Color.clear, Color.white, lerp);
            }

            lerp += Time.deltaTime / duration;
            yield return new WaitForFixedUpdate();
        }

        foreach (var renderer in renderers)
        {
            renderer.color = Color.white;
        }
    }

    #endregion

    #region Fade Out

    public static IEnumerator FadeOut(Transform transform, float duration = 0.5f)
    {
        var renderers = transform.GetComponentsInChildren<SpriteRenderer>();
        yield return FadeIn(renderers, duration);
    }

    public static IEnumerator FadeOut(SpriteRenderer[] renderers, float duration = 0.5f)
    {
        float lerp = 0;
        while (lerp < 1)
        {
            foreach (var renderer in renderers)
            {
                renderer.color = Color.Lerp(Color.white, Color.clear, lerp);
            }

            lerp += Time.deltaTime / duration;
            yield return new WaitForFixedUpdate();
        }

        foreach (var renderer in renderers)
        {
            renderer.color = Color.clear;
        }
    }

    #endregion
}
