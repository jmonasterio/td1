﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Assets.Scripts;

/// <summary>
/// Common properties that every entity in the system should have.
/// </summary>
public class Entity : MonoBehaviour
{
    public event EventHandler Decomposed;
    private Transform _bulletsCollection;
    private DragSource _dragSourceOrNull;

    public enum EntityClasses
    {
        Background = 0,
        Tower = 1, // Cities are just special cases.
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

    public float Health = 5;
    public int HealthMax = 5;

    public ParticleSystem ExplosionPrefab;

    /// <summary>
    /// Minimum time between shots, if there is a target available. 
    /// 
    /// TBD: Should be part of a gun object, or all part of entity?
    /// </summary>
    public float ReloadTime = 0.75f;
    private float _reloadDelay;
    public float DecomposeTimeInterval = 7.0f;
    private float _decomposeTimeRemainingAtStartOfDrag;

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

        _bulletsCollection = GameObject.Find("Bullets").transform; // TBD: Maybe do this in the in the Enemy object.
        _dragSourceOrNull = this.GetComponent<DragSource>(); // TBD: Gross: Too many interactions between dragsource and entity.

    }

    public bool IsAlive()
    {
        return Health > 0;
    }

    public void Explode(bool destroy)
    {
        if (ExplosionPrefab == null)
        {
            Debug.LogError("Missing explosion prefab: " + this.gameObject.name);
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

    private bool IsDragging()
    {
        return ((_dragSourceOrNull != null) && _dragSourceOrNull.Dragging);

    }

    public void Update()
    {
        _reloadDelay -= Time.deltaTime;

        if (!IsDragging())
        {
            // As entity moves around, we want to update the CellMap to know where entity is.
            var cell = Toolbox.Instance.GameManager.GameGrid.MapPositionToGameCellOrNull(this.transform.position);
            UpdateCurrentCell(cell);
            if (IsDecomposingDone()) // Needs to be a constant
            {

                // We're done decomposing.
                // TBD: Sound and graphics here?
                EndDecomposing();
            }
        }
    }


    public bool IsReloaded()
    {
        return _reloadDelay <= 0;
    }

    public Tower FindClosestLiveTower(float fMaxDistance)
    {
        IEnumerable<Tower> nearby = Toolbox.Instance.GameManager.Towers().Where(_ => _.IsAlive());
        return Closest<Tower>(nearby, fMaxDistance);
    }

    public Human FindClosestLiveHuman(float fMaxDistance)
    {
        IEnumerable<Human> nearby = Toolbox.Instance.GameManager.Humans().Where(_ => _.IsAlive());
        return Closest<Human>(nearby, fMaxDistance);
    }

    public Enemy FindClosestLiveEnemy(float fMaxDistance)
    {
        IEnumerable<Enemy> nearby = Toolbox.Instance.GameManager.Enemies().Where( _ => _.IsAlive());
        return Closest<Enemy>( nearby, fMaxDistance);
    }

    private T Closest<T>(IEnumerable<T> nearby, float fMaxDistance) where T:MonoBehaviour
    {
        if (nearby.Any())
        {
            var here = this.transform.position;

            var ordered = nearby.OrderBy(_ => (_.transform.position - here).magnitude);
            var found = ordered.FirstOrDefault();
            if (found == null)
            {
                return null;
            }
            if ((found.transform.position - here).magnitude < fMaxDistance)
            {
                return found;
            }
            return null;
        }
        return null;
    }

    public void FireBulletAt<T>(T target, Bullet bulletPrefab) where T:MonoBehaviour
    {
        var bullet = Instantiate<Bullet>(bulletPrefab);
        var here = this.transform.position;
        bullet.direction = (target.transform.position - here).normalized;
        bullet.transform.position = here;
        bullet.BulletSource = this;
        bullet.transform.SetParent(_bulletsCollection);
        _reloadDelay = ReloadTime;
    }

    public void StartDecomposing()
    {
        Debug.Assert( _decomposeStartTime == 0.0f);
        _decomposeStartTime = Time.time;
        _decomposeTimeRemainingAtStartOfDrag = 0.0f;
    }

    public void ResumeDecompose()
    {
        if (_decomposeTimeRemainingAtStartOfDrag > 0.0f)
        {
            _decomposeStartTime = Time.time - _decomposeTimeRemainingAtStartOfDrag;
            _decomposeTimeRemainingAtStartOfDrag = 0.0f;
        }
    }

    public void PauseDecomposing()
    {
        if (_decomposeStartTime > 0.0f)
        {
            _decomposeTimeRemainingAtStartOfDrag = (Time.time - _decomposeStartTime);
        }
    }

    private void EndDecomposing()
    {
        Debug.Assert( !IsDragging());
        Debug.Assert( _decomposeStartTime > 0.0f);
        if (Decomposed != null)
        {
            Decomposed(this, new EventArgs());
        }
        _decomposeStartTime = 0.0f;
    }

    private bool IsDecomposingDone()
    {
        return (_decomposeStartTime > 0.0f) && (Time.time > _decomposeStartTime + DecomposeTimeInterval);
    }

    public static T InstantiateAt<T>(T prefab, GameObject parent, Vector3 pos, bool isSnap) where T:MonoBehaviour
    {
        var newGameObject = Instantiate<T>(prefab);
        newGameObject.transform.SetParent(parent.transform, worldPositionStays: false);
        newGameObject.transform.position = pos;
        //newGameObject.transform.localScale = this.transform.localScale;
        newGameObject.gameObject.layer = GameGrid.DRAG_LAYER;

        if (isSnap)
        {
            var snap = newGameObject.GetComponent<SnapToGrid>();
            snap.snapToGrid = false;
        }

        return newGameObject;
    }
}





