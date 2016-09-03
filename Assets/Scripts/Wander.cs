using UnityEngine;
using System.Collections;
using Assets.Scripts;

/// <summary>
/// Goes to a random cell. When it gets there (or blocked), picks another random cell.
/// </summary>
public class Wander : MonoBehaviour
{
    public float speed = 5;
    public GameGrid.GameCell _targetCell = null;
    private PathFollower _pf;

    void Start()
    {
        // Set random initial rotation
        _pf = this.GetComponent<PathFollower>();
        _pf.AtFinish -= Pf_AtFinishOrBlocked;
        _pf.Blocked -= Pf_AtFinishOrBlocked;
        _pf.AtFinish += Pf_AtFinishOrBlocked;
        _pf.Blocked += Pf_AtFinishOrBlocked;

    }

    private void Pf_AtFinishOrBlocked(object sender, System.EventArgs e)
    {
        // Will try to make a new path on next update.
        _targetCell = null;
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

        var worldPos = this.transform.position;
        worldPos.z = -10;

        var gameGrid = Toolbox.Instance.GameManager.GameGrid;
        var endCell = gameGrid.RandomGameCell(GameGrid.GameCell.GroundTypes.Dirt);

        _pf.CurrentGameCell = gameGrid.MapPositionToGameCellOrNull(worldPos);
        _pf.PrevGameCell = _pf.CurrentGameCell;
        _pf.TargetCell = endCell;
        _pf.FollowToTargetCell(gameGrid, transform.position);
        return _pf.CurrentGameCell;
    }

    public void RestartWandering()
    {
        _targetCell = null;
    }

    public void StopWandering()
    {
        _pf.TargetCell = null;
    }
}