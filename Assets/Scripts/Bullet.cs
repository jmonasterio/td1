using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    private Vector3 _bulletStart;
    public Vector3 direction;
    public float BulletSpeed;
    public float BulletRange;

    public Entity.EntityClasses BulletSource;

    // Use this for initialization
    void Start()
    {
        _bulletStart = this.transform.position;
    }


    // Update is called once per frame
    void Update()
    {

        float dt = Time.deltaTime;



        this.transform.position = this.transform.position + direction*BulletSpeed*dt;

        if ((this.transform.position - _bulletStart).magnitude > BulletRange)
        {
            Destroy();
        }
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }


}

