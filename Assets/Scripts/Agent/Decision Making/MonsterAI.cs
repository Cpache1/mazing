using Monte;
using System;
using System.Collections.Generic;
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
    private List<ProjectileStruct> latestProjectilesStates = new List<ProjectileStruct>();
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
            ApplyAction(act);

            //reset MCTS 
            ai.reset();

            //And increment the number of decisions
            numbMovesPlayed++;
        }
    }

    private void ApplyAction(float[] action)
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
        latestProjectilesStates.Clear();

        //get current state info from level manager
        latestStateRep = levelManager.GetCurrentState();
        //get current info from bullets and fires
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("projectile");
        for (int i = 0; i < bullets.Length; i++)
        {
            ProjectileStruct fmBullet = new ProjectileStruct();
            GameObject unityBullet = bullets[i];
            fmBullet.x = unityBullet.transform.position.x;
            fmBullet.y = unityBullet.transform.position.y;

            float degrees = unityBullet.transform.eulerAngles.z;
            fmBullet.dirX = (float)Math.Cos(degrees * Math.PI / 180);
            fmBullet.dirY = (float)Math.Sin(degrees * Math.PI / 180);
            latestProjectilesStates.Add(fmBullet);
        }
        GameObject[] bombs = GameObject.FindGameObjectsWithTag("fire");
        for (int i = 0; i < bullets.Length; i++)
        {
            //TODO: Similar here.
            //latestProjectilesStates.Add(bomb);
        }

        //create the state based on that.
        latestAIState = new MonsterAIState(monsterIndx, null, 0, latestStateRep, latestProjectilesStates);
    }


}
