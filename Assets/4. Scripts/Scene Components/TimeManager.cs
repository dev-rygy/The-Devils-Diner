using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager main;

    private void Awake()
    {
        if (main == null)
            main = this;
        else
            Destroy(main);
    }
}
