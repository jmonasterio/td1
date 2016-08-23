using UnityEngine;
using System.Collections;

/// <summary>
/// Goes to a random cell. When it gets there (or blocked), picks another random cell.
/// </summary>
public class Wander : MonoBehaviour
{
    public float speed = 5;
    public GameGrid.GameCell _targetCell = null;

    void Start()
    {
        var pf = this.GetComponent<PathFollower>();
        pf.AtFinish += Pf_AtFinishOrBlocked;
        pf.Blocked += Pf_AtFinishOrBlocked;

    }

    private void Pf_AtFinishOrBlocked(object sender, System.EventArgs e)
    {
        MakeRandomPath();
    }

    void Update()
    {

        // TBD: Are we there, yet?
        if (_targetCell == null)
        {
            _targetCell = MakeRandomPath();
        }
    }

    private GameGrid.GameCell MakeRandomPath()
    {
        // Set random initial rotation
        var pf = this.GetComponent<PathFollower>();

        var worldPos = this.transform.position;
        worldPos.z = -10;

        var gameGrid = Toolbox.Instance.GameManager.GameGrid;
        var endCell = gameGrid.RandomGameCell(GameGrid.GameCell.GroundTypes.Dirt);

        pf.CurrentGameCell = gameGrid.MapPositionToGameCellOrNull(worldPos);
        pf.PrevGameCell = pf.CurrentGameCell;
        pf.TargetCell = endCell;
        pf.FollowToTargetCell(gameGrid);
        return pf.CurrentGameCell;
    }

    public void RestartWandering()
    {
        MakeRandomPath();
    }
}