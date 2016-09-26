using UnityEngine;

public class EntityBehavior : MonoBehaviour
{
    private Entity _entity;

    public Entity Entity
    {
        get
        {
            if (_entity == null)
            {
                _entity = GetComponent<Entity>();
            }
            return _entity;
        }
    }
}