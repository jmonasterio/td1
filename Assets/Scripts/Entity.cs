using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Assets.Scripts;

/// <summary>
/// Common properties that every entity in the system should have.
/// </summary>
public class Entity : EntityBehavior
{
    private DragSource _dragSourceOrNull;

    public enum EntityClasses
    {
        Background = 0,
        Tower = 1, // Cities are just special cases.
        Human = 2,
        Robot = 3,
        Enemy = 4,
        Waypoint = 5,
        Carcas = 6,
    }

    public EntityClasses EntityClass;

    /// <summary>
    /// Cost for player to add this type of entity.
    /// </summary>
    public float BuildValue = 5;
    public float SpawnValue;

    public float Health = 5;
    public int HealthMax = 5;

    public ParticleSystem ExplosionPrefab;
    public Carcas CarcasPrefab;

    /// <summary>
    /// Minimum time between shots, if there is a target available. 
    /// 
    /// TBD: Should be part of a gun object, or all part of entity?
    /// </summary>
    public float ReloadTime = 0.75f;
    private float _reloadDelay;

    new void Start()
    {
        // Does some validation. 
        // TBD: Perhaps GetComponent should be source of truth and entity class should be calculated???
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
                if (null == this.GetComponent<Enemy>())
                {
                    Debug.LogError( "Missing enemy component: " + this.transform.name);
                }
                break;
            case EntityClasses.Carcas:
                Debug.Assert(null != this.GetComponent<Carcas>());
                break;
            case EntityClasses.Waypoint:
                Debug.Assert(null != this.GetComponent<Waypoint>());
                break;
        }

