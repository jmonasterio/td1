using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Tower : MonoBehaviour
{
    public Bullet BulletPrefab;
    public float ReloadTime; // TBD-JM: Should be part of different types of bullets, that are in GUN SLOT
    public float BulletRange;

    private float _reloadDelay;



    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{

        _reloadDelay -= Time.deltaTime;

	    if (_reloadDelay <= 0.0f)
	    {

	        var enempy = FindClosestEnemy(BulletPrefab.BulletRange); // Should be related to bullet range.
	        if (enempy != null)
	        {
	            FireBulletAt(enempy);
	            _reloadDelay = ReloadTime;

	        }
	    }
	}

    private void FireBulletAt(Enemy enempy)
    {
        var bullet = Instantiate<Bullet>(BulletPrefab);
        var here = this.transform.position;
        bullet.direction = (enempy.transform.position - here).normalized;
        bullet.transform.position = here;
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
        
    }
}
