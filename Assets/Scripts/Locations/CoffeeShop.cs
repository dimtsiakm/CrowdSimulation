using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoffeeShop : Location
{
    public override float CalculateInfluence(float time)
    {
        if (isClosed)
            return 0;

        //06:00 to 08:00
        if (time < (2 / DAY_HOURS))
            return 0f;
        //08:00 to 10:00
        else if (time >= (2 / DAY_HOURS) && time < (4 / DAY_HOURS))
            return IncreaseCos(0, maxInfluence, (time - (2 / DAY_HOURS)) / (2 / DAY_HOURS));
        //10:00 to 14:00
        else if (time >= (4 / DAY_HOURS) && time < (8 / DAY_HOURS))
            return DecreaseCos(maxInfluence / 2, maxInfluence, (time - (4 / DAY_HOURS)) / (4 / DAY_HOURS));
        //14:00 to 17:00
        else if (time >= (8 / DAY_HOURS) && time < (11 / DAY_HOURS))
            return IncreaseCos(maxInfluence / 2, maxInfluence, (time - (8 / DAY_HOURS)) / (3 / DAY_HOURS));
        //17:00 to 20:00
        else if (time >= (11 / DAY_HOURS) && time < (14 / DAY_HOURS))
            return DecreaseCos(0, maxInfluence, (time - (11 / DAY_HOURS)) / (3 / DAY_HOURS));
        else
            return 0;
    }
}
