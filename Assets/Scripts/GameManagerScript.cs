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

    private static AudioPoolController AudioPoolController;


    // State

    // Use this for initialization (of me).
    void Awake()
    {
        Toolbox.Instance.GameManager = this;
        Toolbox.Instance.DebugSys = this.GetComponent<DebugSystem>();
        AudioPoolController = GetComponent<AudioPoolController>();
        //if (HideMouseCuror)
        {
            Cursor.visible = true;
        }
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

    public static void PlayClip(AudioClip clip)
    {
        //if (GameManager.Instance.PlayController.State == PlayController.States.Over)
        //{
        //    // No sound when not playing.
        //    return;
        //}
        AudioPoolController.PlayClip(clip);
    }

    /// <summary>
    /// TBD-JM: Next two methods are probably really slow.
    /// </summary>
    /// <returns></returns>
    public List<Tower> Towers()
    {
        return GameGrid.GetActiveObjectsInLayer(GameGrid.TOWER_LAYER).Cast<Tower>().ToList();
    }

    public List<Enemy> Enemies()
    {
        var enemiesCollection = GameObject.Find("Enemies"); // TBD: Maybe do this in the in the Enemy object.


        var list = new List<Enemy>();
        foreach (Transform en in enemiesCollection.transform)
        {
            list.Add( en.gameObject.GetComponent<Enemy>());
        }
        return list;
    }
}
