using Google.Protobuf.WellKnownTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restaurant : Location
{
    public override float CalculateInfluence(float time)
    {
        
        if (isClosed)
            return 0f;

        //06:00 to 12:00
        if (time < (6 / DAY_HOURS))
            return 0f;
        //12:00 to 14:00
        else if (time >= (6 / DAY_HOURS) && time < (8 / DAY_HOURS))
            return IncreaseCos(0, maxInfluence, (time - (6 / DAY_HOURS)) / (2 / DAY_HOURS));
        //14:00 to 20:00
        else if (time >= (8 / DAY_HOURS) && time < (16 / DAY_HOURS))
            return DecreaseCos(0, maxInfluence, (time - (8 / DAY_HOURS)) / (6 / DAY_HOURS));
        else
            return 0;

    }
}
