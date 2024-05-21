using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

/// <summary>
/// Handles anything related to Epoch Time
/// </summary>
public class Epoch : MonoBehaviour
{
    /// <summary>
    /// Returns the Unix/Epoch time in seconds as an integer
    /// </summary>
    /// <returns></returns>
    public static int GetCurrentTime()
    {
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return (int)(DateTime.UtcNow - epochStart).TotalSeconds;
    }

    /// <summary>
    /// Returns the Unix/Epoch time in seconds as an float
    /// </summary>
    /// <returns></returns>
    // TODO: See if it has any actual use
    public static float GetCurrentTimeFloat()
    {
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return (float)(DateTime.UtcNow - epochStart).TotalSeconds;
    }

    /// <summary>
    /// Convert a time in seconds to unix time
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    public static int SecondsToEpoch(int duration)
    {
        return GetCurrentTime() + duration;
    }

    /// <summary>
    /// Convert unix time to seconds
    /// </summary>
    /// <param name="epoch"></param>
    /// <returns></returns>
    public static int EpochToSeconds(int epoch)
    {
        return epoch - GetCurrentTime();
    }
}
