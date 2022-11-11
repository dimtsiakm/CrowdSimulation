using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pharmacy : Location
{
    public override float CalculateInfluence(float time)
    {
        return 0.1f * maxInfluence;
    }
}
