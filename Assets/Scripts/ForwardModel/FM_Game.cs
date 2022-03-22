//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
using Monte;

public class FM_Game
{
    public AIState state;
    //public FM_Player player;
    //public FM_Monster monster;
    //public FM_Grid grid;

    //we must keep track of where the bullets and bombs are as these are not fully part of staterep
    //only their existence.

    public float[] apply(int action, AIState providedState)
    {
        switch (action)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            default: //default does nothing, so don't change providedState.stateRep
                break;
        }

        if (providedState.playerIndex == 0) //player has more things it can do
        {

        }

        return providedState.stateRep;
    }
}
