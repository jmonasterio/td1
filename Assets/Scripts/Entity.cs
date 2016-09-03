using UnityEngine;
using System.Collections;
using Assets.Scripts;

/// <summary>
/// Common properties that every entity in the system should have.
/// </summary>
public class Entity : MonoBehaviour
{
    public enum EntityClasses
    {
        Background = 0,
        Tower = 1,
        Human = 2,
        Robot = 3,
        Enemy = 4,
        Waypoint = 5,


    }

    public EntityClasses EntityClass;

    /// <summary>
    /// Cost for player to add this type of entity.
    /// </summary>
    public int IncomeCost;

    public int Health = 5;
    public int HealthMax = 5;

    public bool IsAlive()
    {
        return Health > 0;
    }

    public float Speed = 1.0f;
    private GameGrid.GameCell _currentGameCell;

    public GameGrid.GameCell GetCurrentGameCell()
    {
        return _currentGameCell;
    }

    /// <summary>
    /// Must  be called whenever an entity moves or changes cells (drag, drop, teleport, etc). This keeps the Cells array up to date with the game.
    /// </summary>
    /// <param name="newGameCell"></param>
    private void UpdateCurrentCell(GameGrid.GameCell newGameCell)
    {
        if (_currentGameCell != newGameCell)
        {
            if (_currentGameCell != null)
            {
            GameGrid.RemoveEntity(_currentGameCell, this.gameObject, EntityClass);
        }
        _currentGameCell = newGameCell;
            if (_currentGameCell != null)
            {
                GameGrid.SetCellEntity(newGameCell, this.gameObject, EntityClass);
            }
        }

    }

    public void Update()
    {
        
        var dragSource = this.GetComponent<DragSource>(); // Otherwise, we can never drop because it looks like the cell is occupied by the thing we're dragging.
        if (dragSource == null || !dragSource.Dragging)
        {
            // As entity moves around, we want to update the CellMap to know where entity is.
            var cell = Toolbox.Instance.GameManager.GameGrid.MapPositionToGameCellOrNull(this.transform.position);
            UpdateCurrentCell(cell);
        }
    }


}


