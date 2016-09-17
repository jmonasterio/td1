using System;
using UnityEngine;
using System.Collections;
using Assets.Scripts;
using Object = UnityEngine.Object;

public class Enemy : MonoBehaviour {

    public enum EnemyClasses
    {
        Standard = 1,
    }

    public EnemyClasses EnemyClass;
    public Bullet BulletPrefab;
    public int CarcasIncomeValue = 10;

    private Entity _entity;
    private PathFollower _pathFollower;
    private Animator _animator;

    public enum AnimStates
    {
        Alive = 0,
        __UNUSED__ = 1,
        Healing = 2,
        Carcas = 4,
        Wounded = 3,
        Decomposing = 5,
        Dead = 6,
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
        _entity = GetComponent<Entity>();
        _entity.Decomposed += _entity_Decomposed; 
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
            if (_entity.IsAlive() && _entity.IsReloaded())
            {
                // TBD: Could do better job of targetting closes HUMAN or TOWER, for example.

                var tower = _entity.FindClosestLiveTower(BulletPrefab.BulletRange); // Should be related to bullet range.
                if (tower != null)
                {
                    _entity.FireBulletAt(tower, BulletPrefab);

                }
                else
                {
                    var human = _entity.FindClosestLiveHuman(BulletPrefab.BulletRange);
                    if (human != null)
                    {
                        _entity.FireBulletAt(human, BulletPrefab);
                    }


                }
            }
        }

    }

    private void _entity_Decomposed(object sender, EventArgs e)
    {
        // TBD: Sounds or graphics here.
        Destroy(this.gameObject);
    }

    // If at end, head back to start. Or if at START, then done.
    private void PathFollower_AtFinish(object sender, System.EventArgs e)
    {
        // TBD-Put in event handler for start to end follower.
        if (_pathFollower.TargetCell.GroundType == GameGrid.GameCell.GroundTypes.End)
        {
            Toolbox.Instance.GameManager.ScoreController.EnemyScored(1);
            Toolbox.Instance.GameManager.WavesController.LiveEnemyCount--;
            Object.Destroy(this.gameObject);

            // TBD: Put damage on robot here.

        }
    }




    void OnTriggerEnter2D(Collider2D collision)
    {
        var colliderGo = collision.gameObject;
        var bullet = colliderGo.GetComponent<Bullet>();

        if (bullet != null)
        {
            // Avoid hit to self
            if (bullet.BulletSource.GetComponent<Enemy>() != null)
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
                    Toolbox.Instance.GameManager.ScoreController.Score += 100;
                    Toolbox.Instance.GameManager.WavesController.LiveEnemyCount--;
                    _entity.Explode(destroy: false);
                    SetAnimState(AnimStates.Carcas);
                    _entity.StartDecomposing(Time.time);

                    // Will fire OnDecomposed() when times out.
                }
            }
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

    // TBD: Probably should be state. On an interface???
    public bool IsAlive()
    {
        return _entity.IsAlive();
    }
}
