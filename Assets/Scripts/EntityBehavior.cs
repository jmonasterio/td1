using UnityEngine;

public class EntityBehavior : MonoBehaviour
{
    protected Entity _entity;

    public void Start()
    {
        _entity = GetComponent<Entity>();
    }

    public Entity Entity
    {
        get { return _entity;  }
    }
}