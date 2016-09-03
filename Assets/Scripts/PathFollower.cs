using System;
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

    public event EventHandler<EventArgs> Blocked;
    public event EventHandler<EventArgs> AtFinish;

    private List<PathFinderNode> _pathNodeList;

    public GameGrid.GameCell CurrentGameCell; // The one we are in.
    public GameGrid.GameCell PrevGameCell; // The one we are leaving.
    public GameGrid.GameCell NextGameCell; // The on we are going to.
    public GameGrid.GameCell TargetCell; // The one we are going to

    protected float _startTime;
    private DragSource _dt;
    private Entity _entity;

    // Use this for initialization
    public void Start()
    {
        // If dragging, pause the path.
        _dt = this.GetComponent<DragSource>();
        _entity = this.GetComponent<Entity>();

    }

    public List<PathFinderNode> Path
    {
        get
        {
            return _pathNodeList;
            
        }
        set { _pathNodeList = value; }
    } 


    public void FollowToTargetCell(GameGrid gameGrid, Vector3 currentPos /* May not be at center of a cell */)
    {
        StartPos = currentPos;

        PrevGameCell = CurrentGameCell;

        // Changed target, so find a new path.
        var ignorePath = gameGrid.FindPath(CurrentGameCell, TargetCell, this);
        if (ignorePath == null || ignorePath.Count == 0)
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
        NextGameCell = gameGrid.GetNextPathGameCell(PrevGameCell, this);


    }

    public Vector3? StartPos;


    void OnSceneGUI()
        {
        OnGUI();
        }

    void OnGUI()
    {
        if (_pathNodeList != null)
        {
            var gameGrid = Toolbox.Instance.GameManager.GameGrid;

            foreach (var node in _pathNodeList)
            {
                // Draw lines between the nodes, just to see.
                //Debug.DrawLine( );
                var nodePoint = new GridPoint(node.X, node.Y);
                if (Toolbox.Instance.DebugSys.ShowXOnPath)
                {
                    gameGrid.DrawTextAtPoint(nodePoint, "X");
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

        var gameGrid = Toolbox.Instance.GameManager.GameGrid;
        if (gameGrid != null)
        {
            var t = Time.time;
            var map = gameGrid.GetMap();

            // Refigure the path on each update (in case the path has changed).
            // TBD: We could optimize this, if we know there is nothing moving that can block things on path,
            //  and we're sure the target is still there ... more complex games might allow the user to drop
            /// walls to block paths.
            var path = gameGrid.FindPath(PrevGameCell, TargetCell, this);

            //var nextGameCell = FindNextGameCell( path, PrevGameCell);

            // If the path is no longer valid.
            //if( !IsPathStillValid(path, PrevGameCell, NextGameCell, this.transform.position))
            //{
            //    //DebugSystem.DebugAssert(false, "Need to handle case where next cell is blocked (no longer on path).");
            //}

            float deltaT = t - _startTime;
            float distCovered = deltaT*GetSpeedFromEntity();

            Debug.Assert(NextGameCell != null);
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

                    // TBD: There may have been a little time left, so we have to move further past current point.
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
