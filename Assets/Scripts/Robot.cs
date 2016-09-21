using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class Robot : EntityBehavior
    {
        public Bullet BulletPrefab;
        private DragSource _dragSource;

        // Use this for initialization
        new void Start()
        {
            base.Start();
            _dragSource = GetComponent<DragSource>();
        }

        // Update is called once per frame
        void Update()
        {
            if (_entity.IsReloaded())
            {

                var enemy = _entity.FindClosestLiveEnemy(BulletPrefab.BulletRange);
                // Should be related to bullet range.
                if (enemy != null)
                {
                    _entity.FireBulletAt(enemy, BulletPrefab);
                }
            }

        }

        public void DropHuman(Human human)
        {
            Toolbox.Instance.GameManager.gameObject.GetComponent<ScoreController>().BuildScore += human.BuildValue;


        }

        public void DropCarcas(Carcas carcas)
        {
            // TBD: Need to do different things, depending on the type of tower.
            Toolbox.Instance.GameManager.gameObject.GetComponent<ScoreController>().GrowScore += carcas.Entity.BuildValue;
        }


        void OnTriggerEnter2D(Collider2D collision)
        {
            var colliderGo = collision.gameObject;
            var bullet = colliderGo.GetComponent<Bullet>();

            if (_dragSource != null && _dragSource.Dragging)
            {
                return;
            }

            if (bullet != null)
            {
                // Avoid hit to self
                if (bullet.BulletSource != Entity.EntityClasses.Enemy)
                {
                    // Only enemy bullets hurt robot.
                    return;
                }

                Debug.Log("Hit!");

                bullet.Destroy();

                RobotTookDamage(0.25f);
            }
            else
            {
                // Did we hit an enemy?
                var enemy = colliderGo.GetComponent<Enemy>();
                if (enemy != null)
                {
                    RobotTookDamage(1.0f);
                }
            }
        }

        private void RobotTookDamage( float amountOfDamage)
        {

            if (_entity.Health > 0)
            {
                _entity.Health-= amountOfDamage;
                if (_entity.Health <= 0)
                {
                    _entity.Explode(destroy: false);
                    // TBD: SetAnimState(AnimStates.Carcas);

                    Toolbox.Instance.GameManager.GameController.GameOver();

                    _entity.SwitchToCarcas();

                    // Will fire OnDecomposed() when times out.
                }
            }
        }

    }
}