        _dragSourceOrNull = this.GetComponent<DragSource>(); 

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
            this.DestroyAndUpdateGrid(exp.duration);
        }
        Destroy(exp.gameObject, exp.duration); // Destroy explosion.
    }

    public float Speed = 1.0f;
    private GameGrid.GameCell _currentGameCell;

    public GameGrid.GameCell GetCurrentGameCell()
    {
        if (Entity.IsDragging())
        {
            return null;
        }
        if (_currentGameCell == null)
        {
            // Could happen if HUMAN's update called before ENTITY.
            var cell = Toolbox.Instance.GameManager.GameGrid.MapPositionToGameCellOrNull(this.transform.position);
            _currentGameCell = cell;
        }
        return _currentGameCell;
    }

    /// <summary>
    /// Must  be called whenever an entity moves or changes cells (drag, drop, teleport, etc). This keeps the Cells array up to date with the game.
    /// </summary>
    /// <param name="newGameCell"></param>
    private void UpdateCurrentCellAndGameGrid(GameGrid.GameCell newGameCell)
    {
        if ( _currentGameCell != newGameCell)
        {
            if ((_currentGameCell != null) && (newGameCell != null) && _currentGameCell.GridPoint == newGameCell.GridPoint)
            {
                // no change
            }
            else
            {
                if (_currentGameCell != null)
                {
                    RemoveEntityFromGameGrid();
                }
                _currentGameCell = newGameCell;
                if (_currentGameCell != null)
                {
                    GameGrid.AddEntityToGameGrid(_currentGameCell,this.gameObject, this.EntityClass);
                }
            }
        }

    }

 


    private void RemoveEntityFromGameGrid()
    {
        var cell = _currentGameCell;
        if (cell == null)
        {
            Debug.LogWarning("Could not get current cell for entity: " + this.name);
        }
        else
        {
            switch (this.EntityClass)
            {
                case Entity.EntityClasses.Background:
                    cell.Background = null;
                    break;
                case Entity.EntityClasses.Waypoint:

                    cell.WayPoint = null;
                    break;

                case Entity.EntityClasses.Enemy:
                    cell.Enemies.Remove(GetComponent<Enemy>());
                    break;
                case Entity.EntityClasses.Human:
                    cell.Humans.Remove(GetComponent<Human>());
                    break;
                case Entity.EntityClasses.Carcas:
                    var carcas = GetComponent<Carcas>();
                    Debug.Assert(carcas != null);
                    bool removed = cell.Carcases.Remove(carcas);
                    Debug.Assert(removed);
                    break;
                case Entity.EntityClasses.Robot:

                    cell.Robot = null;
                    break;
                case Entity.EntityClasses.Tower:
                    cell.Tower = null;
                    break;
                default:
                    Debug.Assert(false, "Unsupported entity type.");
                    break;

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
            UpdateCurrentCellAndGameGrid(cell);
        }
    }


    public bool IsReloaded()
    {
        return _reloadDelay <= 0;
    }

    public Carcas FindClosestCarcas(float fMaxDistance)
    {
        var nearby = Toolbox.Instance.GameManager.Carcases();
        return Closest<Carcas>(nearby, fMaxDistance);
    }

    public Tower FindClosestLiveTower(float fMaxDistance)
    {
        IEnumerable<Tower> nearby = Toolbox.Instance.GameManager.Towers();
        return Closest<Tower>(nearby, fMaxDistance);
    }

    public Human FindClosestLiveHuman(float fMaxDistance)
    {
        IEnumerable<Human> nearby = Toolbox.Instance.GameManager.Humans();
        return Closest<Human>(nearby, fMaxDistance);
    }

    public Enemy FindClosestLiveEnemy(float fMaxDistance)
    {
        IEnumerable<Enemy> nearby = Toolbox.Instance.GameManager.Enemies();
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
        bullet.BulletSource = EntityClass;

        var bulletsCollection = Toolbox.Instance.GameManager.Nodes.BulletsCollection;
        bullet.transform.SetParent(bulletsCollection);
        _reloadDelay = ReloadTime;
    }

    public static T InstantiateAt<T>(T prefab, GameObject parent, Vector3 pos, bool isSnap) where T:MonoBehaviour
    {
        var newGameObject = Instantiate<T>(prefab);
        newGameObject.transform.SetParent(parent.transform, worldPositionStays: false);
        newGameObject.transform.position = pos;

        if (isSnap)
        {
            var snap = newGameObject.GetComponent<SnapToGrid>();
            snap.snapToGrid = false;
        }

        return newGameObject;
    }

    public void SwitchToCarcas()
    {
        if (CarcasPrefab == null)
        {
            Debug.LogError("Missing carcas prefab: " + this.transform.name);
        }

        GameObject parentGo;
        var parent = this.transform.parent;
        if (parent == null)
        {
            parentGo = null;
            Debug.LogWarning("No parent for: " + this.transform.name);
        }
        else
        {
            parentGo = parent.gameObject;
        }

        var carcas = InstantiateAt(CarcasPrefab, parentGo, this.transform.position, isSnap: false);
        carcas.CarcasClass = this.EntityClass;
        Debug.Assert(this.BuildValue > 0.0f);
        var carcasEntity = carcas.GetComponent<Entity>();
        carcasEntity.BuildValue = this.BuildValue;

        this.DestroyAndUpdateGrid();
    }

    public bool TakeDamageFromBullet(Bullet bullet)
    {
        bool justDied = false;
        if (this.Health > 0)
        {
            this.Health--;
            if (this.Health <= 0)
            {
                this.Explode(destroy: false);
                this.SwitchToCarcas();
                justDied = true;
            }
        }
        return justDied;
    }

    /// <summary>
    /// Every time we destroy an object, we need to remove from gamegrid.
    /// </summary>
    public void DestroyAndUpdateGrid( float delayBeforeDestroy = 0.0f)
    {
        if (delayBeforeDestroy == 0.0f)
        {
            RemoveEntityFromGameGrid();
            Destroy(this.gameObject);
        }
        else
        {
            StartCoroutine(
                CoroutineUtils.Chain(
                    CoroutineUtils.WaitForSeconds(delayBeforeDestroy),
                    CoroutineUtils.Do(() =>
                    {
                
                        RemoveEntityFromGameGrid();
                        Destroy(this.gameObject);
                    }
                 )));
        }
    }
}