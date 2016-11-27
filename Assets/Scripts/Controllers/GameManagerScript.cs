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
    public AtlasController AtlasController;
    public LevelController LevelController;

    public struct TreeNodes
    {
        public Transform BulletsCollection;
        public Transform Enemies;
        public Transform Towers;
        public Transform Humans;
    }


    // State
    private TreeNodes _nodes;

    public TreeNodes Nodes
    {
        get { return _nodes; }
    }

    // Use this for initialization (of me).
    void Awake()
    {
        // Need to be in AWAKE because other components refer to this.
        Toolbox.Instance.GameManager = this;
        Toolbox.Instance.DebugSys = this.GetComponent<DebugSystem>();
        AudioPoolController = GetComponent<AudioPoolController>();
        WavesController = GetComponent<WavesController>();
        ScoreController = GetComponent<ScoreController>();
        DataController = GetComponent<DataController>();
        GameController = GetComponent<GameController>();
        AtlasController = GetComponent<AtlasController>();
        LevelController = GetComponent<LevelController>();
    }

    public void RebuildTreeNodes()
    {
        _nodes = new TreeNodes();
        _nodes.BulletsCollection = GameObject.Find("Bullets").transform;
        _nodes.Enemies = GameObject.Find("Enemies").transform;
        _nodes.Towers = GameObject.Find("Towers").transform;
        _nodes.Humans = GameObject.Find("Humans").transform;
    }

    public void Start()
    {
        RebuildTreeNodes();

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
        return GameGrid.GetSceneObjectsInTranform<Tower>(_nodes.Towers);
    }

    public List<Human> Humans()
    {
        return GameGrid.GetSceneObjectsInTranform<Human>(_nodes.Humans);
    }

    public List<Enemy> Enemies()
    {
        return GameGrid.GetSceneObjectsInTranform<Enemy>(_nodes.Enemies);
    }
}
