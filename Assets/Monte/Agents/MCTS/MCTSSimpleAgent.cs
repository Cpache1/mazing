﻿/*
Author: Charlie Ringer, found at https://github.com/charlieringer/Monte 
Library released under MIT License
*/

using System;
using UnityEngine;
using System.Collections.Generic;

namespace Monte
{
    //The most basic MCTS Agent. Uses a simple UCT move selection policy and light, random, rollouts


    public class MCTSSimpleAgent : MCTSMasterAgent
    {
        const float MAX_DISTANCE = 45.0f;
        const float MAX_HEALTH = 100.0f;


        //Constructors
        public MCTSSimpleAgent(string file) : base(file) { }
        public MCTSSimpleAgent(int _numbSimulations, double _exploreWeight, int _maxRollout, double _drawScore) : base(_numbSimulations, _exploreWeight, _maxRollout, _drawScore) { }

        //Main MCTS algortim
        protected override void mainAlgorithm(AIState initialState, long a_timeDue)
        {
            //Generate all possible moves
            /*AIState[] children = initialState.generateChildren();
            //Select a random one
            int index = randGen.Next(children.Length);
            //And set next to it(unless no children were generated
            nextActionId = index;
            done = true;
            return;*/



            if (firstDecision)
            {
                resetMacro = true;
                remainingMASteps = macroActionLength - 1;
                firstDecision = false;
                lastMADecision = runMCTS(initialState, a_timeDue);
            }
            else
            {
                AIState macroActionStartState = rollStateMacroAction(initialState.clone());

                if (remainingMASteps > 0)
                {
            
                    //Still running a previous macro-action. Continue planning from state in tree
                    // at the end of that macro-action
                    runMCTS(macroActionStartState, a_timeDue);
                    remainingMASteps--;
                    resetMacro = false;

                }
                else if (remainingMASteps == 0) //Last iteration for this decision.
                {
                    resetMacro = true;
                    remainingMASteps = macroActionLength - 1;
                    lastMADecision = runMCTS(macroActionStartState, a_timeDue);
                }


            }

            // Set the action to execute by the MCTS agent
            if (lastMADecision == -1) lastMADecision = 0;
            nextActionId = lastMADecision;

        }

        private AIState rollStateMacroAction(AIState state)
        {
            int first = macroActionLength - remainingMASteps - 1;
            for (int i = first; i < macroActionLength; ++i)
            {
                //make the moves to advance the game state.
                state.ApplyActionToChild(lastMADecision, macroActionLength);
            }
            return state;
        }

        private int runMCTS(AIState initialState, long timeDue)
        {
            //if you're supposed to interrupt don't provide a state or say it's done
            if (interrupt)
            {
                Interrupt();
                return -1;
            }

            //Start a count
            int numIterations = 0;
            long remaining = timeDue - LevelManager.CurrentTimeMillis();
            //numbSimulations = 100;
            //while (numIterations < numbSimulations) //uncomment this for number of simulations instead.
            while (remaining > 0)   //Whilst time allows
            {
                //Increment the count
                numIterations++;
                //Start at the inital state
                AIState bestNode = initialState;
                int nextToExpand = -1;
                //And loop through its child
                while (bestNode != null)
                {
                    AIState nextNode = ucb(bestNode, out nextToExpand); //Goes down the tree selecting the best next node with UCB.
                    if (nextNode == null)
                    {
                        nextNode = bestNode;
                        break;
                    }
                    bestNode = nextNode;
                }

                //Finally roll out this node. Includes Back-prop.
                rollout(initialState, bestNode, nextToExpand);
                remaining = timeDue - LevelManager.CurrentTimeMillis();
            }


            Debug.Log("Iterations: "  + numIterations);

            return recommendation(initialState);
        }


