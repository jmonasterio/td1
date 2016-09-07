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

    void OnCollisionEnter2D(Collision2D collision)
    {
        var bulletGo = collision.collider.gameObject;
        var bullet = bulletGo.GetComponent<Bullet>();

        if (bullet != null)
        {
            Debug.Log("Hit!");
           
            bullet.Destroy();

            var health = this.gameObject.GetComponent<Health>();
            _entity.Health--;
            if (_entity.Health <= 0)
            {
                Toolbox.Instance.GameManager.Enemies().Remove(this);
                Toolbox.Instance.GameManager.gameObject.GetComponent<ScoreController>().Score += 100;
                Toolbox.Instance.GameManager.GetComponent<WavesController>().LiveEnemyCount--;
                Destroy(this.gameObject);
            }
        }
    }
}
