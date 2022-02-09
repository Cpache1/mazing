using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monte;

public class MonsterAI : MonoBehaviour
{
	//The AI being used
	public AIAgent ai;
	//Check if the game is still going/decision making is still needed
	//public bool gamePlaying = true;

	//How many moves/decisions have been made
	//protected int numbMovesPlayed = 0;

	//The latest board state
	//protected AIState latestAIState = null;
	//Which side the player is playing as.
	//public int playerIndx;
	//The
	//protected int[] latestStateRep;
	


	// Start is called before the first frame update
	void Start()
    {
		//Make a blank state
		//latestAIState = new MonsterAIState();
		//make it that the AI hasn't played yet or decided anything.
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	//Fro when games are run
	/*public void runGame()
	{
	//TODO: instead of some "turn" it will be "time" in our case. So, how often is it supposed to make a decision? Every second? Twice per 60FPS?
		//If it is not the human player's turn and the game is playing
		if (!(currentPlayersTurn == playerIndx) && gamePlaying)
		{
			//Turn on the AIThinking popup
			AIThinking.SetActive(true);
			//Check the AI
			int result = checkAI();
			//If it is >= 0 the game is over
			if (result >= 0)
			{
				//If 2 then a draw
				if (result == 2)
				{
					winlose.text = "You drew!";
					//If playerIndx then win
				}
				else if (result == playerIndx)
				{
					winlose.text = "You won!";
					//Otherwise loss
				}
				else
				{
					winlose.text = "You lost!";
				}

				//Game is over
				gamePlaying = false;
				//TODO : some reset needed? Check in Movement.cs what happens when dead
			}
			//Otherwise turn the AI thinking pop up off (and wait for the player to make a move)
		}
	}

	//For catching what move the AI has made.
	public void updateBoard()
	{
		//Safety check
		if (latestStateRep == null) return;
		//Loop through the game state
		for (int i = 0; i < latestStateRep.Length; i++)
		{
			//And make sure if a move has been played here it has been visually updated.
			if (latestStateRep[i] > 0) board[i].GetComponent<Tile>().playHere(latestStateRep[i]);
		}
	}*/

	//Called each time the update loop checks the AI progress
	/*public override int checkAI()
	{
		//If the AI has not stated
		if (!ai.started)
		{
			//Start it with the current state.
			AIState currentState = new TTTAIState((currentPlayersTurn + 1) % 2, null, 0, latestStateRep);
			ai.run(currentState);
		}
		//Otherwise if the AI is done 
		else if (ai.done)
		{
			//Get the next state (after the AI has moved)
			TTTAIState nextAIState = (TTTAIState)ai.next;
			//Unpack the state 
			latestAIState = nextAIState;
			latestStateRep = nextAIState.stateRep;
			//Reset the AI
			ai.reset();


			//And increment the number of moves
			numbMovesPlayed++;
		}
		//Return who the winner is
		return latestAIState == null ? -1 : (latestAIState.getWinner());
	}*/



	//Handles the player clicking a tile. TODO: we need something that is checking the player out and, once it moves to reassess (check latest
	//state essentially...
	//TODO
	/*Something is needed to check more often whether or not the game is lost/needs reset
	public override void handlePlayerAt(int x, int y)
	{
		//Get the staterep location and updating it with the correct value
		latestStateRep[x * 3 + y] = playerIndx == 0 ? 2 : 1;
		

		//Set up the last state
		latestAIState = new TTTAIState(playerIndx, null, 0, latestStateRep);

		//Find out the result of the board
		int result = latestAIState.getWinner();

		//And the end game as such
		if (result >= 0)
		{
			if (result == 2)
			{
				winlose.text = "You drew!";
			}
			else if (result == playerIndx)
			{
				winlose.text = "You won!";
			}
			else
			{
				winlose.text = "You lost!";
			}
			gamePlaying = false;
		}
	}*/


}
