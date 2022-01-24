using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PathController : MonoBehaviour
{
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    AStar_Pathfinding pathfinding;

    bool isProcessingPath;

    // Start is called before the first frame update
    void Awake()
    {
        pathfinding = GameObject.FindGameObjectWithTag("a-star").GetComponent<AStar_Pathfinding>();
    }

    public void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], Vector3[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        pathRequestQueue.Enqueue(newRequest);
        TryProcessNext();
    }

    void TryProcessNext()
    {
        Debug.Log("The number of paths are: " + pathRequestQueue.Count);
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd, this);
        }
    }

    public void FinishedProcessingPath(Vector3[] path, Vector3[] riskyPath, bool success)
    {
        currentPathRequest.callback(path, riskyPath, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], Vector3[], bool> callback;

        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], Vector3[], bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }
    }
}
