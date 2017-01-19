using Algorithms;
using Assets.Scripts;
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
    private TextMesh _wpText;

    // Update is called once per frame
    void Start()
    {
        var go2 = new GameObject("WpText");
        go2.transform.SetParent(this.transform);
        go2.AddComponent<MeshRenderer>();
        _wpText = go2.AddComponent<TextMesh>();
        switch (WaypointType)
        {
            case WaypointTypes.Start: _wpText.text = "Start:"+transform.parent.name; break;
            case WaypointTypes.End: _wpText.text = "End:" + transform.parent.name; break;
            case WaypointTypes.Waypoint: _wpText.text = "X:" + transform.parent.name; break;
        }
        _wpText.richText = false;
        _wpText.characterSize = 1;
        _wpText.color = Color.red;
        _wpText.transform.localPosition = new Vector3(0,1,-2);
        _wpText.transform.localScale = new Vector3(5,5,5);
    }

}
