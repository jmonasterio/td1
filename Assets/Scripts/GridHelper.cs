using Algorithms;
using UnityEngine;

namespace Assets.Scripts
{
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

        public static Vector3 MapPointToVector(GameObject go, GridPoint p)
        {
            var gr = GetInternalGridRect(go);
            var v = gr.min + new Vector2(p.X, p.Y);
            return v;
        }


    }
}