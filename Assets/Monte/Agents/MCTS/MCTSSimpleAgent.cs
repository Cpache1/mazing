/*
Author: Charlie Ringer, found at https://github.com/charlieringer/Monte 
Library released under MIT License
*/

using System;
using UnityEngine;
using System.Collections.Generic;
using MLHelperClasses;
using Microsoft.ML;
using System.IO;

namespace Monte
{
    //The most basic MCTS Agent. Uses a simple UCT move selection policy and light, random, rollouts


    public class MCTSSimpleAgent : MCTSMasterAgent
    {
        private bool hasLoadedModel = false;
        MLContext ctx;
        ITransformer loadedModel;
        PredictionEngine<RFData, RFPrediction> predictionEngine;

        const float MAX_DISTANCE = 45.0f;
        const float MAX_HEALTH = 100.0f;
        const float HIGH_POSITIVE = 100000.0f;
        const float HIGH_NEGATIVE = -100000.0f;

        List<Vector2> navPoints = new List<Vector2>();

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

            //No macro:
            //nextActionId = runMCTS(initialState, a_timeDue);
            //return;

            navPoints.Clear();

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

            //DEBUG
            FM_VisualTest gOb = GameObject.FindGameObjectWithTag("VisualTest").GetComponent<FM_VisualTest>();
            //List<Vector2> test = new List<Vector2>();
            //test.Add(new Vector2(0, 0));
            gOb.setNavPoints(navPoints);
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

            navPoints.Add(new Vector2(initialState.stateRep[3], initialState.stateRep[4]));

            //Start a count
            int numIterations = 0;
            //long remaining = timeDue - LevelManager.CurrentTimeMillis();
            //numbSimulations = 100;
            while (numIterations < 400) //uncomment this for number of simulations instead.
            //while (remaining > 0)   //Whilst time allows
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
                //remaining = timeDue - LevelManager.CurrentTimeMillis();
            }


            //Debug.Log("Iterations: "  + numIterations);

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

            if (bestIndex == -1)
            {
                bestIndex = 4; // randGen.Next(0, node.children.Length);
            }
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
            navPoints.Add(new Vector2(nextState.stateRep[3], nextState.stateRep[4]));

            bool terminalStateFound = (nextState.getWinner() > -1);
            //bool terminalStateFound = (nextState == null) || (nextState.getWinner() > -1);
            int rolloutCount = 0;

            float value = 0.0f;
            if(maxRollout > 0)
            { 
                AIState rolloutState = nextState.clone();
                while (!terminalStateFound && rolloutCount < maxRollout)
                {
                    rolloutCount++;
                    int indexAct = randGen.Next(rolloutState.children.Length);
                    rolloutState.ApplyActionToChild(indexAct, macroActionLength);
                    terminalStateFound = (rolloutState.getWinner() > -1);
                }
                value = evalState(initialState, rolloutState);
            }
            else
            {
                value = evalState(initialState, nextState);
            }
            

