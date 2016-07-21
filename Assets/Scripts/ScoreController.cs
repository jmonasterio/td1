﻿using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices.ComTypes;

public class ScoreController : MonoBehaviour {

    public int Score;

    // How many enemies have
    public int EnemyScore;

    // calling them flags like "capture the flag".
    public void EnemyScored(int numberOfFlags)
    {
        this.EnemyScore -= numberOfFlags;
    }

    public void PlayerScored(int points)
    {
        Score += points;
    }

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
