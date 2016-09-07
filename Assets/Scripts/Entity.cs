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

    public ParticleSystem ExplosionPrefab;

    /// <summary>
    /// Minimum time between shots, if there is a target available. 
    /// 
    /// TBD: Should be part of a gun object, or all part of entity?
    /// </summary>
    public float ReloadTime = 0.75f;

    public void Start()
    {
        // TBD: Does some validation. Perhaps GetComponent should be source of truth and entity class should be calculated???
        switch (EntityClass)
        {
            case EntityClasses.Background:
                Debug.Assert( null != this.GetComponent<Block>());
                break;
            case EntityClasses.Tower:
                Debug.Assert( null != this.GetComponent<Tower>());
                break;
            case EntityClasses.Human:
                Debug.Assert(null != this.GetComponent<Human>());
                break;
            case EntityClasses.Enemy:
                Debug.Assert(null != this.GetComponent<Enemy>());
                break;
            case EntityClasses.Waypoint:
                Debug.Assert(null != this.GetComponent<Waypoint>());
                break;
        }
    }

    public bool IsAlive()
    {
        return Health > 0;
    }

    public void Explode(bool destroy)
    {
        if (ExplosionPrefab == null)
        {
            Debug.LogError("Missing explosion prefab");
            return;
        }
        var exp = Instantiate<ParticleSystem>(ExplosionPrefab);
        exp.transform.SetParent( this.transform.parent);
        exp.transform.position = this.transform.position;
        exp.gameObject.layer = GameGrid.BACKGROUND_LAYER;

        exp.Play();
        if (destroy)
        {
            Destroy(gameObject, exp.duration);
        }
        Destroy(exp, exp.duration); // Destroy explosion.
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


