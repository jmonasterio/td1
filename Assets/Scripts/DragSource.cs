using Assets.Scripts;
using UnityEngine;

public class DragSource : MonoBehaviour
{
    public bool Dragging;
    public bool DeleteOnCancel; // If you mouse-cancel the drag from palette, we want to delete the object.
    private Vector3 _startPos;
    private Vector3 _startMousePos;
    private Color _originalColor;
    private SpriteRenderer _spriteRender;

    public void CancelDragging()
    {
        EndDragging(cancel:true);
    }

    // Does the final cleanup. Will hand
    private void EndDragging( bool cancel)
    {
        if (this.DeleteOnCancel && cancel)
        {
            Object.Destroy(this.gameObject);
        }
        else
        {
            RestoreOpacity();
            if (cancel)
            {
                RestorePosition();
            }
            Dragging = false;
        }
    }

    private void RestorePosition()
    {
// Restore position
        this.transform.position = _startPos;
    }

    private void RestoreOpacity()
    {
        _spriteRender.color = _originalColor;
    }

    public void StartDragging( Vector3 mousePosition)
    {
        var dragSource = this;
        dragSource.Dragging = true;
        dragSource.gameObject.layer = GameGrid.DRAG_LAYER;
        _startPos = dragSource.transform.position;
        _startMousePos = mousePosition;
        _spriteRender = dragSource.GetComponent<SpriteRenderer>();
        _originalColor = _spriteRender.color;
        const float TRANSPARENT_50 = 0.5f;
        _spriteRender.color = new Color(_originalColor.r, _originalColor.g, _originalColor.b, TRANSPARENT_50);
    }

    public bool CanDropAt(GameGrid.GameCell dropCellOrNull)
    {
        if (dropCellOrNull == null)
        {
            return false;
        }
        var human = this.GetComponent<Human>();
        if (human != null)
        {
            return true; // Only allowed drop.
        }
        var city = this.GetComponent<Resource>();
        if (city != null)
        {
            return dropCellOrNull.BackgroundGameObject == null;
        }
        var block = this.GetComponent<Block>();
        if (block != null)
        {
            return dropCellOrNull.BackgroundGameObject == null;
        }
        return true;
    }

    /// <summary>
    /// TBD: We probably already know gamecell in caller, so why not pass in?
    /// </summary>
    public void FinishOrCancelDragging(GameGrid.GameCell dropCellOrNull)
    {
        var dragSource = this;
        var gameGrid = Toolbox.Instance.GameManager.GameGrid;

        if (dropCellOrNull != null)
        {
            var human = dragSource.GetComponent<Human>();
            if (human != null)
            {
                // We're only allowed to drop on intersting cells.
                if (dropCellOrNull.BackgroundGameObject != null)
                {
                    var target = dropCellOrNull.BackgroundGameObject;
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
                    else if (target.GetComponent<Block>())
                    {
                        CancelDragging();
                        return;
                    }

                }
                else
                {
                    if (CanHumanWalkOn(dropCellOrNull.GroundType))
                    {
                        gameGrid.DropGameObjectAtGameCell(human.gameObject, dropCellOrNull);
                        human.gameObject.layer = GameGrid.BACKGROUND_LAYER;
                        var wander = human.GetComponent<Wander>();
                        wander.RestartWandering();
                        RestoreOpacity();
                    }
                }


            }
            else
            {

                var income = Toolbox.Instance.GameManager.gameObject.GetComponent<ScoreController>().Income;
                
                // TBD: Race.
                // TBD: Move to EndDragging !Cancel
                var entity = this.GetComponent<Entity>();
                var cost = entity.IncomeCost;
                if (income >= cost)
                {
                    Toolbox.Instance.GameManager.gameObject.GetComponent<ScoreController>().Income -= cost;
                }
                else
                {
                    EndDragging( cancel:true);
                    return;
                }


                // Disallow dups on 
                if (dropCellOrNull.BackgroundGameObject != null)
                {
                    CancelDragging();
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
                EndDragging(cancel:false);
            }
        }
        else
        {
            CancelDragging();
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

    public bool CanStartDragging()
    {
        var human = this.GetComponent<Human>();
        return (human != null);
    }
}