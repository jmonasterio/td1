using Algorithms;
using Assets.Scripts;
#if UNITY_EDITOR
using UnityEditor;
#endif
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
        if (Event.current.type == EventType.Repaint)
        {

            var gameGrid = Toolbox.Instance.GameManager.LevelController.CurrentLevel.GameGrid;

            if (this.WaypointType == WaypointTypes.Start)
            {
                gameGrid.DrawTextAtGridPoint(this.GridPoint, "Start: " + WaypointGroup, Color.black);
            }
            else if (this.WaypointType == WaypointTypes.End)
            {
                gameGrid.DrawTextAtGridPoint(this.GridPoint, "End: " + WaypointGroup, Color.black);

            }
        }
    }

}
