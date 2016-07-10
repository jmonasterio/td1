using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Algorithms;
using UnityEngine.Assertions;

public class GameManagerScript : MonoBehaviour {

    // Inspectors
    public string GameName;
    
    // State
  
    // Use this for initialization (of me).
    void Awake()
    {
        Toolbox.Instance.GameManager = this;
        Toolbox.Instance.DebugSys = this.GetComponent<DebugSystem>();
    }

    void Start()
    {
    }

    public GameGrid GameGrid {
        get
        {
            var ret = Toolbox.Instance.GetComponent<GameGrid>();
            return ret;

        }
    }



}
