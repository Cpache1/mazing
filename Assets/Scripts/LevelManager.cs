using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using Monte;
using System.Linq;

public class LevelManager : MonoBehaviour {    
    public bool moderatorActive;
    public bool doCountDown;
    public bool silentCountDown;
    public bool warnStart;
    public float timeLimit;
    private float countdown;
    private Text timer;

    public bool doScore;
    [HideInInspector]
    public int score;
    private Text scoreBoard;

    //For Level Reset
    [HideInInspector]
    public Vector3 agentStartPosition;
    [HideInInspector]
    public float agentStartFrustration;
    [HideInInspector]
    public Vector3 playerStartPosition;
    GameObject agent;
    GameObject player;

    //For Algorithm stuff
    private GameObject cursor;
    int algorithmIndx; 
    AIAgent agentType;
    Model model;
    string settings;
    float[] stateRep;
    private int tick;
    private int idleTime;
    private int fullTime;
    private List<string> keyPresses;

    private Vector3 previousPlayerPosition;
    private Vector3 previousBotPosition;
    private Vector3 previousCursorPosition;

    //Game Events
    public UnityEvent OnGameStart;
    public UnityEvent OnGameEnd;
    private bool gameEnded = false;
    private bool gameStarted = false;

    //FPS Counter
    private int fps = 0;
    private float fpsTimer;
    public float fpsRefreshRate = 1f;
    public Text fpsText;
    public int fpsLimit;
    private Color fpsIndicatorColor;
    private Color fpsWarningColor = new Color(0.9f, 0, 0.2f, 1);

    private void Awake() {
        keyPresses = new List<string>();
        if (doCountDown) {
            if (!silentCountDown) {
                timer = GameObject.Find("Timer").GetComponent<Text>();
            }
            countdown = timeLimit;
        }
        if (doScore) {
            scoreBoard = GameObject.Find("Score").GetComponent<Text>();
        }
        agent = GameObject.Find("Monster");
        player = GameObject.Find("PlayerController");

        if (agent != null) {
            agentStartFrustration = agent.GetComponent<FrustrationComponent>().levelOfFrustration;
            stateRep = new float[57];
            tick = 0;
        }

        if (player != null) {
            playerStartPosition = player.transform.position;
        }
    }

    private void Start() {
        fpsIndicatorColor = fpsText.color;

        if (!gameStarted) {
            gameStarted = true;
            StartCoroutine(LateStart());
            if (warnStart) {
                StartCoroutine(RemindStart());
            }
        }

        //Make the cursor invisible.
        Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
        Cursor.visible = false;
    }

    IEnumerator LateStart() {
        yield return new WaitForFixedUpdate();
        OnGameStart.Invoke();

        if (player.GetComponent<PlayerController>().isAI)
        {//CHEAT! When the player is AI the cursor is "always" on the enemy
            cursor = GameObject.Find("Monster"); 
        }
        else
        {
            cursor = GameObject.Find("GunControls");
        }
        previousPlayerPosition = player.transform.position;
        previousCursorPosition = cursor.transform.position;
        previousBotPosition = agent.transform.position;
        yield return null;
    }

    IEnumerator RemindStart() {
        yield return new WaitForSeconds(30);
        Button startButton = GameObject.Find("Start").GetComponent<Button>();
        ColorBlock cb = startButton.colors;
        cb.normalColor = fpsWarningColor;
        startButton.colors = cb;
        yield return null;
    }

    private void Update() {
        if (Time.unscaledTime > fpsTimer) {
            fps = (int)(1f / Time.unscaledDeltaTime);
            fpsText.text = "FPS: " + fps;
            fpsTimer = Time.unscaledTime + fpsRefreshRate;
            
            // Gives 3 seconds for the game to stabilize
            if (fps < fpsLimit && Time.timeSinceLevelLoad > 3f) {
                fpsText.color = fpsWarningColor;
            } else {
                fpsText.color = fpsIndicatorColor;
            }
        }
    }

