using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Tower : MonoBehaviour
{

    public enum TowerClasses
    {
        Shooter =1,
        City = 2,
        GathererTower = 3, // Gathers's go get body of dead aliens. Humans get converted into gatherer.
    }

    public Bullet BulletPrefab;
    public float BulletRange;
    public TowerClasses TowerClass;

    private DragSource _dragSource;
    private Entity _entity;


    // Use this for initialization
    void Start ()
    {
        _entity = GetComponent<Entity>();
        _dragSource = GetComponent<DragSource>();
    }
	
	// Update is called once per frame
	void Update ()
	{



        if (_dragSource != null && _dragSource.Dragging)
        {
            return;
        }

	    if (TowerClass == TowerClasses.Shooter)
	    {
	        if (_entity.IsReloaded())
	        {

	            var enemy = _entity.FindClosestEnemy(BulletPrefab.BulletRange); // Should be related to bullet range.
	            if (enemy != null)
	            {
	                FireBulletAt(enemy);
	                _entity.StartReload();

	            }
	        }
	    }
	    else if( TowerClass == TowerClasses.GathererTower)
	    {
	        // TBD-DARRIN: When should gather towers shoot? When they're not birthing a gatherer? Never?
	    }
	}

    private void FireBulletAt(Enemy enempy)
    {
        var bullet = Instantiate<Bullet>(BulletPrefab);
        var here = this.transform.position;
        bullet.direction = (enempy.transform.position - here).normalized;
        bullet.transform.position = here;
        bullet.BulletSource = _entity;
    }


    public void DropHuman(Human human)
    {
        /* TBD: Need to do something different here */
        Toolbox.Instance.GameManager.gameObject.GetComponent<ScoreController>().Income += human.IncomeValue * 1;

    }
}
