﻿using Assets.Scripts;
using UnityEngine;

public class DragSource : MonoBehaviour
{
    public AudioClip InvalidDropSound;
    public AudioClip CancelDropSound;
    public AudioClip DropSound;
    public bool Dragging;
    public bool DeleteOnCancel; // If you mouse-cancel the drag from palette, we want to delete the object.
    private Vector3 _startPos;
    private Vector3 _startMousePos;
    private Color _originalColor;
    private SpriteRenderer _spriteRender;
    private Entity _entity;

    public void Start()
    {
        _entity = this.GetComponent<Entity>();
    }

    public void CancelDragging()
    {
        EndDragging(cancel:true);
    }

    // Does the final cleanup. Will hand
    private void EndDragging( bool cancel)
    {
        // If canceling, return any money paid to buy an item from the palette.
        if (cancel && (this.CostPaidToBuild > 0.0f))
        {
            Toolbox.Instance.GameManager.ScoreController.Income += this.CostPaidToBuild;
            this.CostPaidToBuild = 0.0f;
        }

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
                GameManagerScript.PlayClip(CancelDropSound);
            }
            else
            {
                GameManagerScript.PlayClip(DropSound);
            }
            Dragging = false;
            _entity.ResumeDecompose();
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
        var entity = dragSource.GetComponent<Entity>();
        Debug.Assert(entity != null);
        entity.PauseDecomposing();
    }

    public bool CanDropAt(GameGrid.GameCell dropCellOrNull)
    {
        if (dropCellOrNull == null)
        {
            return false;
        }

        switch (_entity.EntityClass)
        {
            case Entity.EntityClasses.Human:
                var human = this.GetComponent<Human>();
                if (human != null)
                {
                    return dropCellOrNull.Background == null ||
                           (dropCellOrNull.GroundType != GameGrid.GameCell.GroundTypes.Path); // Only allowed drop.
                }
                break;

            case Entity.EntityClasses.Tower:
                var tower = this.GetComponent<Tower>();
                if (tower != null)
                {
                    return dropCellOrNull.Tower == null;
                }
                break;
            case Entity.EntityClasses.Enemy:
            {
                // It's ok to drop an enemy carcas on a tower or robot.
                var enemy = this.GetComponent<Enemy>();
                Debug.Assert( !enemy.IsAlive());
                return (dropCellOrNull.Tower != null) || (dropCellOrNull.Robot != null);
            }
            case Entity.EntityClasses.Background:
            {
                var block = this.GetComponent<Block>();
                if (block != null)
                {
                    return dropCellOrNull.Background == null && dropCellOrNull.Humans.Count == 0 &&
                           dropCellOrNull.Tower == null && dropCellOrNull.Robot == null &&
                           dropCellOrNull.Enemies.Count == 0;
                }
                break;
            }
        }
        return true;
    }

    /// <summary>
    /// TBD: We probably already know gamecell in caller, so why not pass in?
    /// </summary>
    public void FinishOrCancelDragging(GameGrid.GameCell dropCellOrNull, Vector3 mapExactDrop)
    {
        if (dropCellOrNull != null)
        {
            if( _entity.EntityClass == Entity.EntityClasses.Human)
            {
                var human = this.GetComponent<Human>();
                // We're only allowed to drop on intersting cells.
                if (dropCellOrNull.Robot != null)
                {
                    var robot = dropCellOrNull.Robot;
                    robot.DropHuman(human);
                    Destroy(human.gameObject);
                    GameManagerScript.PlayClip(DropSound);
                    return;
                }
                if (dropCellOrNull.Tower != null)
                {
                    var tower = dropCellOrNull.Tower;
                    tower.DropHuman(human); // TBD: Should DropHuman be a component that everyone has?
                    Destroy(human.gameObject);
                    GameManagerScript.PlayClip(DropSound);
                    return;
                }
                else if (dropCellOrNull.Background != null)
                {
                    CancelDragging();
                    return;
                }
                else
                {
                    if (CanHumanWalkOn(dropCellOrNull.GroundType))
                    {
                        human.DropAt(mapExactDrop);
                        EndDragging(cancel: false);
                    }
                }
            }
            else
            {
                switch (_entity.EntityClass)
                {
                    case Entity.EntityClasses.Tower:
                        if (dropCellOrNull.Tower != null)
                        {
                            // TBD: Cell already occupied. Need a sound
                            CancelDragging();
                            return;
                        }
                        this.gameObject.layer = GameGrid.TOWER_LAYER;
                        dropCellOrNull.Tower = this.GetComponent<Tower>();
                        break;
                    case Entity.EntityClasses.Background:
                        if (dropCellOrNull.Background != null)
                        {
                            // TBD: Cell already occupied. Need a sound
                            CancelDragging();
                            return;
                        }
                        this.gameObject.layer = GameGrid.BACKGROUND_LAYER;
                        dropCellOrNull.Background = this.gameObject;
                        break;
                    case Entity.EntityClasses.Enemy:
                    {
                        var enemy = _entity.GetComponent<Enemy>();
                        if (dropCellOrNull.Tower != null)
                        {
                            var tower = dropCellOrNull.Tower;
                            tower.DropEnemyCarcas(enemy); // TBD: Should DropHuman be a component that everyone has?
                        }
                        else if (dropCellOrNull.Robot != null)
                        {
                            var robot = dropCellOrNull.Robot;
                            robot.DropEnemyCarcas(enemy); // TBD: Should DropHuman be a component that everyone has?

                        }
                        else
                        {
                            CancelDragging();
                            return;
                        }

                        break;
                    }

                    default:
                        CancelDragging();
                        return;

                }

                this.gameObject.transform.position =
                    SnapToGrid.RoundTransform(this.gameObject.transform.position, 1.0f);
                EndDragging(cancel: false);
            }
        }
        else
        {
            // Not a valid cell.
            // TBD: Need a sound.

            EndDragging(cancel:false);
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
        var entity = this.GetComponent<Entity>();
        switch (entity.EntityClass)
        {
            case Entity.EntityClasses.Human:
                var human = this.GetComponent<Human>();
                return (human != null && human.IsAlive());
            case Entity.EntityClasses.Enemy:
                var enemy = this.GetComponent<Enemy>();
                return (enemy != null && !enemy.IsAlive()); // Can only drag carcas.
            default:
                return false;

        }
    }

    public float CostPaidToBuild { get; set; }
}