    //TODO: Reset will needs additions for latest AI (MCTS)
    public void ResetStage(int reward) {
        //algorithm related
        tick = 0;
        keyPresses.Clear();
        for (int n = 0; n < stateRep.Length; n++)
            stateRep[n] = 0.0f;

        GameObject[] currentBombs = GameObject.FindGameObjectsWithTag("bomb");
        for (int i = 0; i < currentBombs.Length; i++) {
            //Defusing bombs before destroy prevents them spawing fires
            currentBombs[i].GetComponent<BombBehavior>().fuse = false;
            Destroy(currentBombs[i]);
        }
        GameObject[] currentProjectiles = GameObject.FindGameObjectsWithTag("projectile");
        for (int i = 0; i < currentProjectiles.Length; i++) {
            Destroy(currentProjectiles[i]);
        }
        GameObject[] currentFires = GameObject.FindGameObjectsWithTag("fire");
        for (int i = 0; i < currentFires.Length; i++) {
            Destroy(currentFires[i]);
        }

        player.GetComponent<PlayerHealth>().health = 100;
        agent.GetComponent<Health>().health = 100;
        agent.GetComponent<FrustrationComponent>().levelOfFrustration = agentStartFrustration;

        if (GameObject.Find("playerDestroyEffect(Clone)") == null) {
            Instantiate(player.GetComponent<PlayerHealth>().destroyEffect, new Vector3(player.transform.position.x, player.transform.position.y, -1), player.transform.rotation);
            score += reward;
        }

        player.transform.position = playerStartPosition;
        if (GameObject.Find("agentDestroyEffect(Clone)") == null) {
            Instantiate(agent.GetComponent<Health>().destroyEffect, new Vector3(agent.transform.position.x, agent.transform.position.y, -1), agent.transform.rotation);
        }

        agent.transform.position = agentStartPosition;

        //Monster Movement
        Movement agentStatus = agent.GetComponent<Movement>();
        agentStatus.shouldLookAround = true;
        agentStatus.shouldWait = true;
        agentStatus.hasTarget = false;
        agentStatus.targetLastSeen = false;
        agentStatus.hitRegistered = false;
        agentStatus.takingRiskyPath = false;
        agentStatus.breakBeingStuck = false;
        agentStatus.fleeing = false;
        agentStatus.searching = false;
        agent.GetComponent<FieldOfView>().targetDetected = false;
        agent.GetComponent<ProximityDetector>().playerDetected = false;
        agentStatus.lastVisibleTargetPosition = agentStartPosition;
        
        //Dirty workaround to stop the agent from following the previous path. 
        //For some reason the coroutine started by the same expression was not able to stop, so just call a new path request that instantly exits.
        //PathRequestManager.RequestPath(transform.position, agentStartPosition, agentStatus.OnPathFound);
        PathController agentPathController = agent.GetComponent<PathController>();
        agentPathController.RequestPath(transform.position, agentStartPosition, agentStatus.OnPathFound);

        //Player AI Movement
        PlayerAIMovement playerStatus = player.GetComponent<PlayerAIMovement>();
        playerStatus.hasTarget = false;
        playerStatus.takingRiskyPath = false;
        playerStatus.fleeing = false;
        //missing fov? not using for now but might later.

        PathController playerPathController = agent.GetComponent<PathController>();
        agentPathController.RequestPath(playerStartPosition, playerStartPosition, playerStatus.OnPathFound);

    }

