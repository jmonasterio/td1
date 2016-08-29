using Assets.Scripts;
using UnityEngine;

public class DragSource : MonoBehaviour
{
    public bool Dragging;
    private Vector3 _startPos;
    private Vector3 _originalScale;
    private float _distance;

    public void CancelDragging()
    {
        this.transform.position = _startPos;
        this.transform.localScale = _originalScale;
        CancelDragging(this);
    }

    private static void CancelDragging(DragSource dragSource)
    {
    }

    public void StartDragging()
    {
        var dragSource = this;
        dragSource.Dragging = true;
        _startPos = dragSource.transform.position;
        _originalScale = dragSource.transform.localScale;
        _distance = Vector3.Distance(dragSource.transform.position, Camera.main.transform.position);
        var ds = GetComponent<DragSpawner>();
        if (ds != null)
        {
            ds.StartDrag();
        }

    }


    public void FinishOrCancelDragging()
    {
        var dragSource = this;
        var gameGrid = Toolbox.Instance.GameManager.GameGrid;
        var dropCell = gameGrid.GetSelectorCellOrNull();

        if (dropCell != null)
        {
            var human = dragSource.GetComponent<Human>();
            if (human != null)
            {
                // We're only allowed to drop on intersting cells.
                if (dropCell.BackgroundGameObject != null)
                {
                    var target = dropCell.BackgroundGameObject;
                    if (target.GetComponent<Resource>())
                    {
                        var resource = target.GetComponent<Resource>();
                        resource.DropHuman(human);
                        Destroy(human.gameObject);
                        return;
                    }
                    else if (target.GetComponent<Robot>())
                    {
                        var robot = target.GetComponent<Robot>();
                        robot.DropHuman(human);
                        Destroy(human.gameObject);
                        return;
                    }
                    else if (target.GetComponent<Tower>())
                    {
                        var tower = target.GetComponent<Tower>();
                        tower.DropHuman(human); // TBD: Should DropHuman be a component that everyone has?
                        Destroy(human.gameObject);
                        return;
                    }

                }
                else
                {
                    if (CanHumanWalkOn(dropCell.GroundType))
                    {
                        gameGrid.DropGameObjectAtGameCell(human.gameObject, dropCell);
                        var wander = human.GetComponent<Wander>();
                        wander.RestartWandering();
                    }
                }


            }
            else
            {
                var ds = dragSource.GetComponent<DragSpawner>();
                if (ds != null)
                {
                    // Only blank cells.
                    if (dropCell.BackgroundGameObject == null)
                    {
                        Toolbox.Instance.GameManager.GetComponent<ScoreController>().Income -=
                            ds.SpawnPF.GetComponent<Entity>().IncomeCost;
                        gameGrid.InstaniatePrefabAtGameCell(ds.SpawnPF, dropCell);
                    }

                    ds.StopDrag();

                    // FALL THRU TO CANCEL.
                }
            }
        }
        else
        {
            this.CancelDragging();
        }
    }

    private static bool CanHumanWalkOn(GameGrid.GameCell.GroundTypes groundType)
    {
        return groundType == GameGrid.GameCell.GroundTypes.Dirt || groundType == GameGrid.GameCell.GroundTypes.Path;
    }

    public bool IsMovedAwayFromClick()
    {
        return (_startPos - transform.position).magnitude > 0.1f;
    }
}