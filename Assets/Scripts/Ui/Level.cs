﻿using UnityEngine;
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
        public Transform Level; // Self
        public Transform Paths;
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


        // Backgrounds

        
        // Robots
        var robotData = Toolbox.Instance.GameManager.DataController.RobotClasses;
        var robotLevelData = levelData.RotbotLevelData;
        var robotDatum = robotData.FirstOrDefault(_ => _.EntityId == robotLevelData.EntityId);
        InstantiateRobotFromData(robotDatum, robotLevelData); // Looks up correct prefab + sets some data.

        // Towers
        var towerData = Toolbox.Instance.GameManager.DataController.TowerClasses;
        foreach (var towerLevelData in levelData.TowerLevelData)
        {
            var towerDatum = towerData.FirstOrDefault(_ => _.EntityId == towerLevelData.EntityId);
            InstantiateTowerFromData(towerDatum, towerLevelData); // Looks up correct prefab + sets some data.
        }

        // Humans

        // TBD: Lookup the humans characteristic data.
        var humanData = Toolbox.Instance.GameManager.DataController.HumanClasses;

        foreach (var humanLevelData in levelData.HumanLevelData)
        {
            var humanDatum = humanData.FirstOrDefault(_ => _.EntityId == humanLevelData.EntityId);

            // Make human from
            InstantiateHumanFromData(humanDatum, humanLevelData); // Looks up correct prefab + sets some data.
        }

        // Paths
        foreach (var pathLevelData in levelData.PathLevelData)
        {
            InstantiatePathFromData(pathLevelData);
        }

        // Enemies.

        // Enemies get added by the wave controller.


        Toolbox.Instance.GameManager.LevelController.CurrentLevel.GameGrid.InitCellMapFromLevelEntities();

    }

    private void InstantiateTowerFromData(TowerPoco towerDatum, DataController.TowerLevelData towerLevelData)
    {
        // TBD: This prefab should be coming from the ATLAS?
        var prefab = Toolbox.Instance.GameManager.AtlasController.EmptyPrefabs.Tower;
        var tower = Instantiate<Tower>(prefab);
        tower.gameObject.name = "Tower" + towerDatum.EntityId;
        tower.transform.SetParent(Nodes.Towers);
        tower.transform.position = GameGrid.MapGridPointToVector(towerLevelData.BaseLevelData.GridPoint);
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
        human.transform.position = GameGrid.MapGridPointToVector(humanLevelData.BaseLevelData.GridPoint);
    }

    private void InstantiateRobotFromData(RobotPoco robotDatum, DataController.RobotLevelData robotLevelData)
    {
        // TBD: This prefab should be coming from the ATLAS?
        var prefab = Toolbox.Instance.GameManager.LevelController.EmptyRobotPrefab;
        switch (robotDatum.EntityId)
        {
            default:
                prefab = Toolbox.Instance.GameManager.LevelController.EmptyRobotPrefab;
                break;
        }
        var human = Instantiate<Robot>(prefab);
        human.gameObject.name = "Robot1";
        human.transform.SetParent(Nodes.Level);
        human.transform.position = GameGrid.MapGridPointToVector(robotLevelData.BaseLevelData.GridPoint);
    }

    private void InstantiatePathFromData(DataController.PathLevelData pathLevelData)
    {
        var emptyPrefabs = Toolbox.Instance.GameManager.AtlasController.EmptyPrefabs;
        
        var pathContainer = Instantiate<Path>(emptyPrefabs.PathContainer);
        pathContainer.name = pathLevelData.EntityId;
        pathContainer.transform.SetParent(Nodes.Paths);

        var path = pathContainer.GetComponent<Path>();

        var start = Instantiate<Waypoint>(emptyPrefabs.Start);
        start.transform.SetParent(Nodes.Paths);
        start.transform.SetParent(pathContainer.transform);
        start.transform.position = GameGrid.MapGridPointToVector(pathLevelData.Start);
        start.GetComponent<Waypoint>().WaypointType = Waypoint.WaypointTypes.Start;

        path.StartWaypoint = start;

        int idx = 0;
        var midpoints = new List<Waypoint>();
        foreach (var wp in pathLevelData.Midpoints)
        {
            var mid = Instantiate<Waypoint>(emptyPrefabs.Midpoint);
            mid.transform.SetParent(Nodes.Paths);
            mid.transform.SetParent(pathContainer.transform);
            mid.transform.position = GameGrid.MapGridPointToVector(wp);
            mid.GetComponent<Waypoint>().WaypointIndex = idx++;
            mid.GetComponent<Waypoint>().WaypointType = Waypoint.WaypointTypes.Waypoint;
            midpoints.Add(mid);
        }

        path.MidWaypoints = midpoints;

        var end = Instantiate<Waypoint>(emptyPrefabs.End);
        end.transform.SetParent(Nodes.Paths);
        end.transform.SetParent(pathContainer.transform);
        end.transform.position = GameGrid.MapGridPointToVector(pathLevelData.End);
        end.GetComponent<Waypoint>().WaypointType = Waypoint.WaypointTypes.End;

        path.EndWaypoint = end;

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
        _nodes.Level = this.transform;
        _nodes.Paths = this.transform.FindChild("Paths").transform;
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

    public List<Path> Paths()
    {
        return GetSceneObjectsInTranform<Path>(_nodes.Paths);
    }

    public List<Enemy> Enemies()
    {
        return GetSceneObjectsInTranform<Enemy>(_nodes.Enemies);
    }

}
