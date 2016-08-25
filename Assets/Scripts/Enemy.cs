using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public int FlagCount = 1;
    private Entity _entity;

    // Use this for initialization
    void Start () {
	    this.gameObject.SetActive(false);
        _entity = GetComponent<Entity>();
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
                Destroy(this.gameObject);
            }
        }
    }
}
