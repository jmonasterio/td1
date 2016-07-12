using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Algorithms;

public class PathFollower : MonoBehaviour {

    public GameGrid.Directions Direction;
    public float Speed = 1.0f;

    public GridPoint NextGridPoint;
    public GridPoint PrevGridPoint;

    private float _startTime;


    // Use this for initialization
    void Start () {
	}

    public void StartFollowing( GameGrid gameGrid)
    {
        Direction = GameGrid.Directions.Forward;

        PrevGridPoint = gameGrid.GetStartGameCell().GridPoint;

        NextGridPoint = gameGrid.GetNextPathGameCell( PrevGridPoint, Direction).GridPoint;

        _startTime = Time.time;

    }

    // Update is called once per frame
    void Update()
    {


        var gameGrid = Toolbox.Instance.GameManager.GameGrid;
        if (gameGrid != null)
        {
            if (_startTime == 0.0f)
                {
                // TBD: Put this elsewhere later.
                StartFollowing(gameGrid);
                }

            var map = gameGrid.Map;
            var path = gameGrid.CurrentPath; // TBD - This won't work when the path can move.


            while (true)
            {
                var t = Time.time;
                float deltaT = t - _startTime;
                float distCovered = deltaT * Speed;

                var nextPathVector = GridHelper.MapPointToVector(map, NextGridPoint);
                var prevPathVector = GridHelper.MapPointToVector(map, PrevGridPoint);
                float fracJourney = distCovered/(nextPathVector - prevPathVector).magnitude;

                if (fracJourney <= 1.0f)
                {
                    this.transform.position = Vector3.Lerp(prevPathVector, nextPathVector, fracJourney);
                    break;
                }
                else if (fracJourney < 2.0f)
                {
                    // We moved past the point, so go on to the next point.
                    if (IsTargetPathPoint(NextGridPoint))
                    {
                        Direction = Reverse(Direction);
                    }

                    GameGrid.GameCell gameCell = gameGrid.GetNextPathGameCell(NextGridPoint, Direction);
                    PrevGridPoint = NextGridPoint;
                    NextGridPoint = gameCell.GridPoint;

                    // TBD: There may have been a little time left, so we have to move further past current point.
                    _startTime = t;

                    continue;
                }
                else
                {
                    DebugSystem.DebugAssert(false, "moved way to far");
                    _startTime = t;
                    break;
                }
            }

        }

    }

    
    // This sucks because PathPoints don't tell me anything about the cells.
    private bool IsTargetPathPoint(GridPoint nextGridPoint)
    {
        var path = Toolbox.Instance.GameManager.GameGrid.CurrentPath; // TBD - This won't work when the path can move.

        var gameCell = FindPointOnPath(nextGridPoint, path);

        if (Direction == GameGrid.Directions.Forward)
        {
            if (gameCell.IsEnd)
            {
                return true;
            }
        }
        else
        {
            if (gameCell.IsStart)
            {
                return true;
            }
        }
        return false;
    }

    private GameGrid.GameCell FindPointOnPath(GridPoint nextGridPoint, List<GameGrid.GameCell> path)
    {
        foreach (var node in path)
        {
            if (node.GridPoint == nextGridPoint)
            {
                return node;
            }
        }
        return null;
    }

    private GameGrid.Directions Reverse(GameGrid.Directions direction)
    {
        if (direction == GameGrid.Directions.Back)
        {
            return GameGrid.Directions.Forward;
        }
        else
        {
            return GameGrid.Directions.Back;
        }
    }
}
