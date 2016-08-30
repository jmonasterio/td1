using Assets.Scripts;
using UnityEngine;

public class DragSource : MonoBehaviour
{
    public bool Dragging;
    private Vector3 _startPos;
    private Vector3 _startMousePos;

    public void CancelDragging()
    {
        this.transform.position = _startPos;
        Dragging = false;
    }

    public void StartDragging( Vector3 mousePosition)
    {
        var dragSource = this;
        dragSource.Dragging = true;
        _startPos = dragSource.transform.position;
        _startMousePos = mousePosition;
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
                // Disallow dups on 
                if (dropCell.BackgroundGameObject != null)
                {
                    this.CancelDragging();
                    return;
                }

                var city = dragSource.GetComponent<Resource>();
                if (city != null)
                {
                    dragSource.gameObject.layer = GameGrid.RESOURCE_LAYER;


                }
                else
                {
                    dragSource.gameObject.layer = GameGrid.BACKGROUND_LAYER;
                    dragSource.gameObject.transform.position = SnapToGrid.RoundTransform( dragSource.gameObject.transform.position, 1.0f);
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

    public bool IsMovedAwayFromClick( Vector3 mousePos)
    {
        return (_startMousePos - mousePos).magnitude > 0.2f;
    }
}