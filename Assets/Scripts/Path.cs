using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Algorithms;
using Assets.Scripts;

[ExecuteInEditMode]
public class Path : MonoBehaviour {
    private Waypoint _startWaypoint;
    private List<Waypoint> _midWaypoints;
    private Waypoint _endWaypoint;

    public Waypoint StartWaypoint
    {
        get { return _startWaypoint; }
    }
    public List<Waypoint> MidWaypoints
    {
        get { return _midWaypoints; }
    }
    public Waypoint EndWaypoint
    {
        get { return _endWaypoint; }
    }





// Use this for initialization
void Start () {

        var waypoints = GetComponentsInChildren<Waypoint>();
        _startWaypoint = waypoints.FirstOrDefault();
        _midWaypoints = waypoints.Skip(1).Take(waypoints.Length - 2).ToList();
        _endWaypoint = waypoints.LastOrDefault();

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
        if (Toolbox.Instance == null)
        {
            return;
        }

        var path = target as Path;

        DrawPath(path);
    }

    public static void DrawPath(Path path)
    {
        //if (this.GetInstanceID() == Selection.activeInstanceID)
        //{
        //    if (path.StartWaypoint != null && path.EndWaypoint != null)
        //    {
        //        Debug.DrawLine(path.StartWaypoint.transform.position, path.EndWaypoint.transform.position, Color.green, 0.1f);/
        //
        //    }
        //}

        var gameGrid = Toolbox.Instance.GameManager.GameGrid;
        if (gameGrid != null)
        {
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
        if (Toolbox.Instance == null)
        {
            return;
        }
        var wp = target as Waypoint;

        var path = wp.GetComponentInParent<Path>();

        if (path != null)
        {
            PathEditor.DrawPath(path);
        }
    }
}

