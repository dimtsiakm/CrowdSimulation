using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Location : MonoBehaviour
{
    public float maxInfluence;
    public string Name { get { return this.GetType().Name; } }

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
}
