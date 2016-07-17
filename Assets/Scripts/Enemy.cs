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
        var bulletGo = collision.collider.gameObject;
        var bullet = bulletGo.GetComponent<Bullet>();

        if (bullet != null)
        {
            Debug.Log("Hit!");
            bullet.Destroy();
        }
    }
}
