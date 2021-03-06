﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Algorithms;
using Assets.Scripts;
using Object = UnityEngine.Object;

public class PathFollower : MonoBehaviour
{
    public Vector3? StartPos;
    public List<GameGrid.GameCell> CurrentPath;

    public event EventHandler<EventArgs> Blocked;
    public event EventHandler<EventArgs> AtFinish;

    public GameGrid.GameCell StartCell; // The one we orginally stared from
    public List<GameGrid.GameCell> OrderedWaypointCells;
    public GameGrid.GameCell CurrentGameCell; // The one we are in.
    public GameGrid.GameCell PrevGameCell; // The one we are leaving.
    public GameGrid.GameCell NextGameCell; // The on we are going to.
    public GameGrid.GameCell TargetCell; // The one we are going to

    protected float _startTime;
    private DragSource _dt;
    private Entity _entity;

    // Use this for initialization
    void Start()
    {
        // If dragging, pause the path.
        _dt = this.GetComponent<DragSource>();
        _entity = this.GetComponent<Entity>();

    }

    public void SetPathWaypoints(Path path)
    {
        var gameGrid = Toolbox.Instance.GameManager.LevelController.CurrentLevel.GameGrid;

        PrevGameCell = gameGrid.MapGridPointToGameCellOrNull(path.StartWaypoint.GridPoint);
        CurrentGameCell = PrevGameCell;
        OrderedWaypointCells = GetCells(gameGrid, path.MidWaypoints);
        //gameGrid.RandomizeEndCell();
        TargetCell = gameGrid.MapGridPointToGameCellOrNull(path.EndWaypoint.GridPoint);
        FollowToTargetCell(gameGrid, transform.position);
        _startTime = Time.time;
    }

    private List<GameGrid.GameCell> GetCells(GameGrid gameGrid, List<Waypoint> midWaypoints)
    {
        var ret = new List<GameGrid.GameCell>();

        foreach (var waypoint in midWaypoints)
        {
            ret.Add(gameGrid.MapGridPointToGameCellOrNull(waypoint.GridPoint));
        }

        return ret;
    }




    public void FollowToTargetCell(GameGrid gameGrid, Vector3 currentPos /* May not be at center of a cell */)
    {
        StartPos = currentPos;

        PrevGameCell = CurrentGameCell;

        if (CurrentGameCell == null)
        {
            return;
        }

        if ((OrderedWaypointCells.Count > 0) && (CurrentGameCell.GridPoint == OrderedWaypointCells[0].GridPoint))
        {
            // remove each waypoint as we reach it. The idea is that the path may get changed (due to blockages) from here on out, but we won't go back to this particular waypoint once we reach it.
            // of course, we don't want to remove the last waypoint, either. That is the endpoint (for now).
            OrderedWaypointCells.RemoveAt(0);
        }

        // Changed target, so find a new path.
        if( CurrentPath == null || DetourRequired( OrderedWaypointCells))
        {
            CurrentPath = gameGrid.FindPathWithWaypointsOrNull(CurrentGameCell, OrderedWaypointCells, TargetCell);
        }
        if (CurrentPath == null || CurrentPath.Count == 0)
        {
            // Couldn't find a path!!!!
            //ignorePath = gameGrid.FindPath(CurrentGameCell, TargetCell, this);
            //Debug.Assert(false, "Couldn't find a path! Happens randomly. What???");
            if (Blocked != null)
            {
                Blocked(this, new EventArgs());
            }
            return;
        }
        NextGameCell = GetNextPathGameCell(CurrentGameCell, CurrentPath);


    }


    private static GameGrid.GameCell GetNextPathGameCell(GameGrid.GameCell curGameCell, List<GameGrid.GameCell> currentPath)
    {
        var path = currentPath;
        if (path == null)
        {
            return null;
        }

        var curGridPoint = curGameCell.GridPoint;

        for (int ii = 0; ii < path.Count; ii++)
        {
            var cel = currentPath[ii];
            if (cel.GridPoint == curGridPoint)
            {
                GameGrid.GameCell node;
                if (ii < 1)
                {
                    return null;
                }
                node = currentPath[ii - 1];
                return node;
            }
        }
        return null;

    }




    void OnSceneGUI()
        {
        OnGUI();
        }

