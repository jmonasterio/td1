using System;
using UnityEngine;
using System.Collections;
using Assets.Scripts;
using UnityEditor;

public class Human : EntityBehavior
{
    public enum HumanClasses
    {
        Standard = 1,
        Gatherer = 2, // Gathers's go get body of dead aliens. Humans get converted into gatherer.
    }

    [Serializable]
    public class GathererState
    {

        /// <summary>
        /// A gatherer should grab carcas and keep "grow" value of the carcas here until deposited in city.
        /// </summary>
        public float GrowValue;

        public float Rate;
        public float MaxGrowValue;

        public GathererState()
        {
            GrowValue = 0.0f;
            Rate = 1.0f;
            MaxGrowValue = 2.0f;
        }
    }


    public HumanClasses HumanClass;
    public Bullet BulletPrefab;
    public GathererState GatherState;

    [Header("test")]
    public float BuildValue = 10.0f;

    private Wander _wander;
    private float _dropTime;
    private DragSource _dragSource;

    // Use this for initialization
    new void Start()
    {
        _wander = GetComponent<Wander>();
        _dragSource = GetComponent<DragSource>();

        if (HumanClass == HumanClasses.Gatherer)
        {
            _wander.WanderMode = Wander.WanderModes.ToCarcas;
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (_dragSource != null && _dragSource.Dragging)
        {
            return;
        }

        const float DELAY_BEFORE_NEXT_PATH_MOVE = 0.3f;
        // After being dropped, wait 0.5 seconds before wandering again.
        if (_dropTime != 0 && Time.time > _dropTime + DELAY_BEFORE_NEXT_PATH_MOVE)
        {
            _wander.RestartWandering();
            _dropTime = 0.0f;
        }

        if (HumanClass == HumanClasses.Standard)
        {
            if (Entity.IsReloaded())
            {

                var enemy = Entity.FindClosestLiveEnemy(BulletPrefab.BulletRange);
                // Should be related to bullet range.
                if (enemy != null)
                {
                    Entity.FireBulletAt(enemy, BulletPrefab);
                }
            }
        }
        else if (HumanClass == HumanClasses.Gatherer)
        {
            var cell = Entity.GetCurrentGameCell();
            if (cell == null)
            {
                // This happens, because the HUMAN component seems to "Update()" before the ENTITY component.
                Debug.LogWarning("Could not find cell for gatherer.");
            }
            else
            {
                if (cell.Carcases.Count > 0)
                {

                    if (GatherState.GrowValue < GatherState.MaxGrowValue)
                    {
                        if (!_wander.IsStopped)
                        {
                            _wander.StopWandering(); // TBD: Maybe only stop really near the carcas.
                        }

                        float wantToGatherThisMuch = (GatherState.MaxGrowValue - GatherState.GrowValue)*GatherState.Rate*
                                                     Time.deltaTime;
                        foreach (var carcas in cell.Carcases.ToArray()) // Switch to ARRAY, because we delete items in loop
                        {
                            if (carcas.Entity.BuildValue > wantToGatherThisMuch)
                            {
                                this.GatherState.GrowValue += wantToGatherThisMuch;
                                carcas.Entity.BuildValue -= wantToGatherThisMuch;
                                break;
                            }
                            else
                            {

                                float availableInThisCarcas = carcas.Entity.BuildValue;
                                this.GatherState.GrowValue += availableInThisCarcas;
                                wantToGatherThisMuch -= availableInThisCarcas;
                                carcas.Entity.DestroyAndUpdateGrid();
                            }
                        }
                    }
                }

                if (cell.Tower != null)
                {
                    if (cell.Tower.TowerClass != Tower.TowerClasses.GathererTower)
                    {
                        Debug.LogError("Only really works for gatherer towers.");
                    }
                    if (this.GatherState.GrowValue > 0)
                    {
                        // TBD: Unloading should take a little time???
                        // TBD: Need some sounds an graphics when the gather deposits into a tower.
                        Debug.Log("Added growth to tower.");
                        cell.Tower.AvailableGrowPower += this.GatherState.GrowValue;
                        this.GatherState.GrowValue = 0.0f;
                        _wander.StopWandering();

                    }
                }

                if (_wander.IsStopped)
                {
                    if ((GatherState.GrowValue >= GatherState.MaxGrowValue))
                    {
                        if (cell.Tower != null)
                        {
                            _wander.WanderMode = Wander.WanderModes.ToGathererCity;
                            _wander.RestartWandering();
                        }
                        else
                        {
                            Debug.LogWarning("Should have dumped all our growValue into tower up above." ); // TBD: May happen if we take a while to unload.
                        }
                    }
                    else if (cell.Carcases.Count == 0)
                    {
                        // Otherwise, will just keep wandering back to this city.
                        _wander.WanderMode = Wander.WanderModes.ToCarcas;
                        _wander.RestartWandering();
                    }
                    else
                    {
                        // Just sit here collecting more.
                        Debug.Assert(cell.Carcases.Count > 0);
                    }
                }
            }
        }
    }

    public void DropAt(Vector3 mapExactDrop)
    {
        _dropTime = Time.time;
        _wander.StopWandering();
        this.transform.position = mapExactDrop;
        this.gameObject.layer = GameGrid.BACKGROUND_LAYER;

    }

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

            Debug.Log("Hit!");

            bool justDied = Entity.TakeDamageFromBullet(bullet);
            if (justDied)
            {
                // TBD: Change score or reduce counts, or whatever.
            }

            bullet.Destroy();
        }
    }
}
