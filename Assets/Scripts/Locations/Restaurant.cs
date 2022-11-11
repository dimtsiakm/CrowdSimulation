using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restaurant : Location
{
    public override float CalculateInfluence(float time)
    {
        float range = Mathf.Cos(Mathf.PI + 2 * Mathf.PI * time);
        float scaling = (range + 1) / 2; // scaling from [-1, 1] to [0, 1]
        return scaling * maxInfluence;
    }
}