    void OnGUI()
    {
        return; // ONGUITEST

        // ******** BAD IDEA BECAUSE DREW X'S FOR EVERY SINGLE ENTITY THAT WAS A PATHFOLLOWER.

        if (Event.current.type != EventType.Repaint)
        {
            return;
        }

        if (CurrentPath != null)
        {
            var gameGrid = Toolbox.Instance.GameManager.LevelController.CurrentLevel.GameGrid;

            foreach (var node in CurrentPath)
            {
                // Draw lines between the nodes, just to see.
                //Debug.DrawLine( );
                var nodePoint = node.GridPoint;
                if (Toolbox.Instance.DebugSys.ShowXOnPath)
                {
                    gameGrid.DrawTextAtGridPoint(nodePoint, "X", Color.black);
                }
            }
        }
    }


    // Update is called once per frame
    public void Update()
    {
        if (this.gameObject == null)
        {
            return; // We are already destroyed. So weird.
        }

        if (_dt != null && _dt.Dragging)
        {
            return;
        }

        if (this.TargetCell == null || this.NextGameCell == null || this.PrevGameCell == null)
        {
            // No target. Nothing to do.
            return;
        }

        var gameGrid = Toolbox.Instance.GameManager.LevelController.CurrentLevel.GameGrid;
        if (gameGrid != null)
        {
            // Refigure the path on each update (in case the path has changed).
            // TBD: We could optimize this, if we know there is nothing moving that can block things on path,
            //  and we're sure the target is still there ... more complex games might allow the user to drop
            /// walls to block paths.
            /// 
            /// 
            /// Alternate optimization: Follow path until blocked. Only then calculate a new
            ///  path;

            if (CurrentPath == null || DetourRequired(OrderedWaypointCells))
            {
                // Find a new path.
                CurrentPath = gameGrid.FindPathWithWaypointsOrNull(PrevGameCell, OrderedWaypointCells, TargetCell);
            }

            //var nextGameCell = FindNextGameCell( path, PrevGameCell);

            // If the path is no longer valid.
            //if( !IsPathStillValid(path, PrevGameCell, NextGameCell, this.transform.position))
            //{
            //    //DebugSystem.DebugAssert(false, "Need to handle case where next cell is blocked (no longer on path).");
            //}

            var t = Time.time;
            float deltaT = t - _startTime;
            float distCovered = deltaT*GetSpeedFromEntity();

            Debug.Assert(NextGameCell != null);
            var map = gameGrid.GetMap();
            var nextPathVector = GridHelper.MapPointToVector(map, NextGameCell.GridPoint);
            var prevPathVector = GridHelper.MapPointToVector(map, PrevGameCell.GridPoint);
            if (StartPos.HasValue)
            {
                // At the very beginning, we may start off in a cell, but may not be center of cell.
                prevPathVector = StartPos.Value;
            }

            float fracJourney = distCovered/(nextPathVector - prevPathVector).magnitude;

            if (fracJourney <= 1.0f)
            {
                this.transform.position = Vector3.Lerp(prevPathVector, nextPathVector, fracJourney);
            }
            else if (fracJourney < 2.0f)
            {
                StartPos = null;
                CurrentGameCell = NextGameCell;

                // We moved past the point, so go on to the next point.
                if (CurrentGameCell.GridPoint == TargetCell.GridPoint)
                {
                    if (AtFinish != null)
                    {
                        AtFinish(this, new EventArgs());
                        _startTime = t;
                    }
                }
                else
                {

                    // There may have been a little time left, so we have to move further past current point.
                    _startTime = t;
                    FollowToTargetCell(gameGrid, transform.position);
                }
            }
            else
            {
                // Why would we ever need to go this far, unless we missed aframe?
                StartPos = null;
                DebugSystem.DebugAssert(false, "moved way to far");
                _startTime = t;
                FollowToTargetCell(gameGrid, transform.position);
            }
        }
    }

    private bool DetourRequired(List<GameGrid.GameCell> orderedWaypointCells)
    {
        foreach (var wpc in orderedWaypointCells)
        {
            if (wpc.Background != null)
            {
                return true;
            }
        }
        return false;
    }

    private float GetSpeedFromEntity()
    {
        if (_entity != null)
        {
            return _entity.Speed;
        }
        return 0.0f;
    }


    // Path has changed a lot and we're no longer on the way to a cell on the path.
    private bool IsPathStillValid(List<GameGrid.GameCell> path, GameGrid.GameCell prevGameCell,
        GameGrid.GameCell nextGameCell, Vector3 position)
    {
        var targetcell = path.Find(_ => _.GridPoint == nextGameCell.GridPoint);
        if (targetcell != null)
        {
            return targetcell.IsBlocked();
        }
        else
        {
            return false;
        }

    }
}
