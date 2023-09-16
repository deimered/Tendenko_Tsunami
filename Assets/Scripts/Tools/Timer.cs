using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Timer
{
    //Initial value equal to 1 so there is no error in the function GetTimePercentage.
    private static float initialTime = 1;
    private static float timeInSeconds = 0;
    private static int minutes = 0;
    private static int seconds = 0;

    public static float TimeInSeconds
    {
        get { return timeInSeconds; }
    }

    public static float InitialTime
    {
        get { return timeInSeconds; }
    }

    public static void SetTime(int min, int sec)
    {
        timeInSeconds = min * 60 + sec;
        initialTime = timeInSeconds;
        minutes = min;
        seconds = sec;
    }

    public static void UpdateTimer(float timePassed)
    {
        if (timeInSeconds > 0)
        {
            timeInSeconds = timeInSeconds - timePassed > 0 ? timeInSeconds - timePassed : 0;

            minutes = Mathf.FloorToInt(timeInSeconds / 60);
            seconds = Mathf.FloorToInt(timeInSeconds % 60);
        }
    }

    public static void SubtractTimer(int min, int sec)
    {
        timeInSeconds = Mathf.Max(timeInSeconds - (min * 60 + sec), 0);
    }

    public static void AddTimer(int min, int sec)
    {
        timeInSeconds += (min * 60 + sec);
    }

    public static string GetCurrentTime()
    {
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public static float GetTimePercentage()
    {
        return (timeInSeconds / initialTime) * 100;
    }
}
