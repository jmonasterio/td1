using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Algorithms;
using UnityEngine.Assertions;

public class GameManagerScript : MonoBehaviour
{

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

    public GameGrid GameGrid
    {
        get
        {
            var ret = Toolbox.Instance.GameManager.GetComponent<GameGrid>();
            return ret;

        }
    }

    /// <summary>
    /// TBD-JM: Next two methods are probably really slow.
    /// </summary>
    /// <returns></returns>
    public List<Tower> Towers()
    {
        return GameGrid.GetObjectsInLayer(GameGrid.TOWER_LAYER).Cast<Tower>().ToList();
    }

    public List<Enemy> Enemies()
    {
        var list = new List<Enemy>();
        foreach (var en in GameGrid.GetObjectsInLayer(GameGrid.ENEMY_LAYER))
        {
            list.Add((Enemy) en.GetComponent<Enemy>());
        }
        return list;
    }
}
