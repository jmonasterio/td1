using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class Human : MonoBehaviour
{
    public enum HumanClasses
    {
        Standard = 1,
        Gatherer = 2, // Gathers's go get body of dead aliens. Humans get converted into gatherer.
    }

    public Bullet BulletPrefab;
    public HumanClasses HumanClass;
    public int IncomeValue = 10;
    private Entity _entity;
    private Wander _wander;
    private float _dropTime;
    private DragSource _dragSource;

    // Use this for initialization
    void Start()
    {
        _entity = GetComponent<Entity>();
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

    }

    public void DropAt(Vector3 mapExactDrop)
    {
        _dropTime = Time.time;
        _wander.StopWandering();
        this.transform.position = mapExactDrop;
        this.gameObject.layer = GameGrid.BACKGROUND_LAYER;

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

            Debug.Log("Hit!");

            bullet.Destroy();

            // TBD: This is common code.
            if (_entity.Health > 0)
            {
                _entity.Health--;
                if (_entity.Health <= 0)
                {
                    //Toolbox.Instance.GameManager.ScoreController.Score += 100;
                    //Toolbox.Instance.GameManager.WavesController.LiveEnemyCount--;
                    _entity.Explode(destroy: false);
                    //SetAnimState(AnimStates.Carcas);
                    _entity.SwitchToCarcas();
                }
            }
        }
    }
}
