using System;
using UnityEngine;
using System.Collections;
using Assets.Scripts;
using Object = UnityEngine.Object;

public class Enemy : EntityBehavior {
    public enum EnemyClasses
    {
        Standard = 1,
    }

    public EnemyClasses EnemyClass;
    public Bullet BulletPrefab;

    private PathFollower _pathFollower;
    private Animator _animator;
    private DragSource _dragSource;

    public enum AnimStates
    {
        Alive = 0,
        __UNUSED__ = 1,
        Healing = 2,
        Wounded = 3,
    }

    public PathFollower PathFollower
    {
        get { return GetComponent<PathFollower>(); }
    }

    public void SetAnimState(AnimStates animState)
    {
        _animator.SetInteger("AnimState", (int) animState );
        //string stateName = animState.ToString();
        //_animator.Play(stateName);
    }

    
    // Use this for initialization
    new void Start ()
    {
        base.Start();
        if (this.GetComponentInParent<Wave>() != null)
        {
            this.gameObject.SetActive(false); // So enemies in the Waves controller start disabled.
        }
        _dragSource = GetComponent<DragSource>();
        _animator = GetComponent<Animator>();
        _pathFollower = GetComponent<PathFollower>();


        _pathFollower.AtFinish -= PathFollower_AtFinish;
        _pathFollower.AtFinish += PathFollower_AtFinish;

        SetAnimState(Enemy.AnimStates.Alive); // Starting animation.

        //_pathFollower.Blocked -= PathFollowerStartToEnd_Blocked;
        //_pathFollower.Blocked += PathFollowerStartToEnd_Blocked;
    }

    public void Update()
    {
        if (EnemyClass == EnemyClasses.Standard)
        {
            if ( Entity.IsReloaded())
            {
                var tower = _entity.FindClosestLiveTower(BulletPrefab.BulletRange); // Should be related to bullet range.
                var human = _entity.FindClosestLiveHuman(BulletPrefab.BulletRange);
                if (tower != null && human != null)
                {
                    // pick
                    if (UnityEngine.Random.Range(0.0f, 1.0f) > 0.5f)
                    {
                        _entity.FireBulletAt(tower, BulletPrefab);
                    }
                    else
                    {
                        _entity.FireBulletAt(human, BulletPrefab);
                    }
                }
                else if (tower != null)
                {
                    _entity.FireBulletAt(tower, BulletPrefab);

                }
                else if (human != null)
                {
                    _entity.FireBulletAt(human, BulletPrefab);
                }

            }
        }

    }

    private void PathFollower_AtFinish(object sender, System.EventArgs e)
    {
        if (_pathFollower.TargetCell.GroundType == GameGrid.GameCell.GroundTypes.End)
        {
            Toolbox.Instance.GameManager.WavesController.LiveEnemyCount--;
            Object.Destroy(this.gameObject);

            // Damage to robot happens in collision??? TBD
        }
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
            if (bullet.BulletSource == Entity.EntityClasses.Enemy)
            {
                // Enemy bullets should not hurt enemies.
                return;
            }

            Debug.Log("Hit!");

            bool justDied = Entity.TakeDamageFromBullet(bullet);
            if (justDied)
            {
                Toolbox.Instance.GameManager.ScoreController.Score += 100;
                Toolbox.Instance.GameManager.WavesController.LiveEnemyCount--;
            }

            bullet.Destroy();
        }
        else
        {
            var robot = colliderGo.GetComponent<Robot>();
            if (robot != null)
            {
                _entity.Health = 0; // Kills enemy. Will also STOP it.
                _entity.Explode(destroy: true);
                Toolbox.Instance.GameManager.WavesController.LiveEnemyCount--;
            }

        }
    }

}
