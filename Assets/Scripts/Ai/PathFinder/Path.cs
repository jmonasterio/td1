using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Algorithms;
using Assets.Scripts;
using UnityEditorInternal;

//[ExecuteInEditMode]
public class Path : MonoBehaviour
{
    private GameObject _debugPath;
    public Waypoint StartWaypoint { get; set; }
    public List<Waypoint> MidWaypoints { get; set; }
    public Waypoint EndWaypoint { get; set; }

    public Path()
    {
    }

    void CreateDebugPathContainer()
    {
        var go2 = new GameObject("DebugPathContainer");
        go2.transform.SetParent(this.transform);
        go2.transform.localPosition = new Vector3(0, 1, -2);
        go2.transform.localScale = new Vector3(5, 5, 5);
        _debugPath = go2;
    }

    void DeleteDebugPathContainer()
    {
        if (_debugPath != null)
        {
            UnityEngine.Object.Destroy(_debugPath);
            _debugPath = null;
        }
    }


    // Use this for initialization
    void Start()
    {

        UpdateWaypoints();

    }

    public void UpdateWaypoints()
    {
        var waypoints = GetComponentsInChildren<Waypoint>();
        StartWaypoint = waypoints.FirstOrDefault();
        MidWaypoints = waypoints.Skip(1).Take(waypoints.Length - 2).ToList();
        EndWaypoint = waypoints.LastOrDefault();
    }

    // Update is called once per frame
    void Update()
    {
        if (Toolbox.Instance.DebugSys.ShowXOnPath)
        {
            DeleteDebugPathContainer();
            CreateDebugPathContainer();
            var gameGrid = Toolbox.Instance.GameManager.LevelController.CurrentLevel.GameGrid;
            //var path = gameGrid.FindPathWithWaypointsOrNull(StartWaypoint.GridPoint, MidWaypoints, EndWaypoint);

            // TTBD: Need to find path, and add a cell object PathDebug"X" at each point along the path.

            var go = new GameObject("PathDebugX");
            go.transform.SetParent(_debugPath.transform);
            var sprite = go.AddComponent<SpriteRenderer>();
            sprite.sprite = Toolbox.Instance.GameManager.AtlasController.DebugX;
            sprite.transform.localPosition = new Vector3(0,0,-1); // relative (above)
            sprite.transform.localScale = new Vector3(1,1,1);
            sprite.sortingLayerName = "Active"; // or won't draw. 
        }
        else
        {
            // Delete all X childs, so we don't see path.
            DeleteDebugPathContainer();
        }
    }
}

