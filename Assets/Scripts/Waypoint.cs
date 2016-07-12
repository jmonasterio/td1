using Algorithms;
using UnityEngine;


// TBD: Would be nice to be able to have multiple starts, and associated end.

[RequireComponent(typeof (SpriteRenderer))]
public class Waypoint : MonoBehaviour
{
    public enum WaypointTypes
    {
        Start,
        End, // This is the goal.
        Waypoint
    }

    public WaypointTypes WaypointType = WaypointTypes.Start;

    /// <summary>
    /// Only applies when type is WAYPOINT (not start or end).
    /// </summary>
    public int WaypointIndex = 0; // 0 = start, 

    public GridPoint GridPoint;
}
