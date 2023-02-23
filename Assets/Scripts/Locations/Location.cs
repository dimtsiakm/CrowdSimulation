using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Location : MonoBehaviour
{
    public bool isClosed = false;
    public bool isWeekend = false;
    public float maxInfluence;

    private float startTimeClosed;
    private int hoursLocationClosed;

    [HideInInspector]
    public float DAY_HOURS = 24f;
    public string Name { get { return GetType().Name; } }

    /// <summary>
    /// Calculate the current influence of the location depending on the day's time [0-1].
    /// </summary>
    /// <param name="time"></param>
    /// <returns>The current location's influence</returns>
    public abstract float CalculateInfluence(float time);
    public void SetInfluence(float influence)
    {
        maxInfluence = influence;
    }

    public void AddInfluence(float infuence)
    {
        maxInfluence += infuence;
    }

    public void DecreaseInfluence(float infuence)
    {
        maxInfluence -= infuence;
    }

    /// <summary>
    /// It checks if the location is closed and, when the time arrives, reopens it.
    /// </summary>
    /// <param name="threshold"></param>
    /// <param name="time"></param>
    /// <returns>Boolean value: false if the location just closed, else true.</returns>
    public int checkLocationAvailability(float threshold, float time)
    {
        if (!isClosed)
        {
            float die = UnityEngine.Random.value;
            if (die < threshold)
            {
                isClosed = true;
                startTimeClosed = time;
                hoursLocationClosed = UnityEngine.Random.Range(2, 8);
                //Debug.LogError("Location " + Name + " is closed!");
                return -1;
            }
        }
        else
        {
            if(time >= (startTimeClosed + hoursLocationClosed/24f) % 1)
            {
                isClosed = false;
                //Debug.LogError("Location " + Name + " is open again! The total time was " + hoursLocationClosed);
                return 1;
            }
        }
        return 0;
    }

    public void SetLocationClose(bool value)
    {
        isClosed = value;
    }

    public void SetWeekend(bool value)
    {
        isWeekend = value;
    }

    public float IncreaseCos(float min, float max, float normalizedTime)
    {
        float value = Mathf.Cos(Mathf.PI + Mathf.PI * normalizedTime);
        //scale from one range to another. 
        float newValue = ((value + 1) * (max - min) / 2) + min;
        return newValue;
    }
    public float DecreaseCos(float min, float max, float normalizedTime)
    {
        float value = Mathf.Cos(Mathf.PI * normalizedTime);
        //scale from one range to another. 
        float newValue = ((value + 1) * (max - min) / 2) + min;
        return newValue;
    }
}
