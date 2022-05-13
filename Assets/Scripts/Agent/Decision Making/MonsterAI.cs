using Monte;
using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    [Range(2, 3f)]
    public float speed = 2f;
    [Range(1, 5)]
    public float rotationSpeed = 1f;

    //AI related
    public static int NUM_ACTIONS = 9;
	public AIAgent ai;
    public int monsterIndx = 1; //monster index (1, player is 0 by default).

    //State related
    private AIState latestAIState = null;
    private float[] latestStateRep;
    private long decisionTimer = 40; //milliseconds
    private LevelManager levelManager;
	private FM fm;

    //Game related
    public bool gamePlaying = true; //Check if the game is still going/decision making is still needed

    //How many moves/decisions have been made
    protected int numbMovesPlayed;


    // Start is called before the first frame update
    void Start()
    {
		//Get the level manager and the forward model.
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
		fm = levelManager.transform.GetComponent<FM>();
		//Reset algorithm.
		numbMovesPlayed = 0;
		ai.reset();
    }

    // Update is called once per frame
    void Update()
    {
        //If the AI hasn't started 
        if (!ai.started)
        {
            //Update the latest state.
            getLatestState();

            //Run with the latest state.
            long a_timeDue = decisionTimer + LevelManager.CurrentTimeMillis();
            ai.run(latestAIState, a_timeDue);
        }
        //If AI has started and is now done.
        else if (ai.done)
        {
            //Get the next state (after the AI has moved)
            MonsterAIState nextAIState = (MonsterAIState)ai.next;

			//Get the action resulting from that state and apply the move. 
			float[] act = nextAIState.stateAction;
            Apply(act);

            //reset MCTS 
            ai.reset();

            //And increment the number of decisions
            numbMovesPlayed++;
        }

        //check if someone wins? //TODO: ?

    }

    private void Apply(float[] action)
    {
        Vector3 waypoint = new Vector3(transform.position.x + action[0], transform.position.y + action[1], transform.position.z);

        Quaternion rot = Quaternion.LookRotation(waypoint - transform.position, Vector3.back);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, rotationSpeed * 2);
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z);
        transform.position = Vector3.MoveTowards(transform.position, waypoint, speed * Time.deltaTime);
        //Correcting position z index
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }


    private void getLatestState()
    {
        //get current state info from level manager
        latestStateRep = levelManager.GetCurrentState();

        //create the state based on that.
        latestAIState = new MonsterAIState(monsterIndx, null, 0, latestStateRep);
    }


    //Fro when games are run
    /*public void runGame()
	{
	//TODO: instead of some "turn" it will be "time" in our case. So, how often is it supposed to make a decision? Every second? Twice per 60FPS?
		//If it is not the human player's turn and the game is playing
		if (!(currentPlayersTurn == playerIndx) && gamePlaying)
		{
			
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
*/
}
