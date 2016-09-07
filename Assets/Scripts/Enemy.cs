using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class Enemy : MonoBehaviour {

    public int FlagCount = 1;
    private Entity _entity;
    private PathFollower _pathFollower;

    // Use this for initialization
    void Start () {
	    this.gameObject.SetActive(false);
        _entity = GetComponent<Entity>();
        _pathFollower = GetComponent<PathFollower>();


        _pathFollower.AtFinish -= PathFollower_AtFinish;
        _pathFollower.AtFinish += PathFollower_AtFinish;

        //_pathFollower.Blocked -= PathFollowerStartToEnd_Blocked;
        //_pathFollower.Blocked += PathFollowerStartToEnd_Blocked;
    }

    // If at end, head back to start. Or if at START, then done.
    private void PathFollower_AtFinish(object sender, System.EventArgs e)
    {
        // TBD-Put in event handler for start to end follower.
        if (_pathFollower.TargetCell.GroundType == GameGrid.GameCell.GroundTypes.End)
        {
            var enempyCmp = this.GetComponent<Enemy>();
            Toolbox.Instance.GameManager.GetComponent<ScoreController>().EnemyScored(enempyCmp.FlagCount);
            Toolbox.Instance.GameManager.GetComponent<WavesController>().LiveEnemyCount--;
            Object.Destroy(this.gameObject);

            // TBD: Put damage on robot here.
        }
    }



    // Update is called once per frame
    void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D collision)
    {
        var colliderGo = collision.gameObject;
        var bullet = colliderGo.GetComponent<Bullet>();

        if (bullet != null)
        {
            Debug.Log("Hit!");
           
            bullet.Destroy();

            if (_entity.Health > 0)
            {
                _entity.Health--;
                if (_entity.Health <= 0)
                {
                    Toolbox.Instance.GameManager.Enemies().Remove(this);
                    Toolbox.Instance.GameManager.gameObject.GetComponent<ScoreController>().Score += 100;
                    Toolbox.Instance.GameManager.GetComponent<WavesController>().LiveEnemyCount--;
                    _entity.Explode(destroy: true);

                    /* TBD: Start decomposing */
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

                var robotEntity = robot.GetComponent<Entity>();
                if (robotEntity.Health > 0)
                {
                    robotEntity.Health--;
                    if (robotEntity.Health == 0)
                    {
                        robotEntity.Explode( destroy:true);
                        Debug.Log("Game over.");
                    }
                }

            }
        }
    }
}
