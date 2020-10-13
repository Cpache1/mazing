﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.Networking;
using System.Runtime.InteropServices;

public class ExperimentManager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void InitRecord();
    [DllImport("__Internal")]
    private static extern void StartRecord();
    [DllImport("__Internal")]
    private static extern void StopRecord();

    public Text endCountdown;
    public int endCountdownTimer;
    public string nextLevel;
    public bool lastLevel;
    public bool recordLevel;
    private LevelManager levelManager;

    private void Awake() {
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        if (recordLevel) {
            InitRecord();
        }

        levelManager.OnGameEnd.AddListener(EndLevel);
        if (recordLevel) {
            levelManager.OnGameStart.AddListener(StartRecord);
        }
    }

    private void EndLevel() {
        endCountdown.gameObject.SetActive(true);
        StartCoroutine(EndLevelProcess());
    }

    /// <summary>
    /// Starts the game shutdown coroutine.
    /// </summary>
    /// <returns>yield enumerator</returns>
    public IEnumerator EndLevelProcess() {     
        GameObject.FindWithTag("Player").SetActive(false);
        yield return new WaitForSeconds(0.5f);
        StopRecord();
    }
}
