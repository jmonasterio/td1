using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;

/// <summary>
/// Common properties that every entity in the system should have.
/// </summary>
public class Entity : MonoBehaviour
{
    public event EventHandler Decomposed;

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
    private float _reloadDelay;

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
        Destroy(exp.gameObject, exp.duration); // Destroy explosion.
    }

    public float Speed = 1.0f;
    private GameGrid.GameCell _currentGameCell;
    private float _decomposeStartTime;

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
        _reloadDelay -= Time.deltaTime;

        var dragSource = this.GetComponent<DragSource>(); // Otherwise, we can never drop because it looks like the cell is occupied by the thing we're dragging.
        if (dragSource == null || !dragSource.Dragging)
        {
            // As entity moves around, we want to update the CellMap to know where entity is.
            var cell = Toolbox.Instance.GameManager.GameGrid.MapPositionToGameCellOrNull(this.transform.position);
            UpdateCurrentCell(cell);
        }

        if (_decomposeStartTime > 0)
        {
            // We're done decomposing.
            // TBD: Sound and graphics here?
            if (Time.time > _decomposeStartTime + 5.0f)
            {
                _decomposeStartTime = 0.0f;
                if (Decomposed != null)
                {
                    Decomposed(this, new EventArgs());
                }
            }
        }
    }

    public void StartDecomposing( float decomposeStartTime)
    {
        _decomposeStartTime = decomposeStartTime;
    }

    public bool IsReloaded()
    {
        return _reloadDelay <= 0;
    }

    public Enemy FindClosestEnemy(float fMax)
    {
        IEnumerable<Enemy> nearby = Toolbox.Instance.GameManager.Enemies().Where( _ => _.IsAlive());
        if (nearby.Any())
        {
            var here = this.transform.position;

            var ordered = nearby.OrderBy(_ => (_.transform.position - here).magnitude);
            var found = ordered.FirstOrDefault();
            if (found == null)
            {
                return null;
            }
            if ((found.transform.position - here).magnitude < fMax)
            {
                return found;
            }
            return null;
        }
        return null;
    }

    public void StartReload()
    {
        _reloadDelay = ReloadTime;
    }




}





