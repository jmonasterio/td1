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

    private float _reloadDelay;
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


	    _reloadDelay -= Time.deltaTime;

        if (_dragSource != null && _dragSource.Dragging)
        {
            return;
        }

	    if (TowerClass == TowerClasses.Shooter)
	    {
	        if (_reloadDelay <= 0.0f)
	        {

	            var enempy = FindClosestEnemy(BulletPrefab.BulletRange); // Should be related to bullet range.
	            if (enempy != null)
	            {
	                FireBulletAt(enempy);
	                _reloadDelay = _entity.ReloadTime;;

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

    private Enemy FindClosestEnemy(float fMax)
    {
        IEnumerable<Enemy> nearby = Toolbox.Instance.GameManager.Enemies();
        if (nearby.Any())
        {
            var here = this.transform.position;

            var found = nearby.OrderBy(_ => (_.transform.position - here).magnitude).FirstOrDefault();
            if (found == null)
            {
                return null;
            }
            if ((found.transform.position - here).magnitude < fMax)
            {
                return found;
            }
            return null;
        }
        return null;
    }

    public void DropHuman(Human human)
    {
        /* TBD: Need to do something different here */
        Toolbox.Instance.GameManager.gameObject.GetComponent<ScoreController>().Income += human.IncomeValue * 1;

    }
}
