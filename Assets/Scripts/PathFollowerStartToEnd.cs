using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;

public class PathFollowerStartToEnd : PathFollower {

	// Use this for initialization
	new void Start () {
        this.AtFinish -= PathFollowerStartToEnd_AtFinish;
        this.AtFinish += PathFollowerStartToEnd_AtFinish;
        base.Start();
	}

    // If at end, head back to start. Or if at START, then done.
    private void PathFollowerStartToEnd_AtFinish(object sender, System.EventArgs e)
    {
        // TBD-Put in event handler for start to end follower.

        if (TargetCell.GroundType == GameGrid.GameCell.GroundTypes.End)
        {
            var enempyCmp = this.GetComponent<Enemy>();
            Toolbox.Instance.GameManager.GetComponent<ScoreController>().EnemyScored(enempyCmp.FlagCount);
            Object.Destroy(this.gameObject);

            // TBD: Put damage on robot here.
        }
    }

    public void SetPathFromStartToEndWayPoints(Waypoint startWaypoint, List<Waypoint> midWaypoints, Waypoint endWaypoint)
    {
        var gameGrid = Toolbox.Instance.GameManager.GameGrid;

        PrevGameCell = gameGrid.MapGridPointToGameCellOrNull(startWaypoint.GridPoint);
        CurrentGameCell = PrevGameCell;
        OrderedWaypointCells = GetCells(gameGrid, midWaypoints);
        //gameGrid.RandomizeEndCell();
        TargetCell = gameGrid.MapGridPointToGameCellOrNull(endWaypoint.GridPoint);
        FollowToTargetCell(gameGrid, transform.position);
        _startTime = Time.time;
        return;

    }

    private List<GameGrid.GameCell> GetCells(GameGrid gameGrid, List<Waypoint> midWaypoints)
    {
        var ret = new List<GameGrid.GameCell>();

        foreach (var waypoint in midWaypoints)
        {
            ret.Add( gameGrid.MapGridPointToGameCellOrNull(waypoint.GridPoint));
        }

        return ret;
    }


    // Update is called once per frame
    new void Update () {
	    base.Update();
	}
}
