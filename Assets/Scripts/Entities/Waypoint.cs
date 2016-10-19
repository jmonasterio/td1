using Algorithms;
using UnityEditor;
using UnityEngine;


//[ExecuteInEditMode]
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

    public int WaypointGroup = 0;

    // Update is called once per frame
    void OnGUI()
    {
        if (Application.isPlaying)
        {
           this.GetComponent<SpriteRenderer>().enabled = false;
           return;
        }

        var vector = this.transform.position;
        var color = GUI.color;
        GUI.color = Color.black;
        if (this.WaypointType == WaypointTypes.Start)
        {
            Handles.Label(vector, "Start: " + WaypointGroup);
        }
        else if (this.WaypointType == WaypointTypes.End)
        {
            Handles.Label(vector, "End: " + WaypointGroup);

        }
        GUI.color = color;

    }

}
