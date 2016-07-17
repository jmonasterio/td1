using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Tower : MonoBehaviour
{
    private Bullet _bullet;
    public Bullet BulletPrefab;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (_bullet == null)
	    {
	        var enempy = FindClosestEnemy(5.0f); // Should be related to bullet range.
	        if (enempy != null)
	        {
	            FireBulletAt(enempy);
	        }
	    }
	}

    private void FireBulletAt(Enemy enempy)
    {
        _bullet = Instantiate<Bullet>(BulletPrefab);
        var here = this.transform.position;
        _bullet.direction = (enempy.transform.position - here).normalized;
        _bullet.transform.position = here;
    }

    private Enemy FindClosestEnemy(float fMax)
    {
        IEnumerable<Enemy> nearby = Toolbox.Instance.GameManager.Enemies();
        if (nearby.Any())
        {
            var here = this.transform.position;

            var found = nearby.OrderBy(_ => (_.transform.position - here).magnitude).First();
            if ((found.transform.position - here).magnitude < fMax)
            {
                return found;
            }
            return null;
        }
        return null;
    }
}
