using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Algorithms;
using Assets.Scripts;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class Level : MonoBehaviour
{
    public WavesController WavesController;
    public LevelController.Levels LevelId;

    public struct TreeNodes
    {
        public Transform Bullets;
        public Transform Enemies;
        public Transform Towers;
        public Transform Humans;
        public Transform Map;
    }


    // State
    private TreeNodes _nodes;

    public TreeNodes Nodes
    {
        get { return _nodes; }
    }

    [Multiline]
    [TextArea(3,10)]
    public string LevelNotes;


    void Awake()
    {
        // Need to be in AWAKE because other components refer to this.
        WavesController = GetComponent<WavesController>();
    }

    // Use this for initialization
    void Start ()
    {

        RebuildTreeNodes();
        PopulateLevel();
    }

    private void PopulateLevel()
    {
        var levelData = Toolbox.Instance.GameManager.DataController.LoadLevelData(LevelId);

        // All this needs to come from data files.

        // Paths
        // Backgrounds
        // Robots
        // Towers
        // Humans
        
        // TBD: Lookup the humans characteristic data.
        var humanData = Toolbox.Instance.GameManager.DataController.HumanClasses;

        foreach (var humanLevelData in levelData.HumanLevelData)
        {
            var humanDatum = humanData.FirstOrDefault(_ => _.EntityId == humanLevelData.EntityId);

            // Make human from
            InstantiateHumanFromData(humanDatum, humanLevelData); // Looks up correct prefab + sets some data.
        }


        // Enemies
    }

    private void InstantiateHumanFromData(HumanPoco humanDatum, DataController.HumanLevelData humanLevelData)
    {
        // TBD: This prefab should be coming from the ATLAS?
        var prefab = Toolbox.Instance.GameManager.LevelController.EmptyHumanPrefab;
        switch (humanDatum.EntityId)
        {
            default:
                prefab = Toolbox.Instance.GameManager.LevelController.EmptyHumanPrefab;
                break;
        }
        var human = Instantiate<Human>(prefab);
        human.gameObject.name = "Human1";
        human.transform.SetParent(Nodes.Humans);
        human.transform.position = GameGrid.MapGridPointToVector(new GridPoint(humanLevelData.BaseLevelData.GridX, humanLevelData.BaseLevelData.GridY));
    }


    // Update is called once per frame
    void Update () {
	
	}

    // So we see in EDITOR, too.
    void OnSceneGUI()
        {
        OnGUI();
        }


    // Update is called once per frame
    void OnGUI()
        {
        var color = GUI.color;
        GUI.color = Color.green;

        GUIStyle myStyle = new GUIStyle(GUI.skin.GetStyle("label"));
        myStyle.fontSize = 32;

#if UNITY_EDITOR
        Handles.Label(this.transform.position + new Vector3(-5.0f, +10.0f, 0f), "" + Toolbox.Instance.GameManager.LevelController.ActiveLevelId.ToString(), myStyle);
#endif
        GUI.color = color;
        }

    private static List<T> GetSceneObjectsInTranform<T>(Transform t) where T : UnityEngine.Object
    {
        var ret = new List<T>();
        if (t == null)
        {
            return ret;
        }
        foreach (Transform x in t)
        {
            if (t.hideFlags == HideFlags.None)
            {
                var ent = x.gameObject.GetComponent<T>();

                if (ent != null)
                {
                    ret.Add(ent);
                }
            }
        }
        return ret;
    }




    public void RebuildTreeNodes()
    {
        _nodes = new TreeNodes();
        _nodes.Bullets = this.transform.FindChild("Bullets").transform;
        _nodes.Enemies = this.transform.FindChild("Enemies").transform;
        _nodes.Towers = this.transform.FindChild("Towers").transform;
        _nodes.Humans = this.transform.FindChild("Humans").transform;
        _nodes.Map = this.transform.FindChild("Map").transform;
    }

    public GameGrid GameGrid
    {
        get
        {
            var ret = this.GetComponent<GameGrid>();
            return ret;

        }
    }



    /// <summary>
    /// TBD-JM: Next two methods are probably really slow.
    /// </summary>
    /// <returns></returns>
    public List<Tower> Towers()
    {
        return GetSceneObjectsInTranform<Tower>(_nodes.Towers);
    }

    public List<Human> Humans()
    {
        return GetSceneObjectsInTranform<Human>(_nodes.Humans);
    }

    public List<Enemy> Enemies()
    {
        return GetSceneObjectsInTranform<Enemy>(_nodes.Enemies);
    }

}
