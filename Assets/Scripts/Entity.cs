using UnityEngine;
using System.Collections;

/// <summary>
/// Common properties that every entity in the system should have.
/// </summary>
public class Entity : MonoBehaviour
{
    public enum EntityClasses
    {
        Background = 0,
        Tower = 1,
        Human = 2,
        Robot = 3,
        Enemy = 4,
        Waypoint = 5,


    }

    public EntityClasses EntityClass;

    /// <summary>
    /// Cost for player to add this type of entity.
    /// </summary>
    public int IncomeCost;

    public int Health = 5;
    public int HealthMax = 5;

    public float Speed = 1.0f;

}


