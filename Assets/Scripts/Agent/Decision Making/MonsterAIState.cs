/*
Written by: Charlie Ringer, found at https://github.com/charlieringer/MonteExamples 
Library released under MIT License
Modified by: Cristiana Pacheco
*/


using Monte;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAIState : AIState
{
    FM fm;
    //New State
    public MonsterAIState()
    {
        stateRep = new float[57];
        playerIndex = 0;
        parent = null;
        depth = 0;
        numbPieceTypes = 2; //TODO: This needs understanding. Some of the Model.cs functions depend on this.
        fm = GameObject.Find("LevelManager").GetComponent<FM>();
    }

    //New Child State (of another).
    public MonsterAIState(int pIndex, AIState _parent, int _depth, float[] _stateRep) : base(pIndex, _parent, _depth,
        _stateRep, 2)
    {
        numbPieceTypes = 2; //TODO: Once number is figured out you need to change it here as well.
        fm = GameObject.Find("LevelManager").GetComponent<FM>();
    }

    public override List<AIState> generateChildren()
    {
        //List of children
        children = new List<AIState>();

        //if the game is already over don't bother
        if (getWinner() >= 0)
        {
            return children;
        }
        //Swap the player
        int newPIndx = (playerIndex == 0) ? 1 : 0;
        //int numActions = (newPIndx == 0) ? PlayerAIMovement.NUM_ACTIONS : MonsterAI.NUM_ACTIONS;

        //Remember: actions are only based on movement
        for (int h = -1; h <= 1; h++) //horizontal input
        {
            for (int v = -1; v <= 1; v++) //vertical input
            {
                float[] act = { h, v };
                float[] newState = Apply(act, (float[])stateRep.Clone(), newPIndx);
                MonsterAIState childState = new MonsterAIState(newPIndx, this, depth + 1, newState);
                childState.stateAction = act;
                children.Add(childState);
            }
        }

        return children;
    }


    public override int getWinner()
    {
        //stateRep[43] represents playerDied and stateRep[48] represents botDied
        if (stateRep[43] == 0 && stateRep[48] == 1)
        {
            return 0; //0, player wins
        }
        else if (stateRep[43] == 1) 
        {
            return 1; //1, enemy wins
        }
        //2 draw ?

        //Game is still going
        return -1;
    }

    private float[] Apply(float[] action, float[] providedState, int idx)
    {
        //go to the forward model and change the game with this state (including player and other gameobjects)
        return fm.UpdateGameState(action, providedState, idx);
    }
}
