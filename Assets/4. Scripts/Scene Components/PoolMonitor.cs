using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolMonitor : MonoBehaviour
{
#if UNITY_EDITOR
    private void Update()
    {
        var activeCount = 0;
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeInHierarchy)
                activeCount++;
        }

        name = (name.Contains(" (") ? name.Split(" (")[0] : name)
            + $" ({activeCount}/{transform.childCount})";
    }
#endif
}
