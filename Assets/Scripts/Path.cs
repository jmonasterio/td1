using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Algorithms;
using Assets.Scripts;

[ExecuteInEditMode]
public class Path : MonoBehaviour {
    public Waypoint StartWaypoint { get; private set; }
    public List<Waypoint> MidWaypoints { get; private set; }
    public Waypoint EndWaypoint { get; private set; }

    // Use this for initialization
    void Start ()
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
    void Update () {
	
	}
}

/// Draw PATH in editor MODE when focus is on path
[CustomEditor(typeof(Path))]
public class PathEditor : Editor
{
    void OnSceneGUI()
    {
        var path = target as Path;
        path.UpdateWaypoints();
        DrawPath(path);
    }

    public static void DrawPath(Path path)
    {
        var gm = FindObjectOfType(typeof(GameManagerScript)) as GameManagerScript;
        if (gm == null)
        {
            return;
        }
        var gg = gm.GetComponent<GameGrid>();
        if (gg == null)
        {
            return;
        }

        gg.InitCellMapFromLevelMap( GameGrid.FindActiveMap().gameObject);

        //if (this.GetInstanceID() == Selection.activeInstanceID)
        //{
        //    if (path.StartWaypoint != null && path.EndWaypoint != null)
        //    {
        //        Debug.DrawLine(path.StartWaypoint.transform.position, path.EndWaypoint.transform.position, Color.green, 0.1f);/
        //
        //    }
        //}

        var gameGrid = gg;
        if (path.StartWaypoint != null && path.EndWaypoint != null)
        {
            var startCell = gameGrid.MapGridPointToGameCellOrNull(path.StartWaypoint.GridPoint);
            var midCells = GetCells(gameGrid, path.MidWaypoints);
            var endCell = gameGrid.MapGridPointToGameCellOrNull(path.EndWaypoint.GridPoint);

            var foundPath = gameGrid.FindPathWithWaypoints(startCell, midCells, endCell);
            if (foundPath != null)
            {
                foreach (var node in foundPath)
                {
                    // Draw lines between the nodes, just to see.
                    //Debug.DrawLine( );
                    var nodePoint = node.GridPoint;
                    gameGrid.DrawTextAtPoint(nodePoint, "X");
                }
            }
        }

    }

    private static List<GameGrid.GameCell> GetCells(GameGrid gameGrid, List<Waypoint> midWaypoints)
    {
        var ret = new List<GameGrid.GameCell>();

        foreach (var waypoint in midWaypoints)
        {
            ret.Add(gameGrid.MapGridPointToGameCellOrNull(waypoint.GridPoint));
        }

        return ret;
    }



}

/// Draw PATH in editor MODE when focus is on waypoint
[CustomEditor(typeof(Waypoint))]
public class WaypointEditor : Editor
{
    void OnSceneGUI()
    {
        var wp = target as Waypoint;

        var path = wp.GetComponentInParent<Path>();

        if (path != null)
        {
            PathEditor.DrawPath(path);
        }
    }
}

