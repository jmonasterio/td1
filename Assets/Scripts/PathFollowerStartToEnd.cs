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

        if (TargetCell.GroundType == GameGrid.GameCell.GroundTypes.Start)
        {
            var enempyCmp = this.GetComponent<Enemy>();
            Toolbox.Instance.GameManager.GetComponent<ScoreController>().EnemyScored(enempyCmp.FlagCount);
            Object.Destroy(this.gameObject);
        }
        else
        {
            // Go back to start.
            var gameGrid = Toolbox.Instance.GameManager.GameGrid;
            TargetCell = gameGrid.GetStartGameCell();
            FollowToTargetCell(gameGrid);
            Debug.Assert(NextGameCell != null);
        }
    }

    public void SetRandomTarget()
    {
        var gameGrid = Toolbox.Instance.GameManager.GameGrid;

        MakePathFromStartToEnd(gameGrid);
        FollowToTargetCell(gameGrid);
        _startTime = Time.time;
        return;

    }


    private void MakePathFromStartToEnd(GameGrid gameGrid)
    {
        PrevGameCell = gameGrid.GetStartGameCell();
        CurrentGameCell = PrevGameCell;
        //gameGrid.RandomizeEndCell();
        TargetCell = gameGrid.GetEndGameCell();

    }




    // Update is called once per frame
    new void Update () {
	    base.Update();
	}
}
