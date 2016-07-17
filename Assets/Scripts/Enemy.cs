using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter2D(Collision2D collision)
    {
        var bullet = collision.collider.gameObject;
        if (bullet.GetComponent<Bullet>() != null)
        {
            Debug.Log("Hit!");
        }
    }
}
