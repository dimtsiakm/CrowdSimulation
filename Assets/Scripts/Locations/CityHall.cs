using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityHall : Location
{
    public override float CalculateInfluence(float time)
    {
        float range = Mathf.Cos(Mathf.PI + 2 * Mathf.PI * 2 * time);
        float scaling = (range + 1) / 2; // scaling from [-1, 1] to [0, 1]
        if (time < 0.5)
            return scaling * maxInfluence;
        else
            return 0f;
    }
}