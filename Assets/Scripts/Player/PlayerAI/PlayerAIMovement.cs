using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAIMovement : MonoBehaviour
{
    public LayerMask unwalkableMask;
    public LayerMask fireMask;
    public Transform target; /*-----ALSO ADDED BY ME-----*/
    public bool tutorialActive;

    public float speed = 3f; /*------CHANGED, AI WAS TOO FAST------*/
    public float rotationSpeed = 0.1f;
    public float dashMultiplier = 3f;
    public float dashCoolDown = 1f;
    public float dashDuration = 0.2f;

    [Range(1, 100)]
    public float riskAversion = 100;
    [HideInInspector]
    public Vector3[] path;
    [HideInInspector]
    public Vector3[] riskyPath;
    [HideInInspector]
    public Vector3[] safePath;
    int targetIndex;
    public bool displayPath;


    FieldOfView[] fov;
    PlayerHealth status;
    private GunControls playerGun;
    PathController pathController; //-----NEW----


    [HideInInspector]
    public bool hasTarget = false;

    /*----------THIS ADDED BY ME--------*/
    IndicatorVision vision;
    [HideInInspector]
    public bool monsterTooClose = false;


    [HideInInspector]
    public bool fleeing = false;

    Vector3 newTarget;
    Vector3 previousPosition = Vector3.zero;
    private float checkCooldown = 2f;
    private float checkTimeStamp = 0f;

    [HideInInspector]
    public int riskTakingFactor;
    [HideInInspector]
    public bool takingRiskyPath = false;

    int counter;

    void Awake()
    {
        fov = transform.GetComponents<FieldOfView>();
        status = transform.GetComponent<PlayerHealth>();
        vision = transform.GetComponent<IndicatorVision>();
        pathController = transform.GetComponent<PathController>(); //----New Script---

        target = GameObject.Find("Monster").transform;

        playerGun = GameObject.Find("GunControls").GetComponent<GunControls>();
        previousPosition = transform.position;
        //previousRotation = transform.eulerAngles.z;
        counter = 0;
    }

    private void FixedUpdate()
    {
        float monsterPlayerDistance = Vector2.Distance(target.transform.position, transform.position);
        monsterTooClose = monsterPlayerDistance <= 15.0f ? true : false;
    }

    // Update is called once per frame
    void Update()
    {

        if (checkTimeStamp <= Time.time)
        {
            checkTimeStamp = Time.time + checkCooldown;
            previousPosition = transform.position;
            //previousRotation = transform.eulerAngles.z;
        }

        //If it sees the monster and it isn't too close
        if (vision.agent.detected && !monsterTooClose)
        {
            //shoot = true;
            fleeing = false;
        }
        else if (!vision.agent.detected) //if it doesn't see the monster go to it
        {
            //shoot = false;
            fleeing = false;
            pathController.RequestPath(transform.position, target.position, OnPathFound);
        }
        else if (vision.agent.detected && monsterTooClose) //if it sees the monster but it is too close.
        {
            //shoot = true;
            fleeing = true;
            if(!hasTarget)
            {
                hasTarget = true;
                Debug.Log("(Player) Getting new target");
                GetNewTarget();
                previousPosition = transform.position;
            }
            pathController.RequestPath(transform.position, newTarget, OnPathFound);
        }

        //Correcting position z index
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    public void OnPathFound(Vector3[] newPath, Vector3[] newRiskyPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = FindBestPath(newPath, newRiskyPath);
            Debug.Log("(Player) Path found!");
            StopAllCoroutines();
            StartCoroutine("FollowThePath");
        }
    }

    Vector3[] FindBestPath(Vector3[] safePath, Vector3[] riskyPath)
    {
        if (riskyPath != null && displayPath)
        {
            for (int i = targetIndex; i < riskyPath.Length; i++)
            {
                if (i == targetIndex)
                {
                    Debug.DrawLine(transform.position, riskyPath[i], Color.red);
                }
                else
                {
                    Debug.DrawLine(riskyPath[i - 1], riskyPath[i], Color.red);
                }
            }
        }
        if (safePath != null && displayPath)
        {
            for (int i = targetIndex; i < safePath.Length; i++)
            {
                if (i == targetIndex)
                {
                    Debug.DrawLine(transform.position, safePath[i], Color.green);
                }
                else
                {
                    Debug.DrawLine(safePath[i - 1], safePath[i], Color.green);
                }
            }
        }
        riskTakingFactor = Mathf.RoundToInt(Mathf.Pow(2, riskAversion / 20)) + Mathf.Clamp(Mathf.Abs(100 - status.health) / 10, 0, 10);

        if (safePath.Length - riskyPath.Length > riskTakingFactor)
        {
            takingRiskyPath = true;
            return riskyPath;
        }
        else
        {
            takingRiskyPath = false;
            return safePath;
        }
    }

    IEnumerator FollowThePath()
    {
        counter++;
        Debug.Log(counter);
        Vector3 currentWaypoint;
        if (path.Length <= 0)
        {
            Debug.Log("(Player) Path Ends.");
            StopAllCoroutines();
            hasTarget = false;
            takingRiskyPath = false;
            fleeing = false;
            yield break;
        }

        while (true)
        {
            currentWaypoint = path[0];
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }

            Vector3 waypoint = new Vector3(currentWaypoint.x, currentWaypoint.y, transform.position.z);

            Quaternion rot = Quaternion.LookRotation(waypoint - transform.position, Vector3.back);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, rotationSpeed * 2);
            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z);
            transform.position = Vector3.MoveTowards(transform.position, waypoint, speed * Time.deltaTime);
            //Correcting position z index
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
            yield return null;
        }
    }


    // This function gets a random point from the circumference of a sector twice the size of the given angle
    Vector3 RandomPointAtDistance(float radius, float angle)
    {
        float randomAngle = Random.Range(transform.eulerAngles.z + 90 - angle, transform.eulerAngles.z + 90 + angle) * (Mathf.PI / 180);

        float x = Mathf.Clamp(transform.position.x + radius * Mathf.Cos(randomAngle), -18, 18);
        float y = Mathf.Clamp(transform.position.y + radius * Mathf.Sin(randomAngle), -13, 13);

        return new Vector3(x, y, transform.position.z);
    }

    // Builds a stack of random points
    Vector3 GetRandomPoint(float radius, float angle)
    {
        Stack<Vector3> PointStack = new Stack<Vector3>();
        for (int i = 0; i < 50; i++)
        {
            PointStack.Push(RandomPointAtDistance(radius, angle));
        }
        for (int j = 0; j < 50; j++)
        {
            PointStack.Push(RandomPointAtDistance(radius, angle + 180));
        }
        Vector3 randomPoint = PointStack.Pop();

        for (int stackIndex = 0; stackIndex < PointStack.Count; stackIndex++)
        {
            if (PointIsFree(randomPoint))
            {
                return randomPoint;
            }
            else
            {
                randomPoint = PointStack.Pop();
            }
        }
        return previousPosition;
    }

    // Checks if a given point is free
    bool PointIsFree(Vector3 point)
    {
        return !(Physics.CheckSphere(point, 1.5f, unwalkableMask));
    }

    // Gets new target
    Vector3 GetNewTarget()
    {
        Vector3 randomPosition = GetRandomPoint(fov[0].viewRadius, fov[0].viewAngle);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, randomPosition, out hit, fov[0].viewRadius, unwalkableMask))
        {
            Vector3 distance = new Vector3(hit.point.x, hit.point.y, transform.position.z) - transform.position;
            newTarget = transform.position + (distance.normalized * (distance.magnitude - 2f));
            return newTarget;
        }
        else
        {
            newTarget = randomPosition;
            return newTarget;
        }
    }

  
}
