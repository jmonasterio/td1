using UnityEngine;
using System.Collections;

public class Robot : MonoBehaviour
{
    private Entity _entity;

    public Bullet BulletPrefab;

    // Use this for initialization
    void Start()
    {
        _entity = GetComponent <Entity>();
        _entity.Decomposed += _entity_Decomposed;
    }

    private void _entity_Decomposed(object sender, System.EventArgs e)
    {
        // TBD: Sounds and graphics. 
        // TBD: Try /catch
        Destroy(this.gameObject);
        Toolbox.Instance.GameManager.GameController.GameOver();
    }


    // Update is called once per frame
    void Update()
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

    public void DropHuman(Human human)
    {
        Toolbox.Instance.GameManager.gameObject.GetComponent<ScoreController>().Income += human.IncomeValue*2;


    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        var colliderGo = collision.gameObject;
        var bullet = colliderGo.GetComponent<Bullet>();

        bool robotDestroying = false;

        if (bullet != null)
        {
            // Avoid hit to self
            if (bullet.BulletSource.GetComponent<Enemy>() == null)
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
                _entity.StartDecomposing(Time.time);

                // Will fire OnDecomposed() when times out.
            }
        }
    }
}
