using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;

public class Tower : EntityBehavior
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
    private float _availableGrowPower = 0.0f;
    private float _humanGrowTotal = 0.0f;


    // Use this for initialization
    new void Start()
    {
        base.Start();
        _dragSource = GetComponent<DragSource>();
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
            if ( _entity.IsReloaded())
            {
                var enemy = _entity.FindClosestLiveEnemy(BulletPrefab.BulletRange);
                if (enemy != null)
                {
                    _entity.FireBulletAt(enemy, BulletPrefab);

                }
            }
        }
        else if (TowerClass == TowerClasses.GathererTower || TowerClass == TowerClasses.City)
        {
            // Grow humans every once in a while.
            // Use up some resource to grow human
            var result = GrowHumanALittleBit(Time.deltaTime);
            
            if (result == GrowHumanResult.NotEnoughGrowPower)
            {
                AttemptToTakeSomeGrowPowerFromScore();
            }
            else if ( result == GrowHumanResult.Finished)
            {
                var parent = GameObject.Find("Humans");

                // TBD: Humans settings should come from CSV... not from prefab.
                var newHuman = Entity.InstantiateAt(Spawn1Prefab, parent, this.transform.position, isSnap: false);
            }
            else if( result == GrowHumanResult.InProgress)
            {
                // TBD: Update progress
            }
            else
            {
                Debug.LogError("Unknown result growing human");
            }
        }
        else if ( TowerClass == TowerClasses.GathererTower)
        {
            // TBD-DARRIN: When should gather towers shoot? When they're not birthing a gatherer? Never?
        }
    }

    // Not thread safe.
    private void AttemptToTakeSomeGrowPowerFromScore()
    {
        if (Toolbox.Instance.GameManager.ScoreController.GrowScore > AMOUNT_TO_TAKE)
        {
            _availableGrowPower += AMOUNT_TO_TAKE;
            Toolbox.Instance.GameManager.ScoreController.GrowScore -= AMOUNT_TO_TAKE;
        }
    }

    private enum GrowHumanResult
    {
        InProgress,
        NotEnoughGrowPower,
        Finished,
    }

    private const float MAX_GROW_POWER_PER_SECOND = 1.0f;
    private const float NEEDED_FOR_HUMAN = 0.8f;
    private const float AMOUNT_TO_TAKE = 1.0f;
    private const float AMOUNT_TO_RELOAD_AT = 0.01f;

    private GrowHumanResult GrowHumanALittleBit(float deltaTime)
    {
        if (_availableGrowPower <= AMOUNT_TO_RELOAD_AT)
        {
            return GrowHumanResult.NotEnoughGrowPower;
        }

        float powerToUserPerSecond = Mathf.Min(_availableGrowPower, MAX_GROW_POWER_PER_SECOND);
        float used = deltaTime*powerToUserPerSecond;
        _availableGrowPower -= used;

     
        _humanGrowTotal += used*Toolbox.Instance.GameManager.ScoreController.GrowRate;

        if (_humanGrowTotal > NEEDED_FOR_HUMAN) // TBD: Each kind of human could take longer
        {
            _availableGrowPower += (_humanGrowTotal - NEEDED_FOR_HUMAN);
            _humanGrowTotal = 0.0f;
            return GrowHumanResult.Finished;
        }
        else
        {
            return GrowHumanResult.InProgress;
        }

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
            if (bullet.BulletSource != Entity.EntityClasses.Enemy)
            {
                return;
            }

            bullet.Destroy();

            if (_entity.Health > 0)
            {
                _entity.Health--;
                if (_entity.Health <= 0)
                {
                    //Toolbox.Instance.GameManager.ScoreController.Score += 100;
                    //Toolbox.Instance.GameManager.WavesController.LiveEnemyCount--;
                    _entity.Explode(destroy: false);
                    _entity.SwitchToCarcas();

                    // Will fire OnDecomposed() when times out.
                }
            }
        }
    }

    public void DropHuman(Human human)
    {
        /* TBD: Need to do something different here */
        Toolbox.Instance.GameManager.ScoreController.BuildScore += human.BuildValue;

    }

    public void DropCarcas(Carcas carcas)
    {
        // TBD: Need to do different things, depending on the type of tower.
        Toolbox.Instance.GameManager.ScoreController.GrowScore += carcas.Entity.BuildValue;
    }
}
