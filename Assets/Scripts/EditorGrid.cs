using UnityEngine;
using System.Collections;
using Algorithms;

public static class GridHelper
{
    // Return a rectangle for that bounds the points insite a sprite where all integer grid points are found (no more than 0.5 from edge of sprite)
    public static Rect GetInternalGridRect(GameObject go)
    {
        var spriteBounds = go.GetComponent<SpriteRenderer>().bounds;

        Vector3 start = spriteBounds.min;
        Vector3 end = spriteBounds.max;

        var xMin = Mathf.FloorToInt(start.x + 0.5f) + 1;
        var yMin = Mathf.FloorToInt(start.y + 0.5f) + 1;
        var xMax = Mathf.FloorToInt(end.x - 0.5f) + 1;
        var yMax = Mathf.FloorToInt(end.y - 0.5f) + 1;
        Rect rc = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
        return rc;
    }

    public static Vector3 MapPointToVector(GameObject go, Point p)
    {
        var gr = GetInternalGridRect(go);
        var v = gr.min + new Vector2(p.X, p.Y);
        return v;
    }


}

[ExecuteInEditMode]
public class EditorGrid : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {
        if (!Application.isPlaying || Toolbox.Instance.DebugSys.ShowGridAtRuntime )
        {
            var rcInternalGrid = GridHelper.GetInternalGridRect(this.gameObject);

            DrawGridAroundIntegerPoints(rcInternalGrid);
        }
    }

    private void DrawGridAroundIntegerPoints(Rect rcInternalGrid)
    {
        for (int xx = (int)rcInternalGrid.xMin; xx <= rcInternalGrid.xMax; xx++)
        {
            Vector3 ss = new Vector3(xx - 0.5f, (rcInternalGrid.yMin - 0.5f), 0);
            Vector3 ee = new Vector3(xx - 0.5f, (rcInternalGrid.yMax - 0.5f), 0);

            Debug.DrawLine(ss, ee, Color.green, 0.1f);
        }

        for (int yy = (int)rcInternalGrid.yMin; yy <= rcInternalGrid.yMax; yy++)
        {
            Vector3 ss = new Vector3(rcInternalGrid.xMin - 0.5f, yy - 0.5f, 0);
            Vector3 ee = new Vector3(rcInternalGrid.xMax - 0.5f, yy - 0.5f, 0);

            Debug.DrawLine(ss, ee, Color.green, 0.1f);
        }
        //Debug.DrawLine( start, end, Color.magenta, 2f);
        //Debug.DrawLine( new Vector3(0,0,0), end, Color.red );

    }
}