            nextState.addResult(value);

        }

        private float evalState(AIState rootState, AIState endState)
        {
            float w_mcts = 1.0f;
            float w_model = 0.0f;

            float s_mcts = winningScore(rootState, endState);
            float s_model = 0.0f;

            if (useModel)
            {
                //Debug.Log("YES2");
                w_mcts = 0.5f;
                w_model = 0.5f;
                s_model = playerModelScore(endState);
            }
            

            return w_mcts * s_mcts + w_model * s_model;

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

            float terminalScore = 0.0f;

            //Default win/lose conditions.
            //if (endState.getWinner() == 0) // Player wins
           //     terminalScore += HIGH_NEGATIVE;
            //else 
            if (endState.getWinner() == 1) // Bot wins.
                terminalScore += HIGH_POSITIVE;

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

            float wDist = 0.5f;//0.5f; 20
            float wHealth = 0.25f;//0.25f; 55
            float wCollision = 0.25f;//0.25f; 25

            //Debug.Log(diffDist);
            if (useModel)
            {
                //Debug.Log("YES");
                wDist = 0.80f;//0.5f; 20
                wHealth = 0.10f;//0.25f; 55
                wCollision = 0.10f;//0.25f; 25
            }



            float nonTerminalScore = diffDist * wDist + diffHealth * wHealth + collisionScore * wCollision;
            

            //Debug.Log(actualCollisions + " => " + collisionScore);

            return terminalScore + nonTerminalScore;
        }



        private float playerModelScore(AIState endState)
        {
            LoadMLModel();
            float pred = MakePrediction(endState.stateRep);
            return pred;
        }

        void LoadMLModel()
        {
            if (!hasLoadedModel)
            {
                hasLoadedModel = true;
                var modelPath = Application.dataPath + "\\RandomForest\\Models\\BTRACE_025_10_64_1.zip";
                //var modelPath = Application.dataPath + "\\BTRACE_02_5_32_1.zip";
                //".\\Assets\\RandomForest\\Models\\BTRACE_02_5_32_1.zip";
                //Debug.Log("Creating Context Object");
                ctx = new MLContext();


                loadedModel = ctx.Model.Load(modelPath, out DataViewSchema inputSchema);
                //Debug.Log("Model Loaded");

                predictionEngine = ctx.Model.CreatePredictionEngine<RFData, RFPrediction>(loadedModel);
                //Debug.Log("Prediction Engine Created");
            }

        }

        private float MakePrediction(float[] stateRep)
        {
            RFData state = new RFData
            {
                idleTime = stateRep[0],
                score = stateRep[1],

                botDistanceTraveled = stateRep[2],
                botPositionX = stateRep[3],
                botPositionY = stateRep[4],
                botRotation = stateRep[5],
                botSpeed = stateRep[6],
                botRotationSpeed = stateRep[7],
                botViewAngle = stateRep[8],
                botViewRadius = stateRep[9],
                botSearching = stateRep[10],
                botSearchTurns = stateRep[11],
                botHearingRadius = stateRep[12],
                botHearingProbability = stateRep[13],
                botHealth = stateRep[14],
                botFrustration = stateRep[15],
                botRiskTakingFactor = stateRep[16],
                botTakingRiskyPath = stateRep[17],
                botSeeingPlayer = stateRep[18],
                botChasingPlayer = stateRep[19],

                botDistanceFromPlayer = stateRep[20],
                cursorDistanceFromPlayer = stateRep[21],
                cursorDistanceFromBot = stateRep[22],
                playerDistanceTravelled = stateRep[23],
                playerPositionX = stateRep[24],
                playerPositionY = stateRep[25],
                playerRotation = stateRep[26],
                playerHealth = stateRep[27],
                playerIsDashing = stateRep[28],
                playerTriesDashOnCD = stateRep[29],

                dashPressed = stateRep[30],
                cursorDistanceTraveled = stateRep[31],
                cursorPositionX = stateRep[32],
                cursorPositionY = stateRep[33],
                playerTriesToFireOnCD = stateRep[34],
                playerTriesToBombOnCD = stateRep[35],
                shotsFired = stateRep[36],
                bombDropped = stateRep[37],
                gunReloading = stateRep[38],
                bombReloading = stateRep[39],

                playerBurning = stateRep[40],
                playerHealing = stateRep[41],
                playerDeltaHealth = stateRep[42],
                playerDied = stateRep[43],
                botLostPlayer = stateRep[44],
                botSpottedPlayer = stateRep[45],
                botBurning = stateRep[46],
                botDeltaHealth = stateRep[47],
                botDied = stateRep[48],

                onScreenFires = stateRep[49],
                onScreenBullets = stateRep[50],

                keyPressCount = stateRep[51],
                gen_timePassed = stateRep[52],
                gen_inputIntensity = stateRep[53],
                gen_inputDiversity = stateRep[54],
                gen_activity = stateRep[55],
                gen_score = stateRep[56]
            };

            var prediction = predictionEngine.Predict(state);
            
            return prediction.Probability;

        }
    }
}