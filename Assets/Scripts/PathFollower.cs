using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Algorithms;

public class PathFollower : MonoBehaviour {

    public GameGrid.GridDirections GridDirection;
    public float Speed = 1.0f;

    public GameGrid.GameCell CurrentGameCell; // The one we are in.
    public GameGrid.GameCell PrevGameCell; // The one we are leaving.
    public GameGrid.GameCell NextGameCell; // The on we are going to.
    public GameGrid.GameCell TargetCell; // The one we are going to

    private GridPoint? _target = null;
    private float _startTime;

    // Use this for initialization
    public void Start()
    {
    }

    public void StartFollowing( GameGrid gameGrid, GridPoint target)
    {
        GridDirection = GameGrid.GridDirections.Forward;

        PrevGameCell = gameGrid.GetStartGameCell();

        NextGameCell = gameGrid.GetNextPathGameCell( PrevGameCell, GridDirection);

        TargetCell = gameGrid.GetEndGameCell();

    }

    // Update is called once per frame
    public void Update()
    {
        var gameGrid = Toolbox.Instance.GameManager.GameGrid;
        if (gameGrid != null)
        {
            if (!_target.HasValue)
            {
                _target = gameGrid.GetEndGameCell().GridPoint;
                StartFollowing(gameGrid, _target.Value);
                _startTime = Time.time;
            }


            var map = gameGrid.Map;

            // Refigure the path on each update (in case the path has changed).
            // TBD: We could optimize this, if we know there is nothing moving that can block things on path,
            //  and we're sure the target is still there ... more complex games might allow the user to drop
            /// walls to block paths.
            var path = gameGrid.FindPath(PrevGameCell, TargetCell);

            //var nextGameCell = FindNextGameCell( path, PrevGameCell);

            // If the path is no longer valid.
            //if( !IsPathStillValid(path, PrevGameCell, NextGameCell, this.transform.position))
            //{
            //    //DebugSystem.DebugAssert(false, "Need to handle case where next cell is blocked (no longer on path).");
            //}

            var t = Time.time;
            float deltaT = t - _startTime;
            float distCovered = deltaT * Speed;

            var nextPathVector = GridHelper.MapPointToVector(map, NextGameCell.GridPoint);
            var prevPathVector = GridHelper.MapPointToVector(map, PrevGameCell.GridPoint);
            float fracJourney = distCovered/(nextPathVector - prevPathVector).magnitude;

            if (fracJourney <= 1.0f)
            {
                this.transform.position = Vector3.Lerp(prevPathVector, nextPathVector, fracJourney);
            }
            else if (fracJourney < 2.0f)
            {
                CurrentGameCell = NextGameCell;
                // We moved past the point, so go on to the next point.
                if (NextGameCell.GridPoint == TargetCell.GridPoint)
                {
                    GridDirection = GameGrid.Reverse(GridDirection);
                    if (GridDirection == GameGrid.GridDirections.Back)
                    {
                        gameGrid.RandomizeStartCell();
                        TargetCell = gameGrid.GetStartGameCell();
                    }
                    else
                    {
                        TargetCell = gameGrid.GetEndGameCell();
                    }

                    // Changed target, so find a new path.
                    path = gameGrid.FindPath(CurrentGameCell, TargetCell);
                }
                PrevGameCell = CurrentGameCell;
                NextGameCell = gameGrid.GetNextPathGameCell(CurrentGameCell, GridDirection);


                // TBD: There may have been a little time left, so we have to move further past current point.
                _startTime = t;

            }
            else
            {
                // Why would we ever need to go this far, unless we missed aframe?
                DebugSystem.DebugAssert(false, "moved way to far");
                _startTime = t;
            }

        }

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
            int a = 0;
            return false;
        }

    }
}
