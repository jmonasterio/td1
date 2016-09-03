using UnityEngine;
using System.Collections;
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

    public void SetPathFromStartToEndWayPoints(Waypoint startWaypoint, Waypoint endWaypoint)
    {
        var gameGrid = Toolbox.Instance.GameManager.GameGrid;

        MakePathFromStartToEnd(gameGrid, startWaypoint, endWaypoint);
        FollowToTargetCell(gameGrid, transform.position);
        _startTime = Time.time;
        return;

    }


    private void MakePathFromStartToEnd(GameGrid gameGrid, Waypoint startWaypoint, Waypoint endWaypoint)
    {
        PrevGameCell = gameGrid.MapGridPointToGameCellOrNull(startWaypoint.GridPoint);
        CurrentGameCell = PrevGameCell;
        //gameGrid.RandomizeEndCell();
        TargetCell = gameGrid.MapGridPointToGameCellOrNull(endWaypoint.GridPoint);
    }




    // Update is called once per frame
    new void Update () {
	    base.Update();
	}
}
