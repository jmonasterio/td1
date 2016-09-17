using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Algorithms;
using Assets.Scripts;
using UnityEngine.Assertions;

public class GameManagerScript : MonoBehaviour
{

    // Inspectors
    public string GameName;

    private static AudioPoolController AudioPoolController;
    public WavesController WavesController;
    public ScoreController ScoreController;
    public DataController DataController;
    public GameController GameController;


    // State

    // Use this for initialization (of me).
    void Awake()
    {
        // Need to be in AWAKE because other components refer to this.
        Toolbox.Instance.GameManager = this;
        Toolbox.Instance.DebugSys = this.GetComponent<DebugSystem>();
    }

    public void Start()
    {
        AudioPoolController = GetComponent<AudioPoolController>();
        WavesController = GetComponent<WavesController>();
        ScoreController = GetComponent<ScoreController>();
        DataController = GetComponent<DataController>();
        GameController = GetComponent<GameController>();

        //if (HideMouseCuror)
        {
            Cursor.visible = true;
        }
    }



    public GameGrid GameGrid
    {
        get
        {
            var ret = Toolbox.Instance.GameManager.GetComponent<GameGrid>();
            return ret;

        }
    }

    public static void PlayClip(AudioClip clip)
    {
        //if (GameManager.Instance.PlayController.State == PlayController.States.Over)
        //{
        //    // No sound when not playing.
        //    return;
        //}
        if (AudioPoolController == null)
        {
            Debug.LogWarning("No audio pool controller.");
            return;
        }
        AudioPoolController.PlayClip(clip);
    }

    /// <summary>
    /// TBD-JM: Next two methods are probably really slow.
    /// </summary>
    /// <returns></returns>
    public List<Tower> Towers()
    {
        var gos = GameGrid.GetActiveObjectsInLayer(GameGrid.TOWER_LAYER);
        var ret = new List<Tower>();
        foreach (var go in gos)
        {
            ret.Add(go.GetComponent<Tower>());
        }
        return ret;
    }

    public List<Human> Humans()
    {
        var gos = GameGrid.GetActiveObjectsInLayer(GameGrid.HUMAN_LAYER);
        var ret = new List<Human>();
        foreach (var go in gos)
        {
            ret.Add(go.GetComponent<Human>());
        }
        return ret;
    }

    public List<Enemy> Enemies()
    {
        var enemiesCollection = GameObject.Find("Enemies"); // TBD: Maybe do this in the in the Enemy object.


        var list = new List<Enemy>();
        foreach (Transform en in enemiesCollection.transform)
        {
            var enemy = en.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                list.Add(enemy);
            }
            else
            {
                // Maybe an explosion or something.
            }
             
        }
        return list;
    }
}
