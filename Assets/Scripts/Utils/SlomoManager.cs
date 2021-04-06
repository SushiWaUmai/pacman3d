using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SlomoManager
{
    private static float originalFixedDeltaTime;

    [RuntimeInitializeOnLoadMethod]
    private static void GetOriginalFixedDeltaTime()
    {
        originalFixedDeltaTime = Time.fixedDeltaTime;
    }

    public static void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = originalFixedDeltaTime * timeScale;
    }
}
