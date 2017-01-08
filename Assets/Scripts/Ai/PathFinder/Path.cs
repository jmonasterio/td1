using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Algorithms;
using Assets.Scripts;

//[ExecuteInEditMode]
public class Path : MonoBehaviour
{
    public Waypoint StartWaypoint { get; set; }
    public List<Waypoint> MidWaypoints { get; set; }
    public Waypoint EndWaypoint { get; set; }

    public Path()
    {
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

    }
}

