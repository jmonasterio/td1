using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Tower : MonoBehaviour
{

    public enum TowerClasses
    {
        Shooter = 1,
        City = 2,
        GathererTower = 3, // Gathers's go get body of dead aliens. Humans get converted into gatherer.
    }

    public Bullet BulletPrefab;
    public float BulletRange;
    public TowerClasses TowerClass;
    public Human Spawn1Prefab; // TBD: Could be different types of things to spawn.

    private DragSource _dragSource;
    private Entity _entity;
    private float _timeSinceLastSpawn = 0.0f;


    // Use this for initialization
    void Start()
    {
        _entity = GetComponent<Entity>();
        _entity.Decomposed += _entity_Decomposed;
        _dragSource = GetComponent<DragSource>();
    }

    private void _entity_Decomposed(object sender, System.EventArgs e)
    {
        // TBD: Sounds and graphics. 
        // TBD: Try /catch
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (_dragSource != null && _dragSource.Dragging)
        {
            return;
        }

        if (TowerClass == TowerClasses.Shooter)
        {
            if (_entity.IsAlive() && _entity.IsReloaded())
            {

                var enemy = _entity.FindClosestLiveEnemy(BulletPrefab.BulletRange);
                    // Should be related to bullet range.
                if (enemy != null)
                {
                    _entity.FireBulletAt(enemy, BulletPrefab);

                }
            }
        }
        else if (TowerClass == TowerClasses.City)
        {
            // Spawn humans every once in a while.
            if (IsTimeToSpawnAgain())
            {
                var parent = GameObject.Find("Humans");

                // TBD: Humans settings should come from CSV... not from prefab.
                var newHuman = Entity.InstantiateAt(Spawn1Prefab, parent, this.transform.position, isSnap: false);
                newHuman.HumanClass = Human.HumanClasses.Gatherer;
                _timeSinceLastSpawn = Time.time;
            }
        }
        else if ( TowerClass == TowerClasses.GathererTower)
        {
            // TBD-DARRIN: When should gather towers shoot? When they're not birthing a gatherer? Never?
        }
    }

    private bool IsTimeToSpawnAgain()
    {
        if (_timeSinceLastSpawn == 0)
        {
            _timeSinceLastSpawn = Time.time;
            return false;
        }
        float spawnRate = Toolbox.Instance.GameManager.ScoreController.SpawnRate;

        if (Time.time - _timeSinceLastSpawn > spawnRate)
        {
            return true;
        }
        return false;
    }



    // TBD: Probably should be state. On an interface???
    public bool IsAlive()
    {
        return _entity.IsAlive();
    }

    // TBD: Same code in tower.
    // TBD: Similar code in Enemy.
    void OnTriggerEnter2D(Collider2D collision)
    {
        var colliderGo = collision.gameObject;
        var bullet = colliderGo.GetComponent<Bullet>();

        if (_dragSource.Dragging)
        {
            return;
        }

        if (bullet != null)
        {
            // Avoid hit to self
            if (bullet.BulletSource.GetComponent<Human>() != null)
            {
                // Enemy bullets should not hurt enemies.
                // TBD: Might be a cleaner way to do this.
                return;
            }
            if (bullet.BulletSource.GetComponent<Tower>() != null)
            {
                // Enemy bullets should not hurt enemies.
                // TBD: Might be a cleaner way to do this.
                return;
            }

            Debug.Log("Hit!");

            bullet.Destroy();

            if (_entity.Health > 0)
            {
                _entity.Health--;
                if (_entity.Health <= 0)
                {
                    //Toolbox.Instance.GameManager.ScoreController.Score += 100;
                    //Toolbox.Instance.GameManager.WavesController.LiveEnemyCount--;
                    _entity.Explode(destroy: false);
                    //SetAnimState(AnimStates.Carcas);
                    _entity.StartDecomposing();

                    // Will fire OnDecomposed() when times out.
                }
            }
        }
    }

    public void DropEnemyCarcas(Enemy enemy)
    {
        // TBD: Need to do different things, depending on the type of tower.
        Toolbox.Instance.GameManager.gameObject.GetComponent<ScoreController>().Income += enemy.CarcasIncomeValue;
    }

    public void DropHuman(Human human)
    {
        /* TBD: Need to do something different here */
        Toolbox.Instance.GameManager.gameObject.GetComponent<ScoreController>().Income += human.IncomeValue * 1;

    }

}
