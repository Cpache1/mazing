/*
Written by: Charlie Ringer, found at https://github.com/charlieringer/MonteExamples 
Library released under MIT License
Modified by: Cristiana Pacheco
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monte;

public class MonsterAIState : AIState
{
    //New State
    public MonsterAIState()
    {
        //TODO
    }

    //New Child State (of another).
    public MonsterAIState(int pIndex, AIState _parent, int _depth, int[] _stateRep) : base(pIndex, _parent, _depth,
        _stateRep, 2)
    {
        numbPieceTypes = 2; //TODO
    }

    public override List<AIState> generateChildren()
    {//TODO
        //List of children
        children = new List<AIState>();
        return children;
    }

    public override int getWinner()
    {//TODO
        throw new System.NotImplementedException();

        //0, player wins
        //1, enemy wins
        //2 draw
        //-1 it's still going
    }


}
