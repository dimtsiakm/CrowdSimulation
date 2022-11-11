using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Bar : Location
{
    public override float CalculateInfluence(float time)
    {
        float range = Mathf.Cos(2 * Mathf.PI * time);
        float scaling = (range + 1) / 2; // scaling from [-1, 1] to [0, 1]
        return scaling * maxInfluence + 0.1f;
    }
}