        private AIState ucb(AIState node, out int childIdx)
        {
            //Set the scores as a base line
            double bestScore = -1;
            int bestIndex = -1;

            //Loop thorugh all of the children
            for (int i = 0; i < node.children.Length; i++)
            {
                if (node.children[i] == null)
                {
                    childIdx = i;
                    return null; // Expansion phase
                }

                //win score is basically just wins/games unless no games have been played, then it is 1
                double wins = node.children[i].wins;
                double games = node.children[i].totGames;
                double score = (games > 0) ? wins / games : 1.0;

                //UCT (Upper Confidence Bound 1 applied to trees) function balances explore vs exploit.
                //Because we want to change things the constant is configurable.
                double exploreRating = exploreWeight * Math.Sqrt((Math.Log(node.totGames + 1) / (games + 0.1)));
                double noise = randGen.NextDouble() * epsilon;
                //Total score is win score + explore score + noise for tie-breaks.
                double totalScore = score + exploreRating + noise;
                //If the score is better update
                if (totalScore > bestScore)
                {
                    bestScore = totalScore;
                    bestIndex = i;
                }
            }

            //Set the best child for the next iteration
            childIdx = bestIndex;
            return node.children[bestIndex];
        }

        private int recommendation(AIState initialState)
        {
            //Onces all the simulations have taken place we select the best move...
            double mostGames = -1.0;
            int bestMove = -1;
            //Loop through all children
            for (int i = 0; i < initialState.children.Length; i++)
            {
                //Find the one that was played the most (this is the best move as we are selecting the robust child)
                if (initialState.children[i] != null)
                {
                    double noise = randGen.NextDouble() * epsilon;
                    int games = initialState.children[i].totGames;
                    if (games + noise > mostGames)
                    {
                        mostGames = games + noise;
                        bestMove = i;
                    }
                }
                
            }

            //if you're supposed to interrupt don't provide a state or say it's done
            if (interrupt)
            {
                Interrupt();
                return -1;
            }

            //And we are done
            done = true;
            return bestMove;
        }


        //Rollout function (plays random moves till it hits a termination)
        protected void rollout(AIState initialState, AIState rolloutStart, int nodeToExpand)
        {
            //First, advance node with action. This will belong to the tree (EXPANSION)
            AIState nextState = rolloutStart.generateChild(nodeToExpand, macroActionLength);
            if (nextState == null)
            {
                rolloutStart.addResult(rolloutStart.getWinner());
                return;
            }

            bool terminalStateFound = (nextState == null) || (nextState.getWinner() > -1);
            int rolloutCount = 0;

            AIState rolloutState = nextState.clone();
            while (!terminalStateFound && rolloutCount < maxRollout)
            {
                rolloutCount++;
                int indexAct = randGen.Next(rolloutState.children.Length);
                rolloutState.ApplyActionToChild(indexAct, macroActionLength);
                terminalStateFound = (rolloutState.getWinner() > -1);
            }

            float value = rolloutState.getWinner();
            if (!terminalStateFound)
                value = evalState(initialState, rolloutState); 
            
            nextState.addResult(value);

        }

        private float evalState(AIState rootState, AIState endState)
        {
            float w0 = 1.0f; // 0.5f;
            float a0 = winningScore(rootState, endState);

            float w1 = 0.0f; // 0.5f;
            float a1 = playerModelScore(rootState, endState);

            return w0 * a0 + w1 * a1;

        }

        private float winningScore(AIState rootState, AIState endState)
        {
            // This function needs to evaluate how "good" a state is for the monster,
            // where "good" means closer to win the game.
            // Values close to 0 mean a bad state, and close to 1 mean good state.

            // Things that we can consider:
            //   * Distance to the player (using A* if possible). The closer the better.
            //   * Other things? health of the monster, burning, cursor distance from bot (this could offer safely approaching).
            //   * Other game state player attributes that could be useful?
            //   * Penalize collisions.

            //Default win/lose conditions.
            if (endState.getWinner() == 0) // Player wins
                return 0;
            else if (endState.getWinner() == 1) // Bot wins.
                return 1;

            // We want distance to go down.
            float initialDistance = rootState.stateRep[20];
            float finalDistance = endState.stateRep[20];
            float diffDist = (initialDistance - finalDistance) / MAX_DISTANCE;

            // Health to be up.
            float initialHealth = rootState.stateRep[14];
            float finalHealth = endState.stateRep[14];
            float diffHealth = (finalHealth - initialHealth) / MAX_HEALTH;

            //Collisions count.
            float maxCollisions = endState.depth;
            float actualCollisions = endState.numCollisions;
            float collisionScore = 1 - (actualCollisions / maxCollisions);

            //Debug.Log(diffDist);

            float totScore = diffDist * 0.75f + diffHealth * 0.20f  + collisionScore * 0.05f;

            return totScore;
        }



        private float playerModelScore(AIState rootState, AIState endState)
        {
            return 0.0f;
        }
    }
}