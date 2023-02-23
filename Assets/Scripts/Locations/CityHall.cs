using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityHall : Location
{
    public override float CalculateInfluence(float time)
    {
        
        if(isClosed)
            return 0;

        if (isWeekend)
            return 0;

        //06:00 to 08:00
        if (time < (2 / DAY_HOURS))
            return 0f;
        //08:00 to 12:00
        else if (time >= (2 / DAY_HOURS) && time < (6 / DAY_HOURS))
            return IncreaseCos(0, maxInfluence, (time - (2 / DAY_HOURS)) / (4 / DAY_HOURS));
        //12:00 to 15:00
        else if (time >= (6 / DAY_HOURS) && time < (9 / DAY_HOURS))
            return DecreaseCos(0, maxInfluence, (time - (6 / DAY_HOURS)) / (3 / DAY_HOURS));
        else
            return 0;
    }
}