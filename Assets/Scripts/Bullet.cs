﻿using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    private Vector3 _bulletStart;
    public Vector3 direction;
    public float BulletSpeed;
    public float BulletRange;
    private Transform _bulletsCollection;

    public Entity BulletSource;

    // Use this for initialization
    void Start()
    {
        _bulletsCollection = GameObject.Find("Bullets").transform; // TBD: Maybe do this in the in the Enemy object.
        _bulletStart = this.transform.position;
    }


    // Update is called once per frame
    void Update()
    {

        float dt = Time.deltaTime;



        this.transform.position = this.transform.position + direction*BulletSpeed*dt;
        this.transform.SetParent( _bulletsCollection);

        if ((this.transform.position - _bulletStart).magnitude > BulletRange)
        {
            Destroy();
        }
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Each object decides what to do if it hits a bullet.
#if OLD_WAY
        var bullet = collision.collider.gameObject;
        if (bullet.GetComponent<Enemy>() != null)
        {
            Debug.Log("Hit! Enemy");
            Destroy(this.gameObject);

        }
#endif
    }

}

