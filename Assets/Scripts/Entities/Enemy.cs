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
    void Start ()
    {
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
                var tower = Entity.FindClosestLiveTower(BulletPrefab.BulletRange); // Should be related to bullet range.
                var human = Entity.FindClosestLiveHuman(BulletPrefab.BulletRange);
                if (tower != null && human != null)
                {
                    // pick
                    if (UnityEngine.Random.Range(0.0f, 1.0f) > 0.5f)
                    {
                        Entity.FireBulletAt(tower, BulletPrefab);
                    }
                    else
                    {
                        Entity.FireBulletAt(human, BulletPrefab);
                    }
                }
                else if (tower != null)
                {
                    Entity.FireBulletAt(tower, BulletPrefab);

                }
                else if (human != null)
                {
                    Entity.FireBulletAt(human, BulletPrefab);
                }

            }
        }

    }

    private void PathFollower_AtFinish(object sender, System.EventArgs e)
    {
        if (_pathFollower.TargetCell.GroundType == GameGrid.GameCell.GroundTypes.End)
        {
            Toolbox.Instance.GameManager.LevelController.CurrentLevel.WavesController.LiveEnemyCount--;
            Entity.DestroyAndUpdateGrid();

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

            //Debug.Log("Hit!");

            bool justDied = Entity.TakeDamageFromBullet(bullet);
            if (justDied)
            {
                Toolbox.Instance.GameManager.ScoreController.Score += 100;
                Toolbox.Instance.GameManager.LevelController.CurrentLevel.WavesController.LiveEnemyCount--;
            }

            bullet.Destroy();
        }
        else
        {
            var robot = colliderGo.GetComponent<Robot>();
            if (robot != null)
            {
                Entity.Health = 0; // Kills enemy. Will also STOP it.
                Entity.Explode(destroy: true);
                Toolbox.Instance.GameManager.LevelController.CurrentLevel.WavesController.LiveEnemyCount--;
            }

        }
    }

}
