using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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


    void OnGUI()
    {
        if (this.GetInstanceID() == Selection.activeInstanceID)
        {
            Debug.LogWarning("in match.");
        }
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