    private void FixedUpdate() {
        tick++;

        if (doCountDown) {
            countdown -= Time.deltaTime;
            if (countdown < 0) {
                if (!gameEnded) {
                    gameEnded = true;
                    OnGameEnd.Invoke();
                }
                
                //LoadSurvey();
                //LoadEndScreen();
            }
            if (!silentCountDown) {
                timer.text = Mathf.RoundToInt(Mathf.Clamp(countdown, 0, timeLimit)) > 9 ?
                             Mathf.RoundToInt(Mathf.Clamp(countdown, 0, timeLimit)).ToString() :
                             "0" + Mathf.RoundToInt(Mathf.Clamp(countdown, 0, timeLimit)).ToString();
            }
        }
        if (doScore) {
            score = Mathf.Max(0, score);
            scoreBoard.text = score.ToString() + " SCORE";

        }

        //ADDED FOR ALGORITHM STUFF
        if (!player.GetComponent<PlayerController>().isAI)
        {
            foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(kcode))
                {
                    keyPresses.Add(kcode.ToString());
                }
            }
        }

        //TODO: There needs to be a way of checking idle time when the player is an AI...
        if (!Input.anyKey)
        {
            //Starts counting when no button is being pressed
            idleTime = idleTime + 1;
        }
        fullTime = fullTime + 1;
    }

    public void LoadLevel(string name) {
        tick = 0;
        // Moderator is a research assistant who has to hold down Ctrl for key commands to work.
        if (moderatorActive) {
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
                SceneManager.LoadScene(name);
            }
        }
        if (!moderatorActive || (moderatorActive && name == "Tutorial")) {
            SceneManager.LoadScene(name);
        }
    }

    //======================STATE MANAGEMENT============================
    /// <summary>
    /// Will return the current state by updating stateRep array.
    /// HACK: It follows a specific order - same as model, as it needs to have the same 
    /// elements in the same place. 
    /// DANGER: Careful with changing said order. 
    /// </summary>
    /// <returns></returns>
    public float[] GetCurrentState()
    {
        stateRep[0] = ((float)idleTime / (float)fullTime); //idleTime 
        stateRep[1] = score; //score 

        //AGENT RELATED
        if (!agent.Equals(null))
        {
            stateRep[2] = ((agent.transform.position - previousBotPosition).magnitude); //botDistanceTraveled
            previousBotPosition = agent.transform.position;

            stateRep[3] = agent.transform.position.x; //botPositionX
            stateRep[4] = agent.transform.position.y; //botPositionY
            stateRep[5] = agent.transform.eulerAngles.z; //botRotation
            stateRep[6] = agent.GetComponent<Movement>().speed; //botSpeed
            stateRep[7] = agent.GetComponent<Movement>().rotationSpeed; //botRotationSpeed
            stateRep[8] = agent.GetComponent<FieldOfView>().viewAngle; //botViewAngle
            stateRep[9] = agent.GetComponent<FieldOfView>().viewRadius; //botViewRadius
            stateRep[10] = agent.GetComponent<Movement>().isWaiting ? 1 : 0; //botSearching
            stateRep[11] = agent.GetComponent<Movement>().lookAroundCycle; //botSearchTurns
            stateRep[12] = agent.GetComponent<ProximityDetector>().hearingRadius; //botHearingRadius
            stateRep[13] = agent.GetComponent<ProximityDetector>().detectionChance; //botHearingProbability
            stateRep[14] = agent.GetComponent<Health>().health; //botHealth
            stateRep[15] = agent.GetComponent<FrustrationComponent>().levelOfFrustration; //botFrustration
            stateRep[16] = agent.GetComponent<Movement>().riskTakingFactor; //botRiskTakingFactor
            stateRep[17] = agent.GetComponent<Movement>().takingRiskyPath ? 1 : 0; //botTakingRiskyPath

            int targetSeen = (agent.GetComponent<ProximityDetector>().playerDetected || agent.GetComponent<FieldOfView>().targetDetected) ? 1 : 0;
            stateRep[18] = targetSeen; //botSeeingPlayer
            int botInChase = agent.GetComponent<Movement>().targetLastSeen ? 1 : 0;
            stateRep[19] = botInChase; //botChasingPlayer

            stateRep[46] = agent.GetComponent<Health>().isBurning ? 1 : 0; //botBurning
            stateRep[47] = agent.GetComponent<Health>().deltaHealth; //botDeltaHealth
            stateRep[48] = agent.GetComponent<Health>().botDied ? 1 : 0; //botDied

            float cursorDistanceFromBot = (new Vector2(cursor.transform.position.x, cursor.transform.position.y) -
                       new Vector2(agent.transform.position.x, agent.transform.position.y)).magnitude;
            stateRep[22] = cursorDistanceFromBot; //cursorDistanceFromBot

        }
        // AGENT AND PLAYER RELATED
        if (!agent.Equals(null) && !player.Equals(null))
        {
            float botDistanceFromPlayer = (new Vector2(agent.transform.position.x, agent.transform.position.y) -
                new Vector2(player.transform.position.x, player.transform.position.y)).magnitude;
            stateRep[20] = botDistanceFromPlayer; //botDistanceFromPlayer
        }

        //PLAYER RELATED
        if (!player.Equals(null))
        {
            stateRep[23] = ((player.transform.position - previousPlayerPosition).magnitude); //playerDistanceTravelled
            previousPlayerPosition = player.transform.position;

            stateRep[24] = player.transform.position.x; //playerPositionX
            stateRep[25] = player.transform.position.y; //playerPositionY
            stateRep[26] = player.transform.eulerAngles.z; //playerRotation
            stateRep[27] = player.GetComponent<PlayerHealth>().health; //playerHealth
            stateRep[28] = player.GetComponent<PlayerController>().dashing ? 1 : 0; //playerIsDashing
            stateRep[29] = player.GetComponent<PlayerController>().dashOnCD ? 1 : 0; //playerTriesDashOnCD

            float cursorDistanceFromPlayer = (new Vector2(cursor.transform.position.x, cursor.transform.position.y) -
               new Vector2(player.transform.position.x, player.transform.position.y)).magnitude;
            stateRep[21] = cursorDistanceFromPlayer; //cursorDistanceFromPlayer

            stateRep[40] = player.GetComponent<PlayerHealth>().isBurning ? 1 : 0; //playerBurning
            stateRep[41] = player.GetComponent<PlayerHealth>().isHealing ? 1 : 0; //playerHealing
            stateRep[42] = player.GetComponent<PlayerHealth>().deltaHealth; //playerDeltaHealth
            stateRep[43] = player.GetComponent<PlayerHealth>().playerDied ? 1 : 0; //playerDied
        }

        if (!player.GetComponent<PlayerController>().isAI)
        {
            //dashPressed
            stateRep[30] = ((Input.GetKeyDown(KeyCode.LeftShift) ||
                Input.GetKeyDown(KeyCode.RightShift) ||
                Input.GetKeyDown(KeyCode.Space)) ? 1 : 0);

            //playerTriesToFireOnCD
            stateRep[34] =(Input.GetMouseButton(0) &&
                GameObject.Find("GunControls").GetComponent<GunControls>().projectileCount == 0 ? 1 : 0); 
            
            //playerTriesToBombOnCD
            stateRep[35] = ((Input.GetMouseButtonUp(1) &&
            cursor.GetComponent<GunControls>().bombCount == 0 ? 1 : 0));
        }
        else
        {
            stateRep[30] = 0.0f;
            stateRep[34] = 0.0f; //playerTriesToFireOnCD
            stateRep[35] = 0.0f; //playerTriesToBombOnCD
        }
             
  
        
        stateRep[31] = (cursor.transform.position - previousCursorPosition).magnitude; //cursorDistanceTraveled
        previousCursorPosition = cursor.transform.position;


        stateRep[32] = cursor.transform.position.x; //cursorPositionX
        stateRep[33] = cursor.transform.position.y; //cursorPositionY


        stateRep[36] =
            !GameObject.Find("GunControls").GetComponent<GunControls>().reloading ? 1 : 0; //shotsFired

        stateRep[37] =
            !GameObject.Find("GunControls").GetComponent<GunControls>().bombReloading ? 1 : 0; //bombDropped

        stateRep[38] = 
            GameObject.Find("GunControls").GetComponent<GunControls>().reloading ? 1 : 0; //gunReloading

        stateRep[39] = 
            GameObject.Find("GunControls").GetComponent<GunControls>().bombReloading ? 1 : 0; //bombReloading

        stateRep[49] = GameObject.FindGameObjectsWithTag("fire").Length; //onScreenFires
        stateRep[50] = GameObject.FindGameObjectsWithTag("projectile").Length; //onScreenBullets


        stateRep[44] = agent.GetComponent<FrustrationComponent>().targetWasVisible &&
            !agent.GetComponent<Movement>().targetLastSeen ? 1 : 0; //botLostPlayer


        stateRep[45] = !agent.GetComponent<FrustrationComponent>().targetWasVisible &&
            agent.GetComponent<Movement>().targetLastSeen ? 1 : 0; //botSpottedPlayer


        stateRep[51] = keyPresses.Count; //keyPressCount
        stateRep[52] = timeLimit - countdown; //[general]timePassed
        stateRep[53] = keyPresses.Count; //[general]inputIntensity
        stateRep[54] = keyPresses.Distinct().Count(); //[general]inputDiversity
        stateRep[55] = 1 - ((float)idleTime / (float)fullTime); //[general]activity
        stateRep[56] = score; //[general]score

        return stateRep;
    }

    //======================TIME MANAGEMENT============================

    private static readonly DateTime Jan1st1970 = new DateTime
    (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static long CurrentTimeMillis()
    {
        return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
    }

    private long ReturnEpochStamp()
    {
        return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
    }
}