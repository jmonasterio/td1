using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Algorithms;

public class PathFollower : MonoBehaviour {

    private List<PathFinderNode> _pathNodeList;

    public float Speed = 1.0f;

    public GameGrid.GameCell CurrentGameCell; // The one we are in.
    public GameGrid.GameCell PrevGameCell; // The one we are leaving.
    public GameGrid.GameCell NextGameCell; // The on we are going to.
    public GameGrid.GameCell TargetCell; // The one we are going to

    private float _startTime;

    // Use this for initialization
    public void Start()
    {
    }

    public List<PathFinderNode> Path
    {
        get
        {
            return _pathNodeList;
            
        }
        set { _pathNodeList = value; }
    } 

    public void MakeNewRandomPath( GameGrid gameGrid)
    {
        PrevGameCell = gameGrid.GetStartGameCell();
        CurrentGameCell = PrevGameCell;
        //gameGrid.RandomizeEndCell();
        TargetCell = gameGrid.GetEndGameCell();

    }


    public void FollowToTargetCell(GameGrid gameGrid)
    {
        PrevGameCell = CurrentGameCell;

        // Changed target, so find a new path.
        var ignorePath = gameGrid.FindPath(CurrentGameCell, TargetCell, this);
        if (ignorePath == null || ignorePath.Count == 0)
        {
            // Couldn't find a path!!!!
            ignorePath = gameGrid.FindPath(CurrentGameCell, TargetCell, this);
            Debug.Assert(false, "Couldn't find a path! Happens randomly. What???");
        }

        NextGameCell = gameGrid.GetNextPathGameCell(PrevGameCell, this);
        Debug.Assert(NextGameCell != null, "Should have something on path? What is up?");


    }



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
        var gameGrid = Toolbox.Instance.GameManager.GameGrid;
        if (gameGrid != null)
        {
            var t = Time.time;

            if (TargetCell == null)
            {
                MakeNewRandomPath(gameGrid);
                FollowToTargetCell(gameGrid);
                _startTime = t;
                return;
            }


            var map = gameGrid.Map;

            // Refigure the path on each update (in case the path has changed).
            // TBD: We could optimize this, if we know there is nothing moving that can block things on path,
            //  and we're sure the target is still there ... more complex games might allow the user to drop
            /// walls to block paths.
            var path = gameGrid.FindPath(PrevGameCell, TargetCell,this);

            //var nextGameCell = FindNextGameCell( path, PrevGameCell);

            // If the path is no longer valid.
            //if( !IsPathStillValid(path, PrevGameCell, NextGameCell, this.transform.position))
            //{
            //    //DebugSystem.DebugAssert(false, "Need to handle case where next cell is blocked (no longer on path).");
            //}

            float deltaT = t - _startTime;
            float distCovered = deltaT*Speed;

            Debug.Assert(NextGameCell != null);
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
                    if (TargetCell.GroundType == GameGrid.GameCell.GroundTypes.Start)
                    {
                        // TBD-JM: This is just for testing. Really we want to destroy
                        //  units and add to score.
                        // TBD-JM: Lame that we have to cast to ENEMY.
                        Toolbox.Instance.GameManager.Enemies().Remove(this.GetComponent<Enemy>());
                        Destroy(this.gameObject);
                    }
                    else
                    {
                        // Go back to start.
                        TargetCell = gameGrid.GetStartGameCell();
                        FollowToTargetCell(gameGrid);
                        ;
                        Debug.Assert(NextGameCell != null);

                    }
                }
                else
                {

                    // TBD: There may have been a little time left, so we have to move further past current point.
                    _startTime = t;
                    FollowToTargetCell(gameGrid);
                }
            }
            else
            {
                // Why would we ever need to go this far, unless we missed aframe?
                DebugSystem.DebugAssert(false, "moved way to far");
                _startTime = t;
                FollowToTargetCell(gameGrid);
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
            return false;
        }

    }
}
