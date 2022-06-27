/*
Written by: Charlie Ringer, found at https://github.com/charlieringer/MonteExamples 
Library released under MIT License
Modified by: Cristiana Pacheco
*/


using Monte;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;

public class MonsterAIState : AIState
{
    FM fm;


    const int numActions = 9;
    public static float[,] allActions = new float[numActions, 2] { {-1,-1}, { -1, 0}, { -1, 1 },
                                                                    {0,-1},  { 0, 0},  { 0, 1 },
                                                                    {1,-1},  { 1, 0},  { 1, 1 } };
    public List<ProjectileStruct> projectilesStates;


    //New State
    public MonsterAIState()
    {
        stateRep = new float[57];
        children = new AIState[numActions];
        playerIndex = 0;
        parent = null;
        depth = 0;
        fm = GameObject.Find("LevelManager").GetComponent<FM>();
        projectilesStates = new List<ProjectileStruct>();
    }

    //New Child State (of another).

    public MonsterAIState(int pIndex, AIState _parent, int _depth, float[] _stateRep, List<ProjectileStruct> _projectiles) : 
        base(pIndex, _parent, _depth, _stateRep)
    {
        children = new AIState[numActions];
        fm = GameObject.Find("LevelManager").GetComponent<FM>();
        projectilesStates = _projectiles;
    }


    public override AIState clone()
    {
        MonsterAIState newState = new MonsterAIState();
        for (int i = 0; i < stateRep.Length; i++) newState.stateRep[i] = this.stateRep[i];
        children = new AIState[numActions]; //We do not clone children
        newState.playerIndex = this.playerIndex;
        newState.parent = this.parent;
        newState.depth = this.depth;
        newState.numCollisions = this.numCollisions;

        newState.projectilesStates = new List<ProjectileStruct>();
        foreach (ProjectileStruct ps in this.projectilesStates)
        {
            ProjectileStruct psCopy = ps.GetCopy();
            newState.projectilesStates.Add(psCopy);
        }

        return newState;
    }



    public override void ApplyActionToChild(int actionId, int macroActionLength = 1)
    {
        stateRep = ApplyMacroAction(actionId, macroActionLength, (float[])stateRep.Clone());
        depth++;
    }

    public override AIState generateChild(int actionId, int macroActionLength = 1)
    {
        if (getWinner() >= 0) return null;

        float[] newState = ApplyMacroAction(actionId, macroActionLength, (float[])stateRep.Clone());
        List<ProjectileStruct> newProjectilesStates = fm.GetProjectileStructs();
        MonsterAIState childState = new MonsterAIState(playerIndex, this, depth + 1, newState, newProjectilesStates);
        childState.stateActionId = actionId;
        children[actionId] = childState;

        return childState;
    }

    public override AIState[] generateChildren(int macroActionLength = 1)
    {
        //if the game is already over don't bother
        if (getWinner() >= 0)
            return children;

        //foreach (ActionType at in Enum.GetValues(typeof(ActionType)))
        for(int childIdx = 0; childIdx < children.Length; childIdx++)
            generateChild(childIdx, macroActionLength);

        return children;
    }

    public override float[] getAction(int actionId)
    {
        float[] act = new float[2];
        act[0] = allActions[actionId, 0];
        act[1] = allActions[actionId, 1];
        return act;
    }


    private float[] ApplyMacroAction(int actionId, int macroActionLength, float[] state)
    {
        try { 
            float[] act = getAction(actionId);
            float[] newState = (float[])stateRep.Clone();
            //As many actions as the macro action length
            bool gameOver = isGameOver(newState); //should be false...
                                                  
            //As many actions as the macro action length
            for (int actionIdx = 0; !gameOver && actionIdx < macroActionLength; actionIdx++)
            {
                newState = ApplySingleAction(act, newState, playerIndex);
                gameOver = isGameOver(newState);
            }

            return newState;
        }
        catch (Exception e) 
        {
            int a = 0;

            //Debug.Log(e.StackTrace);
            
            float[] act = getAction(actionId);
            float[] newState = (float[])stateRep.Clone();
            //As many actions as the macro action length
            bool gameOver = isGameOver(newState); //should be false...

            //As many actions as the macro action length
            for (int actionIdx = 0; !gameOver && actionIdx < macroActionLength; actionIdx++)
            {
                newState = ApplySingleAction(act, newState, playerIndex);
                gameOver = isGameOver(newState);
            }

            return newState;
        }

        return null;
    }

    private bool isGameOver(float[] stateRep)
    {
        return ((stateRep[43] == 0 && stateRep[48] == 1) || (stateRep[43] == 1)) ;
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
            return 1; //1, enemy (bot) wins
        }

        //Game is still going
        return -1;
    }

    private float[] ApplySingleAction(float[] action, float[] providedState, int idx)
    {
        float[] nextState = fm.UpdateGameState(action, providedState, projectilesStates, idx);

        bool collision = fm.GetGame().monster.GetMovementComponent().DidMonsterCollide();
        if (collision)
            numCollisions++;

      // return nextState;

        return playerAction(nextState, 1 - idx);
    }

    private float[] playerAction(float[] state, int idx)
    {

        float[] playerMovement = { 0.0f, 0.0f };

        Vector2 diff = fm.GetGame().monster.GetPosition() - fm.GetGame().player.GetPosition();
        Vector2 dir = diff.normalized;
        float monsterPlayerDistance = diff.magnitude;
        bool monsterTooClose = monsterPlayerDistance <= 15.0f ? true : false;


        //run until another distance is established
        if (monsterTooClose)
        {
            //run to opposite
            playerMovement[0] = -dir.x;
            playerMovement[1] = -dir.y;

            // shooting:
            if (new System.Random().NextDouble() < 0.2)
                fm.GetGame().GiveGunInputs(true, false);
            if (new System.Random().NextDouble() < 0.1)
                fm.GetGame().GiveGunInputs(false, true);
        }
        else //go to it
        {
            playerMovement[0] = dir.x;
            playerMovement[1] = dir.y;

            // shooting/bombing:
            if (new System.Random().NextDouble() < 0.1)
                fm.GetGame().GiveGunInputs(true, false);
            if (new System.Random().NextDouble() < 0.1)
                fm.GetGame().GiveGunInputs(false, true);
        }



        float[] stateAfterPlayerPlays = fm.UpdateGameState(playerMovement, state, projectilesStates, idx);

        projectilesStates = fm.GetProjectileStructs();

        return stateAfterPlayerPlays;
    }

}
