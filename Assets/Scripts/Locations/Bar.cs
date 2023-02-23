using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Bar : Location
{
    public override float CalculateInfluence(float time)
    {
        if (isClosed)
            return 0f;

        if (isWeekend)
        {
            //06:00 to 19:00
            if (time < (11 / DAY_HOURS))
                return 0f;
            //19:00 to 22:00
            else if (time >= (11 / DAY_HOURS) && time < (14 / DAY_HOURS))
                return IncreaseCos(0, maxInfluence, (time - (11 / DAY_HOURS)) / (3 / DAY_HOURS));
            //22:00 to 03:00
            else if (time >= (14 / DAY_HOURS) && time < (19 / DAY_HOURS))
                return DecreaseCos(0, maxInfluence, (time - (14 / DAY_HOURS)) / (5 / DAY_HOURS));
            else
                return 0;
        }
        else
        {
            //06:00 to 19:00
            if (time < (11f / DAY_HOURS))
                return 0f;
            //19:00 to 22:00
            else if (time >= (11 / DAY_HOURS) && time < (14 / DAY_HOURS))
                return IncreaseCos(0, maxInfluence, (time - (11 / DAY_HOURS)) / (3 / DAY_HOURS));
            //22:00 to 01:00
            else if (time >= (14 / DAY_HOURS) && time < (17 / DAY_HOURS))
                return DecreaseCos(0, maxInfluence, (time - (14 / DAY_HOURS)) / (3 / DAY_HOURS));
            else
                return 0; 
        }
    }
}
