using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pharmacy : Location
{
    public override float CalculateInfluence(float time)
    {

        float minProb = 0.05f;

        //06:00 to 07:00
        if (time < (1 / DAY_HOURS))
            return minProb;
        //7:00 to 10:00
        else if (time >= (1 / DAY_HOURS) && time < (4 / DAY_HOURS))
            return IncreaseCos(minProb, maxInfluence*0.2f, (time - (1 / DAY_HOURS)) / (3 / DAY_HOURS));
        //10:00 to 18:00
        else if (time >= (4 / DAY_HOURS) && time < (12 / DAY_HOURS))
            return DecreaseCos(minProb, maxInfluence*0.2f, (time - (4 / DAY_HOURS)) / (8 / DAY_HOURS));
        else
            return minProb;

    }
}
